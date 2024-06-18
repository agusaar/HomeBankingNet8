using HomeBankingNet8.Models;

namespace HomeBankingNet8.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        IEnumerable<Account> GetAllAccounts();
        Account FindById(long id);
        void Save(Account acc);
        IEnumerable<Account> GetAccountsByClient(long clientId);
        Account FindByAccountNumber(string accountNumber);
    }
}
