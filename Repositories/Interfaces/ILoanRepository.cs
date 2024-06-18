using HomeBankingNet8.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace HomeBankingNet8.Repositories.Interfaces
{
    public interface ILoanRepository
    {
        IEnumerable<Loan> GetAllLoans();
        Loan FindById(long id);
        void Save(Loan loan);
        IDbContextTransaction BeginTransaction();
    }
}
