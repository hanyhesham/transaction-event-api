using System;
using System.ComponentModel.DataAnnotations;

namespace Glasswall.Administration.K8.TransactionEventApi.Common.Models.V1
{
    public class GetTransactionsRequestV1
    {
        [Required]
        public FileStoreFilterV1 Filter { get; set; }
    }

    public class FileStoreFilterV1
    {
        [Required]
        public DateTimeOffset? TimestampRangeStart { get; set; }

        [Required]
        public DateTimeOffset? TimestampRangeEnd { get; set; }
    }
}