using Glasswall.Administration.K8.TransactionEventApi.Common.Enums;

namespace Glasswall.Administration.K8.TransactionEventApi.Business.Enums
{
    public static class RiskAssessment
    {
        public static Risk DetermineRisk(NCFSOutcome ncfsOutcome, GwOutcome gwOutcome)
        {
            Risk risk;
            switch (ncfsOutcome)
            {
                case NCFSOutcome.Relayed:
                    risk = Risk.AllowedByNCFS;
                    break;
                case NCFSOutcome.Replaced:
                    risk = Risk.Safe;
                    break;
                case NCFSOutcome.Blocked:
                    risk = Risk.BlockedByNCFS;
                    break;
                default:
                    risk = Risk.Unknown;
                    break;
            }

            if (risk != Risk.Unknown) return risk;

            switch (gwOutcome)
            {
                case GwOutcome.Replace:
                    risk = Risk.Safe;
                    break;
                case GwOutcome.Unmodified:
                    risk = Risk.AllowedByPolicy;
                    break;
                case GwOutcome.Failed:
                    risk = Risk.BlockedByPolicy;
                    break;
                default:
                    risk = Risk.Unknown;
                    break;
            }

            return risk;
        }
    }
}