using HomeBankingNet8.DTOs;
using HomeBankingNet8.Utils;

namespace HomeBankingNet8.Services.Interfaces
{
    public interface IAccountService
    {
        public Response<AccountClientDTO> CreateAccount(long ClientId);
        public List<AccountDTO> GetAllAccounts();
        public Response<AccountDTO> GetAccountById(long AccountId);
    }
}
