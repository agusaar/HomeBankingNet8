using HomeBankingNet8.DTOs;

namespace HomeBankingNet8.Services.Interfaces
{
    public interface IAccountService
    {
        public AccountDTO CreateAccount(long ClientId);
        public List<AccountDTO> GetAllAccounts();
        public AccountDTO GetAccountById(long AccountId);
    }
}
