using System.Collections.Generic;
using System.Threading.Tasks;
using Glasswall.Administration.K8.TransactionEventApi.Common.Models.V1;

namespace Glasswall.Administration.K8.TransactionEventApi.Common.Services
{
    public interface ITransactionService
    {
        Task<IEnumerable<GetTransactionsResponseV1>> GetTransactionsAsync(GetTransactionsRequestV1 requestV1);
        Task<GetDetailResponseV1> GetDetailAsync(string fileDirectory);
    }
}