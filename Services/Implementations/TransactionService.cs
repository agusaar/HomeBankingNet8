using HomeBankingNet8.DTOs;
using HomeBankingNet8.Models;
using HomeBankingNet8.Repositories.Interfaces;
using HomeBankingNet8.Services.Interfaces;
using HomeBankingNet8.Utils;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace HomeBankingNet8.Services.Implementations
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IClientRepository _clientRepository;
        public TransactionService(ITransactionRepository transactionRepository, IAccountRepository accountRepository, IClientRepository clientRepository)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
            _clientRepository = clientRepository;
        }
        public Response<TransactionDTO> CreateTransaction(TransferDTO transferDTO, string currentUserEmail)
        {
            if (transferDTO.FromAccountNumber.IsNullOrEmpty() || transferDTO.ToAccountNumber.IsNullOrEmpty()
                || transferDTO.Description.IsNullOrEmpty() || transferDTO.Amount <= 0)
                return new Response<TransactionDTO>(null, 401); //Bad Request
            
            if (currentUserEmail == string.Empty)
                return new Response<TransactionDTO>(null, 402); //No hay usuario loggeado

            if (string.Equals(transferDTO.FromAccountNumber, transferDTO.ToAccountNumber))
                return new Response<TransactionDTO>(null, 403); //Igual cuenta de origen y destino

            Account fromAccount = _accountRepository.FindByAccountNumber(transferDTO.FromAccountNumber);
            if (fromAccount == null)
                return new Response<TransactionDTO>(null, 404); //From account Not Found

            Client fromClient = _clientRepository.FindById(fromAccount.ClientId);
            if (fromClient == null)
                return new Response<TransactionDTO>(null, 500); //No se encontro el dueño de la cuenta, no puede no tener dueño -> internal server error
            if (!string.Equals(fromClient.Email, currentUserEmail))
                return new Response<TransactionDTO>(null, 405); //Unauthorized, no esta logeado

            Account toAccount = _accountRepository.FindByAccountNumber(transferDTO.ToAccountNumber);
            if (toAccount == null)
                return new Response<TransactionDTO>(null, 406); //To account Not Found

            if (fromAccount.Balance < transferDTO.Amount)
                return new Response<TransactionDTO>(null, 407); //No hay saldo suficiente. Revisar

            Transaction fromTransaction = new Transaction
            {
                AccountId = fromAccount.Id,
                Amount = transferDTO.Amount * (-1),
                Date = DateTime.Now,
                Description = transferDTO.Description+" || Enviado a: "+toAccount.Number,
                Type = TransactionType.DEBIT
            };

            _transactionRepository.Save(fromTransaction);

            Transaction toTransaction = new Transaction
            {
                AccountId = toAccount.Id,
                Amount = transferDTO.Amount,
                Date = DateTime.Now,
                Description = transferDTO.Description + " || Recibido de: "+fromAccount.Number,
                Type = TransactionType.CREDIT
            };

            _transactionRepository.Save(toTransaction);

            fromAccount.Balance -= transferDTO.Amount;
            toAccount.Balance += transferDTO.Amount;

            _accountRepository.Save(fromAccount);
            _accountRepository.Save(toAccount);

            return new Response<TransactionDTO>(new TransactionDTO(fromTransaction), 200);
        }


        public List<TransactionDTO> GetAllTransactions()
        {
            try
            {
                var transactions = _transactionRepository.GetAllTransactions();
                var transactionsDTO = transactions.Select(tr => new TransactionDTO(tr)).ToList();
                return transactionsDTO;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Response<TransactionDTO> GetTransactionById(long TransactionId)
        {
            try
            {
                var transaction = _transactionRepository.FindById(TransactionId);
                if (transaction == null)
                    return new Response<TransactionDTO>(null, 404);
                return new Response<TransactionDTO>(new TransactionDTO(transaction),200);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
