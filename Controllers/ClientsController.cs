using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HomeBankingNet8.DTOs;
using HomeBankingNet8.Models;
using Microsoft.AspNetCore.Authorization;
using HomeBankingNet8.Utils;
using System;
using HomeBankingNet8.Services.Interfaces;
using Azure;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
        public IActionResult GetCurrent()
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                var response = _clientService.FindByEmail(email);

                if (response.statusCode == 403)
                    return StatusCode(403, "Unauthorized Capo");
                else if (response.statusCode == 404)
                    return NotFound();
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
                    return Created("", response.data);
                else if (response.statusCode == 400)
                    return StatusCode(400, "datos invalidos");
                else if (response.statusCode == 409)
                    return StatusCode(403, "Forbidden. El mail esta en uso.");
                else
                    return StatusCode(500, "Error al crear la cuenta"); //Por si recibo un 404 o 403, no tendria sentido
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
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                var res = _accountService.CreateAccount(email);

                if (res.statusCode == 403)
                    return StatusCode(403, "Se llego al limite de cuentas");
                else
                    if (res.statusCode == 401)
                    return StatusCode(401, "Unauthorized");
                else
                    return Created("", res.data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("current/accounts")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult getAccounts()
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                var res = _accountService.GetAccountsByCLient(email);

                if (res.statusCode == 200)
                    return StatusCode(200, res.data);

                return StatusCode(500, "No se encontro el cliente autenticado");
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }



        [HttpPost("current/cards")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult newCard(NewCardDTO newCardDto)
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                var response = _cardService.CreateNewCard(newCardDto, email);

                if (response.statusCode == 400)
                    return StatusCode(400, "Bad request. Debe completar todos los campos.");
                if (response.statusCode == 401)
                    return StatusCode(401, "Unauthorized");
                else if (response.statusCode == 403)
                    return StatusCode(403, "Forbidden. Ya tiene el numero maximo de tarjetas del tipo " + newCardDto.Type);
                else if (response.statusCode == 404)
                    return StatusCode(500, "No se encontro el cliente autenticado.");
                else
                    return Created("", response.data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("current/cards")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult getCurrentCards()
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                var res = _cardService.GetCardsByClient(email);

                if (res.statusCode == 200)
                    return StatusCode(200, res.data);

                return StatusCode(500, "No se encontro el cliente autenticado");
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }
    }
}
