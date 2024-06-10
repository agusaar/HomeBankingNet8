using HomeBankingNet8.DTOs;
using HomeBankingNet8.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeBankingNet8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private readonly ILoanService _loanService;
        public LoansController(ILoanService loanService)
        {
            _loanService = loanService;
        }

        [HttpGet]
        public IActionResult GetLoans()
        {
            try
            {
                var loanDTOs = _loanService.getAllLoans();
                return Ok(loanDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Policy="ClientOnly")]
        public IActionResult PostLoans(LoanApplicationDTO loanAplication)
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                var response = _loanService.makeLoan(loanAplication,email);

                if (response.statusCode == 401)
                    return StatusCode(403, "Forbidden. Uno o mas datos invalidos.");
                if (response.statusCode == 402)
                    return StatusCode(401, "Unauthorized. No hay cuenta autenticada.");
                if (response.statusCode == 403)
                    return StatusCode(403, "Forbidden. No existe el prestamo.");
                if (response.statusCode == 404)
                    return StatusCode(403, "Forbidden. El monto solicitado supera el monto máximo permitido del préstamo solicitado.");
                if (response.statusCode == 405)
                    return StatusCode(403, "Forbidden. La cuenta destino no existe.");
                if (response.statusCode == 406)
                    return StatusCode(403, "Forbidden. La cuenta origen no pertenece al cliente autenticado.");
                if (response.statusCode == 407)
                    return StatusCode(403, "Forbidden. La cantidad de cuotas no está disponible para el préstamo solicitado .");

                return Ok(response.data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
