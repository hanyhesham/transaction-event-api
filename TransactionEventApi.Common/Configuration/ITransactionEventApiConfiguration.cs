namespace Glasswall.Administration.K8.TransactionEventApi.Common.Configuration
{
    public interface ITransactionEventApiConfiguration
    {
        string TransactionStoreConnectionStringCsv { get; }
        string ShareName { get; }
    }
}