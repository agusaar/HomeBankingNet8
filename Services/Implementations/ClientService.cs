using HomeBankingNet8.DTOs;
using HomeBankingNet8.Models;
using HomeBankingNet8.Repositories.Interfaces;
using HomeBankingNet8.Services.Interfaces;

namespace HomeBankingNet8.Services.Implementations
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;

        public ClientService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public Client FindByEmail(string email)
        {
            try
            {
                Client client = _clientRepository.FindByEmail(email);
                if (client == null)
                    return null;
                else
                    return client;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ClientDTO FindByID(long Id)
        {
            Client client = _clientRepository.FindById(Id);
            if(client == null)
                return null;
            else
                return new ClientDTO(client);
        }

        public ClientDTO CreateNewClient(ClientSignUpDTO signUpDto)
        {
            Client user = FindByEmail(signUpDto.Email); //Esto lo hace el servicio
            if (user != null)
                return null;
            
            Client newClient = new Client { Email = signUpDto.Email, FirstName = signUpDto.FirstName, LastName = signUpDto.LastName, Password = signUpDto.Password };
            _clientRepository.Save(newClient);

            return new ClientDTO(signUpDto);
        }


        public List<ClientDTO> getAllClients()
        {
            var clients = _clientRepository.GetAllClients();
            return clients.Select(c => new ClientDTO(c)).ToList();
        }
    }
}
   
