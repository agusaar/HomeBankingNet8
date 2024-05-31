using HomeBankingNet8.Models;

namespace HomeBankingNet8.Repositories.Interfaces
{
    public interface ITransactionRepository
    {
        IEnumerable<Transaction> GetAllTransactions();
        Transaction FindById(long id);
        void Save(Transaction transaction);
    }
}
