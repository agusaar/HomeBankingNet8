using HomeBankingNet8.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeBankingNet8.Repositories.implementation
{
    public class AccountRepository : RepositoryBase<Account>, IAccountRepository
    {
        public AccountRepository(HomeBankingContext repositoryContext) : base(repositoryContext) { }

        public Account FindById(long id)
        {
            return FindByCondition(acc => acc.Id == id)
                .Include(acc => acc.Transactions)
                .FirstOrDefault();
        }

        public IEnumerable<Account> GetAllAccounts()
        {
            return FindAll()
                .Include(acc => acc.Transactions)
                .ToList();
        }
    }
}
