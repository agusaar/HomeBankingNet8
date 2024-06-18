using HomeBankingNet8.Models;
using HomeBankingNet8.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace HomeBankingNet8.Repositories.implementation
{
    public class LoanRepository : RepositoryBase<Loan>, ILoanRepository
    {
        public LoanRepository(HomeBankingContext repositoryContext) : base(repositoryContext) {}

        public Loan FindById(long id)
        {
            return FindByCondition(loan =>  loan.Id == id).FirstOrDefault();
        }

        public IEnumerable<Loan> GetAllLoans()
        {
            return FindAll().ToList();
        }

        public void Save(Loan loan)
        {
            Create(loan);
            SaveChanges();
        }

        public IDbContextTransaction BeginTransaction()
        {
            return RepositoryContext.Database.BeginTransaction();
        }
    }
}
