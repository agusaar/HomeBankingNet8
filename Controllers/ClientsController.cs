using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HomeBankingNet8.DTOs;
using HomeBankingNet8.Models;
using Microsoft.AspNetCore.Authorization;
using HomeBankingNet8.Utils;
using System;
using HomeBankingNet8.Services.Interfaces;

namespace HomeBankingNet8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly IAccountService _accountService;
        private readonly ICardService _cardService;
        public ClientsController(IClientService clientService, IAccountService accountService, ICardService cardService)
        {
           _clientService = clientService;
           _accountService = accountService;
           _cardService = cardService;
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult getAll()
        {
            try
            {
                var clientsDTO = _clientService.getAllClients();
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
                ClientDTO client = _clientService.FindByID(id);
                if (client == null)
                    return NotFound();
                else
                    return Ok(client);
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
                Client client = _clientService.FindByEmail(email);
                if(client == null)
                    return StatusCode(403, "Unauthorized Capo");
                else
                    return Ok(new ClientDTO(client));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult newClient([FromBody] ClientSignUpDTO signUpDto)
        {
            try
            {
                if (string.IsNullOrEmpty(signUpDto.FirstName) || string.IsNullOrEmpty(signUpDto.LastName)
                    || string.IsNullOrEmpty(signUpDto.Email) || string.IsNullOrEmpty(signUpDto.Password))
                    return StatusCode(403, "datos invalidos");

                ClientDTO client = _clientService.CreateNewClient(signUpDto);
                if (client == null)
                    return StatusCode(403, "El mail esta en uso.");
                else
                {
                    Client newClient = _clientService.FindByEmail(signUpDto.Email);
                    AccountClientDTO accountDto = _accountService.CreateAccount(newClient.Id);
                    if (accountDto == null)
                        return StatusCode(500, "No se pudo crear la cuenta asociada al cliente.");
                    return Created("", client);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("current/accounts")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult newAccount()
        {
            string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
            Client currentUser = _clientService.FindByEmail(email);
            if (currentUser == null) 
                return NotFound();
            else
            {
                AccountClientDTO newAccount = _accountService.CreateAccount(currentUser.Id);
                if (newAccount == null)
                    return StatusCode(403, "Forbidden. Ya tiene el numero maximo de cuentas");
                else
                    return Created("", newAccount);
            }
                
        }

        [HttpPost("current/cards")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult newCard(NewCardDTO newCardDto)
        {
            string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
            Client currentUser = _clientService.FindByEmail(email);
            if (currentUser == null)
                return NotFound();
            else
            {
            CardDTO card = _cardService.CreateNewCard(newCardDto, currentUser);
            if (card != null)
                return Created("", card);
            else
                return StatusCode(403, "Forbidden. Ya tiene el numero maximo de tarjetas del tipo " + newCardDto.Type);
            }
        }
    }
}
