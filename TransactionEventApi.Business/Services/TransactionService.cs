using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Glasswall.Administration.K8.TransactionEventApi.Business.Store;
using Glasswall.Administration.K8.TransactionEventApi.Common.Enums;
using Glasswall.Administration.K8.TransactionEventApi.Common.Models.AnalysisReport;
using Glasswall.Administration.K8.TransactionEventApi.Common.Models.V1;
using Glasswall.Administration.K8.TransactionEventApi.Common.Serialisation;
using Glasswall.Administration.K8.TransactionEventApi.Common.Services;
using Microsoft.Extensions.Logging;
using EventId = Glasswall.Administration.K8.TransactionEventApi.Common.Enums.EventId;

namespace Glasswall.Administration.K8.TransactionEventApi.Business.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ILogger<ITransactionService> _logger;
        private readonly IEnumerable<IFileShare> _fileShares;
        private readonly IJsonSerialiser _jsonSerialiser;
        private readonly IXmlSerialiser _xmlSerialiser;

        public TransactionService(
            ILogger<ITransactionService> logger, 
            IEnumerable<IFileShare> fileStores,
            IJsonSerialiser jsonSerialiser,
            IXmlSerialiser xmlSerialiser)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _fileShares = fileStores ?? throw new ArgumentNullException(nameof(fileStores));
            _jsonSerialiser = jsonSerialiser ?? throw new ArgumentNullException(nameof(jsonSerialiser));
            _xmlSerialiser = xmlSerialiser ?? throw new ArgumentNullException(nameof(xmlSerialiser));
        }

        public Task<IEnumerable<GetTransactionsResponseV1>> GetTransactionsAsync(GetTransactionsRequestV1 request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            return InternalGetTransactionsAsync(request);
        }

        public Task<GetDetailResponseV1> GetDetailAsync(string fileDirectory)
        {
            if (string.IsNullOrWhiteSpace(fileDirectory))
                throw new ArgumentException("Value must not be null or whitespace", nameof(fileDirectory));

            return InternalTryGetDetailAsync(fileDirectory);
        }

        private async Task<GetDetailResponseV1> InternalTryGetDetailAsync(string fileDirectory)
        {
            GWallInfo analysisReport = null;
            var status = DetailStatus.FileNotFound;

            foreach (var fileStore in _fileShares)
            {
                if (!await fileStore.ExistsAsync(fileDirectory)) continue;

                var fullPath = $"{fileDirectory}/report.xml";

                var analysisReportStream = await fileStore.DownloadAsync(fullPath);

                if (analysisReportStream == null)
                {
                    status = DetailStatus.AnalysisReportNotFound;
                    break;
                }

                analysisReport = await _xmlSerialiser.Deserialize<GWallInfo>(analysisReportStream, Encoding.UTF8);
                status = DetailStatus.Success;
                break;
            }

            return new GetDetailResponseV1
            {
                Status = status,
                AnalysisReport = analysisReport
            };
        }

        private async Task<IEnumerable<GetTransactionsResponseV1>> InternalGetTransactionsAsync(GetTransactionsRequestV1 request)
        {
            _logger.LogInformation("Searching file store ");

            var responseItems = new List<GetTransactionsResponseV1>();

            foreach (var share in _fileShares)
            {
                await foreach(var fileDirectory in share.ListAsync(new DatePathFilter(request.Filter)))
                {
                    using var ms = await share.DownloadAsync($"{fileDirectory}/metadata.json");

                    var eventFile = await _jsonSerialiser.Deserialize<TransactionAdapationEventMetadataFile>(ms, Encoding.UTF8);
                    var newDocumentEvent = eventFile.Events.EventOrDefault(EventId.NewDocument);
                    var fileTypeDetectedEvent = eventFile.Events.EventOrDefault(EventId.FileTypeDetected);

                    Guid.TryParse(newDocumentEvent.PropertyOrDefault("PolicyId"), out var policyId);
                    Guid.TryParse(newDocumentEvent.PropertyOrDefault("FileId"), out var fileId);
                    DateTimeOffset.TryParse(newDocumentEvent.PropertyOrDefault("Timestamp"), out var timestamp);

                    responseItems.Add(new GetTransactionsResponseV1
                    {
                        ActivePolicyId = policyId,
                        DetectionFileType = fileTypeDetectedEvent.PropertyOrDefault<FileType>("FileType").GetValueOrDefault(FileType.Unknown),
                        FileId = fileId,
                        Risk = Risk.Safe, // TBD - need to work out logic
                        Timestamp = timestamp,
                        Directory = fileDirectory
                    });
                }
            }

            return responseItems;
        }
    }
}