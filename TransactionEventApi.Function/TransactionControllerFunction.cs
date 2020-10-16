using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Glasswall.Administration.K8.TransactionEventApi.Common.Models.V1;
using Glasswall.Administration.K8.TransactionEventApi.Common.Serialisation;
using Glasswall.Administration.K8.TransactionEventApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

[assembly: FunctionsStartup(typeof(TransactionEventApi.Function.Startup))]

namespace TransactionEventApi.Function
{
    [ExcludeFromCodeCoverage]
    public class TransactionControllerFunction
    {
        private readonly IJsonSerialiser _jsonSerialiser;
        private readonly TransactionController _controller;

        public TransactionControllerFunction(
            IJsonSerialiser jsonSerialiser,
            TransactionController controller)
        {
            _jsonSerialiser = jsonSerialiser ?? throw new ArgumentNullException(nameof(jsonSerialiser));
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
        }

        [FunctionName("transactions")]
        public async Task<IActionResult> GetTransactions(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "v1/transactions")] HttpRequest req,
            ILogger log)
        {
            if (req.Method.ToUpper() == "POST")
            {
                var request = await req.ReadAsStringAsync();

                var requestDeserialized = await _jsonSerialiser.Deserialize<GetTransactionsRequestV1>(request);

                return await _controller.GetTransactions(requestDeserialized);
            }

            var filePath = req.GetQueryParameterDictionary()["filePath"];
            return await _controller.GetDetail(filePath);
        }
    }
}
