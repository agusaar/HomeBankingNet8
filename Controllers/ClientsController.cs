using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HomeBankingNet8.DTOs;
using HomeBankingNet8.Repositories.Interfaces;
using HomeBankingNet8.Models;
using Microsoft.AspNetCore.Authorization;
using HomeBankingNet8.Utils;
using System;

namespace HomeBankingNet8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientRepository _clientRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ICardRepository _cardRepository;
        public ClientsController(IClientRepository clientRepository, IAccountRepository accountRepository, ICardRepository cardRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _cardRepository = cardRepository;
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
        public IActionResult newClient([FromBody] ClientSignUpDTO signUpDto)
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

                //Creo el cliente
                Client newClient = new Client { Email = signUpDto.Email, FirstName = signUpDto.FirstName, LastName = signUpDto.LastName, Password = signUpDto.Password };

                _clientRepository.Save(newClient);

                //Creo la cuenta asociada
                string accNum = "VIN-";

                Random random = new Random();
                int numeroRand;
                for (var i = 0; i < 8; i++)
                {
                    numeroRand = random.Next(0, 10);
                    accNum += numeroRand;
                }
                Account newAccount = new Account { ClientId = newClient.Id, CreationDate = DateTime.Now, Number = accNum, Balance = 0 };
                _accountRepository.Save(newAccount);

                return Created("", new ClientDTO(signUpDto));
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
            Client currentUser = _clientRepository.FindByEmail(email);
            if (currentUser == null) 
                return NotFound();
            else
            {
                var accounts = _accountRepository.GetAccountsByClient(currentUser.Id);
                if (accounts.Count() >= 3)
                    return StatusCode(403, "Forbidden. Ya tiene el numero maximo de cuentas");
                else
                {
                    Boolean existeAccNum = true;
                    string accNum = "VIN-";
                    Random random = new Random();
                    int numeroRand;
                    while (existeAccNum)
                    {
                        for (var i = 0; i < 8; i++)
                        {
                            numeroRand = random.Next(0, 10);
                            accNum += numeroRand;
                        }
                        if (_accountRepository.FindByAccountNumber(accNum) != null)
                            accNum = "VIN-";
                        else
                            existeAccNum = false;
                    }

                    Account newAccount = new Account { ClientId = currentUser.Id, CreationDate = DateTime.Now, Number = accNum, Balance = 0, Transactions = [] };
                    _accountRepository.Save(newAccount);

                    return Created("", new AccountDTO(newAccount));
                }
            }
                
        }

        [HttpPost("current/cards")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult newCard(NewCardDTO newCardDto)
        {
            string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
            Client currentUser = _clientRepository.FindByEmail(email);
            if (currentUser == null)
                return NotFound();
            else
            {
                CardType newCardType = (CardType)Enum.Parse(typeof(CardType), newCardDto.Type);
                var userCards = _cardRepository.FindByClientId(currentUser.Id);
                int cantCards = userCards.Count(card => card.Type == newCardType);

                if (cantCards < 3)
                {
                    CardColor newCardColor = (CardColor)Enum.Parse(typeof(CardColor), newCardDto.Color);
                    Random random = new Random();
                    int numeroRand;
                    string cardNum = "";
                    Boolean existeCardNum = true;
                    while (existeCardNum)
                    {
                        for (var i = 0; i < 4; i++)
                        {
                            for (var j = 0; j < 4; j++)
                            {
                                numeroRand = random.Next(0, 10);
                                cardNum += numeroRand;
                            }
                            if (i < 3)
                                cardNum += "-";
                        }
                        if (_cardRepository.FindByCardNum(cardNum) != null)
                            cardNum = "";
                        else
                            existeCardNum = false;
                    }

                    Card card = new Card
                    {
                        ClientId = currentUser.Id,
                        CardHolder = currentUser.FirstName + " " + currentUser.LastName,
                        Type = newCardType,
                        Color = newCardColor,
                        Number = cardNum,
                        Cvv = random.Next(100, 1000),
                        FromDate = DateTime.Now,
                        ThruDate = DateTime.Now.AddYears(5)
                    };
                    _cardRepository.Save(card);
                    return Created("", new CardDTO(card));

                }
                else
                {
                    return StatusCode(403, "Forbidden. Ya tiene el numero maximo de tarjetas del tipo " + newCardType.ToString().ToLower());
                }
            }
        }
    }
}
