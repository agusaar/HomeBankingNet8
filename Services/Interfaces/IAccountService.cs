using HomeBankingNet8.DTOs;
using HomeBankingNet8.Utils;

namespace HomeBankingNet8.Services.Interfaces
{
    public interface IAccountService
    {
        public Response<AccountClientDTO> CreateAccount(string email);
        public List<AccountDTO> GetAllAccounts();
        public Response<AccountDTO> GetAccountById(long AccountId);
    }
}
