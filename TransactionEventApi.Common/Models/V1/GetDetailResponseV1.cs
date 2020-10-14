using Glasswall.Administration.K8.TransactionEventApi.Common.Enums;
using Glasswall.Administration.K8.TransactionEventApi.Common.Models.AnalysisReport;

namespace Glasswall.Administration.K8.TransactionEventApi.Common.Models.V1
{
    public class GetDetailResponseV1
    {
        public DetailStatus Status { get; set; }
        public GWallInfo AnalysisReport { get; set; }
    }
}