using HomeBankingNet8.Models;

namespace HomeBankingNet8.Repositories.Interfaces
{
    public interface ILoanRepository
    {
        IEnumerable<Loan> GetAllLoans();
        Loan FindById(long id);
        void Save(Loan loan);
    }
}
