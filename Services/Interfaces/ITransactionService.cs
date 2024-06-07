using HomeBankingNet8.DTOs;
using HomeBankingNet8.Utils;

namespace HomeBankingNet8.Services.Interfaces
{
    public interface ITransactionService
    {
        public Response<TransactionDTO> CreateTransaction(TransferDTO transferDTO,string currentUserEmail);
        public List<TransactionDTO> GetAllTransactions();
        public Response<TransactionDTO> GetTransactionById(long AccountId);
    }
}
