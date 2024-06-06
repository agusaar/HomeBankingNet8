using HomeBankingNet8.DTOs;

namespace HomeBankingNet8.Services.Interfaces
{
    public interface IAccountService
    {
        public AccountClientDTO CreateAccount(long ClientId);
        public List<AccountDTO> GetAllAccounts();
        public AccountDTO GetAccountById(long AccountId);
    }
}
