using HomeBankingNet8.DTOs;
using HomeBankingNet8.Models;
using HomeBankingNet8.Utils;

namespace HomeBankingNet8.Services.Interfaces
{
    public interface IClientService
    {
        public List<ClientDTO> getAllClients();
        public Response<ClientDTO> FindByID(long Id);
        public Response<Client> FindByEmail(string email);
        public Response<ClientDTO> CreateNewClient(ClientSignUpDTO ClientSignUp);

    }
}
