using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Glasswall.Administration.K8.TransactionEventApi.Common.Models.V1;
using Glasswall.Administration.K8.TransactionEventApi.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Glasswall.Administration.K8.TransactionEventApi.Controllers
{
    [ApiController]
    [Route("api/v1/transactions")]
    public class TransactionController : ControllerBase
    {
        private readonly ILogger<TransactionController> _logger;
        private readonly ITransactionService _transactionService;

        public TransactionController(ILogger<TransactionController> logger, ITransactionService transactionService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _transactionService = transactionService ?? throw new ArgumentNullException(nameof(transactionService));
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> GetTransactions([Required][FromBody]GetTransactionsRequestV1 request)
        {
            _logger.LogInformation("Beginning get transactions request");

            var transactions = await _transactionService.GetTransactionsAsync(request);

            _logger.LogInformation("Finished get transactions request");

            return Ok(transactions);
        }

        [HttpGet]
        [ValidateModel]
        public async Task<IActionResult> GetDetail([Required] [FromQuery] string filePath)
        {
            _logger.LogInformation("Beginning get detail request");

            var detail = await _transactionService.GetDetailAsync(filePath);

            _logger.LogInformation("Finished get detail request");

            return Ok(detail);
        }
    }
}
