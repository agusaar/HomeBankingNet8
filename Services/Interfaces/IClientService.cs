using HomeBankingNet8.DTOs;
using HomeBankingNet8.Models;

namespace HomeBankingNet8.Services.Interfaces
{
    public interface IClientService
    {
        public List<ClientDTO> getAllClients();
        public ClientDTO FindByID(long Id);
        public Client FindByEmail(string email);
        public ClientDTO CreateNewClient(ClientSignUpDTO ClientSignUp);

    }
}
