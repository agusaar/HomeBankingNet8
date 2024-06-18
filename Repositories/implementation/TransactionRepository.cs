using HomeBankingNet8.Models;
using HomeBankingNet8.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace HomeBankingNet8.Repositories.implementation
{
    public class TransactionRepository:RepositoryBase<Transaction>, ITransactionRepository
    {
        public TransactionRepository(HomeBankingContext repositoryContext) : base(repositoryContext) { }

        public IDbContextTransaction BeginTransaction()
        {
            return RepositoryContext.Database.BeginTransaction();
        }

        public Transaction FindById(long id)
        {
            return FindByCondition(tr => tr.Id == id)
                .FirstOrDefault();
        }

        public IEnumerable<Transaction> GetAllTransactions()
        {
            return FindAll()
                .ToList();
        }
        public void Save(Transaction transaction)
        {
            Create(transaction);
            SaveChanges();
        }
    }
}
