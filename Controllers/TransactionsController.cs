using HomeBankingNet8.DTOs;
using HomeBankingNet8.Repositories.Interfaces;
using HomeBankingNet8.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeBankingNet8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        
        private readonly ITransactionService _transactionService;
        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet]
        public IActionResult getAll()
        {
            try
            {
                var transactions = _transactionService.GetAllTransactions();
                return Ok(transactions);
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
                var response = _transactionService.GetTransactionById(id);
                if (response.statusCode == 200)
                    return Ok(response.data);
                else
                    return StatusCode(404, "Transaccion no encontrada");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult PostTransaction(TransferDTO transferDTO)
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                var response = _transactionService.CreateTransaction(transferDTO,email);

                //400,401,403,404,500
                if (response.statusCode == 400)
                    return StatusCode(400, "Error en la peticion. Faltan campos o FromAccount es igual a ToAccount");
                if (response.statusCode == 401)
                    return StatusCode(401, "Unauthorized. No hay email logeado");
                if (response.statusCode == 403)
                    return StatusCode(403, "Forbiden. La cuenta no corresponde al usuario loggeado"); 
                if (response.statusCode == 404)
                    return StatusCode(404, "FromAccount o ToAccount no encontrada.");
                if (response.statusCode == 500)
                    return StatusCode(500, "Error. No se encontro al dueño de la cuenta loggeada.");
                    
                return StatusCode(200, response.data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        
    }
}
