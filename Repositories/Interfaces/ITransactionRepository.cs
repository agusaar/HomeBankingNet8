using HomeBankingNet8.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace HomeBankingNet8.Repositories.Interfaces
{
    public interface ITransactionRepository
    {
        IEnumerable<Transaction> GetAllTransactions();
        Transaction FindById(long id);
        void Save(Transaction transaction);
        IDbContextTransaction BeginTransaction();
    }
}
