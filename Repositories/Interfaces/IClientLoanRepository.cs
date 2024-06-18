using HomeBankingNet8.Models;

namespace HomeBankingNet8.Repositories.Interfaces
{
    public interface IClientLoanRepository
    {
        void Save(ClientLoan loan);
    }
}
