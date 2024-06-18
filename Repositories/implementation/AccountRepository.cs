using HomeBankingNet8.Models;
using HomeBankingNet8.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HomeBankingNet8.Repositories.implementation
{
    public class AccountRepository : RepositoryBase<Account>, IAccountRepository
    {
        public AccountRepository(HomeBankingContext repositoryContext) : base(repositoryContext) { }

        public Account FindByAccountNumber(string accountNumber)
        {
            return FindByCondition(acc => string.Equals(acc.Number.ToUpper(), accountNumber.ToUpper()))
                .Include(acc=> acc.Transactions)
                .FirstOrDefault();
        }

        public Account FindById(long id)
        {
            return FindByCondition(acc => acc.Id == id)
                .Include(acc => acc.Transactions)
                .FirstOrDefault();
        }

        public IEnumerable<Account> GetAccountsByClient(long clientId)
        {
            return FindByCondition(acc => acc.ClientId == clientId)
                .Include(acc => acc.Transactions)
                .ToList();
        }

        public IEnumerable<Account> GetAllAccounts()
        {
            return FindAll()
                .Include(acc => acc.Transactions)
                .ToList();
        }
        public void Save(Account acc)
        {
            if (acc.Id == 0)
                Create(acc);
            else
                Update(acc);

            SaveChanges();
            RepositoryContext.ChangeTracker.Clear();
        }
    }
}
