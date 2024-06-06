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
                var response = _clientService.FindByID(id);
                if (response.statusCode != 200)
                    return NotFound();
                else
                    return Ok(response.data);
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
                var response = _clientService.FindByEmail(email);
                if(response.statusCode != 200)
                    return StatusCode(403, "Unauthorized Capo");
                else
                    return Ok(new ClientDTO(response.data));
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
                var response = _clientService.CreateNewClient(signUpDto);

                if (response.statusCode == 200)
                {
                    var res = _clientService.FindByEmail(signUpDto.Email);
                    var accResponse = _accountService.CreateAccount(res.data.Id); //Revisar

                    if (accResponse.statusCode != 200)
                        return StatusCode(500, "Error al crear la cuenta");//Esto ocurriria unicamente si se supera el limite de cuentas. No tiene sentido

                    return Created("", response.data);
                }
                else if (response.statusCode == 403)
                    return StatusCode(403, "datos invalidos");
                else
                    return StatusCode(409, "El mail esta en uso.");
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
            var response = _clientService.FindByEmail(email);
            
            if (response.statusCode != 200) 
                return NotFound();
            else
            {
                var res = _accountService.CreateAccount(response.data.Id);
                if (res.statusCode != 200)
                    return StatusCode(403, "Forbidden. Ya tiene el numero maximo de cuentas");
                else
                    return Created("", res.data);
            }
                
        }

        [HttpPost("current/cards")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult newCard(NewCardDTO newCardDto)
        {
            string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
            var response = _clientService.FindByEmail(email);

            if (response.statusCode != 200)
                return NotFound();
            else
            {
                var res = _cardService.CreateNewCard(newCardDto, response.data);

                if (res.statusCode==200)
                    return Created("", res.data);
                else
                    return StatusCode(403, "Forbidden. Ya tiene el numero maximo de tarjetas del tipo " + newCardDto.Type);
            }
        }
    }
}
