using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HomeBankingNet8.DTOs;
using HomeBankingNet8.Repositories.Interfaces;
using HomeBankingNet8.Models;
using Microsoft.AspNetCore.Authorization;

namespace HomeBankingNet8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientRepository _clientRepository;
        public ClientsController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult getAll()
        {
            try
            {
                var clients = _clientRepository.GetAllClients();
                var clientsDTO = clients.Select(c => new ClientDTO(c)).ToList();
                return Ok(clientsDTO);
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
                var client = _clientRepository.FindById(id);
                if (client == null)
                {
                    return NotFound();
                }
                var clientDTO = new ClientDTO(client);
                return Ok(clientDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("current")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult GetCurrent() {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                {
                    return StatusCode(403, "Unauthorized Capo");
                }
                Client client = _clientRepository.FindByEmail(email);
                if (client == null)
                {
                    return StatusCode(403, "Unauthorized Capo");
                }
                var clientDto = new ClientDTO(client);
                return Ok(clientDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] ClientSignUpDTO signUpDto)
        {
            try
            {
                if (string.IsNullOrEmpty(signUpDto.FirstName) || string.IsNullOrEmpty(signUpDto.LastName)
                    || string.IsNullOrEmpty(signUpDto.Email) || string.IsNullOrEmpty(signUpDto.Password))
                    return StatusCode(403, "datos invalidos");

                Client user = _clientRepository.FindByEmail(signUpDto.Email);
                if (user != null)
                {
                    return StatusCode(403, "El mail esta en uso");
                }

                Client newClient = new Client { Email = signUpDto.Email, FirstName = signUpDto.FirstName, LastName = signUpDto.LastName, Password = signUpDto.Password };

                _clientRepository.Save(newClient);
                return Created("", new ClientDTO(signUpDto));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
