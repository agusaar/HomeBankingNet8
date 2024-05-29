using HomeBankingNet8.Models;

namespace HomeBankingNet8.Repositories
{
    public interface IAccountRepository
    {
        IEnumerable<Account> GetAllAccounts();
        Account FindById(long id);
    }
}
