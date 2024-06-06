using HomeBankingNet8.DTOs;
using HomeBankingNet8.Models;
using HomeBankingNet8.Repositories.Interfaces;
using HomeBankingNet8.Services.Interfaces;
using HomeBankingNet8.Utils;

namespace HomeBankingNet8.Services.Implementations
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;

        public ClientService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public Response<Client> FindByEmail(string email)
        {
            try
            {
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
            Client client = _clientRepository.FindById(Id);
            if(client == null)
                return new Response<ClientDTO>(null, 404);
            else
                return new Response<ClientDTO>(new ClientDTO(client),200);
        }

        public Response<ClientDTO> CreateNewClient(ClientSignUpDTO signUpDto)
        {
            if (string.IsNullOrEmpty(signUpDto.FirstName) || string.IsNullOrEmpty(signUpDto.LastName)
                    || string.IsNullOrEmpty(signUpDto.Email) || string.IsNullOrEmpty(signUpDto.Password))
                return new Response<ClientDTO>(null,403);

            var user = FindByEmail(signUpDto.Email); 
            if (user.data != null)
                return new Response<ClientDTO>(null,409);
            
            Client newClient = new Client { Email = signUpDto.Email, FirstName = signUpDto.FirstName, LastName = signUpDto.LastName, Password = signUpDto.Password };
            _clientRepository.Save(newClient);

            return new Response<ClientDTO>(new ClientDTO(signUpDto),200);
        }


        public List<ClientDTO> getAllClients()
        {
            var clients = _clientRepository.GetAllClients();
            return clients.Select(c => new ClientDTO(c)).ToList();
        }
    }
}
   
