using System;
using System.Diagnostics.CodeAnalysis;
using Glasswall.Administration.K8.TransactionEventApi.Common.Enums;

namespace Glasswall.Administration.K8.TransactionEventApi.Common.Models.V1
{
    public class GetTransactionsResponseV1
    {
        public DateTimeOffset Timestamp { get; set; }
        public Guid FileId { get; set; }
        public FileType DetectionFileType { get; set; }
        public Risk Risk { get; set; }
        public Guid ActivePolicyId { get; set; }
        public string Directory { get; set; }
    }
}