using HomeBankingNet8.DTOs;
using HomeBankingNet8.Models;
using HomeBankingNet8.Repositories.Interfaces;
using HomeBankingNet8.Services.Interfaces;
using HomeBankingNet8.Utils;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace HomeBankingNet8.Services.Implementations
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IAccountRepository _accountRepository;

        public ClientService(IClientRepository clientRepository, IAccountRepository accountRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
        }

        public Response<Client> FindByEmail(string email)
        {
            try
            {
                if (email == string.Empty)
                    return new Response<Client>(null, 401);

                Client client = _clientRepository.FindByEmail(email);
                if (client == null)
                    return new Response<Client>(null, 404);
                else
                    return new Response<Client>(client,200);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Response<ClientDTO> FindByID(long Id)
        {
            try
            {
                Client client = _clientRepository.FindById(Id);
                if(client == null)
                    return new Response<ClientDTO>(null, 404);
                else
                    return new Response<ClientDTO>(new ClientDTO(client),200);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Response<ClientDTO> CreateNewClient(ClientSignUpDTO signUpDto)
        {
            try
            {
                if (string.IsNullOrEmpty(signUpDto.FirstName) || string.IsNullOrEmpty(signUpDto.LastName)
                        || string.IsNullOrEmpty(signUpDto.Email) || string.IsNullOrEmpty(signUpDto.Password))
                    return new Response<ClientDTO>(null,400);

                var user = FindByEmail(signUpDto.Email); 
                if (user.data != null)
                    return new Response<ClientDTO>(null,409);
            
                Client newClient = new Client { Email = signUpDto.Email, FirstName = signUpDto.FirstName, LastName = signUpDto.LastName, Password = signUpDto.Password };
                _clientRepository.Save(newClient);

                //Inicio Create acc
                var response = FindByEmail(signUpDto.Email);
                if (response.statusCode == 404)
                    return new Response<ClientDTO>(null, 404); //No encuentro el cliente recien creado
                else
                {
                    var accounts = _accountRepository.GetAccountsByClient(response.data.Id);
                    if (accounts.Count() >= 3)
                        return new Response<ClientDTO>(null, 403); //El cliente nuevo supera el limite de cuentas. No tendria sentido.
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

                        Account newAccount = new Account { ClientId = response.data.Id, CreationDate = DateTime.Now, Number = accNum, Balance = 0 };
                        _accountRepository.Save(newAccount);
                        //Fin
                        return new Response<ClientDTO>(new ClientDTO(signUpDto), 200);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        public List<ClientDTO> getAllClients()
        {
            try
            {
                var clients = _clientRepository.GetAllClients();
                return clients.Select(c => new ClientDTO(c)).ToList();

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
   
