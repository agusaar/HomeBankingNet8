using HomeBankingNet8.DTOs;
using HomeBankingNet8.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeBankingNet8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        public class AccountsController : ControllerBase
        {
            private readonly ITransactionRepository _transactionRepository;
            public AccountsController(ITransactionRepository transactionRepository)
            {
                _transactionRepository = transactionRepository;
            }

            [HttpGet]
            public IActionResult getAll()
            {
                try
                {
                    var transactions = _transactionRepository.GetAllAccounts();
                    var transactionsDTO = transactions.Select(tr => new TransactionDTO(tr)).ToList();
                    return Ok(transactionsDTO);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
            }

            [HttpGet("{id}")]
            public IActionResult Get(long id)
            {
                try
                {
                    var transaction = _transactionRepository.FindById(id);
                    if (transaction == null)
                    {
                        return NotFound();
                    }
                    var transactionDTO = new TransactionDTO(transaction);
                    return Ok(transactionDTO);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
            }
        }
    }
}
