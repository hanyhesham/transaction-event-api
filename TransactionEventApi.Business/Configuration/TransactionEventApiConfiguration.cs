using Glasswall.Administration.K8.TransactionEventApi.Common.Configuration;

namespace Glasswall.Administration.K8.TransactionEventApi.Business.Configuration
{
    public class TransactionEventApiConfiguration : ITransactionEventApiConfiguration
    {
        public string TransactionStoreConnectionStringCsv { get; set; }
        public string ShareName { get; set; }
    }
}