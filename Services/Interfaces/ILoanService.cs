using HomeBankingNet8.DTOs;
using HomeBankingNet8.Utils;

namespace HomeBankingNet8.Services.Interfaces
{
    public interface ILoanService
    {
        public List<LoanDTO> getAllLoans();
        public Response<ClientLoanDTO> makeLoan(LoanApplicationDTO loanDTO, string email);
    }
}
