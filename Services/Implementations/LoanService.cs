using HomeBankingNet8.DTOs;
using HomeBankingNet8.Models;
using HomeBankingNet8.Repositories.Interfaces;
using HomeBankingNet8.Services.Interfaces;
using HomeBankingNet8.Utils;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.IdentityModel.Tokens;

namespace HomeBankingNet8.Services.Implementations
{
    public class LoanService : ILoanService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IClientRepository _clientRepository;
        private readonly ILoanRepository _loanRepository;
        private readonly IClientLoanRepository _clientLoanRepository;
        public LoanService(ITransactionRepository transactionRepository, IAccountRepository accountRepository, IClientRepository clientRepository, ILoanRepository loanRepository, IClientLoanRepository clientLoanRepository)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
            _clientRepository = clientRepository;
            _loanRepository = loanRepository;
            _clientLoanRepository = clientLoanRepository;
        }
        public List<LoanDTO> getAllLoans()
        {
            try
            {
                var loans = _loanRepository.GetAllLoans();
                return loans.Select(loan => new LoanDTO(loan)).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Response<ClientLoanDTO> makeLoan(LoanApplicationDTO loanAppDTO, string email)
        {
            try
            {
                if (loanAppDTO.Payments.IsNullOrEmpty() || loanAppDTO.ToAccountNumber.IsNullOrEmpty()
                    || loanAppDTO.LoanId<=0 || loanAppDTO.Amount <= 0)
                    return new Response<ClientLoanDTO>(null, 401); //Bad Request

                if(email.IsNullOrEmpty())
                    return new Response<ClientLoanDTO>(null, 402); //Forbidden. No hay usuario autentificado

                Loan loan = _loanRepository.FindById(loanAppDTO.LoanId);
                if (loan == null)
                    return new Response<ClientLoanDTO>(null, 403); //Forbidden.No existe el loan.
                if(loan.MaxAmount < loanAppDTO.Amount)
                    return new Response<ClientLoanDTO>(null, 404); //Forbidden.Monto mayor al permitido
            
                var paymentsList = loan.Payments.Split(",").ToList();
                if(!paymentsList.Contains(loanAppDTO.Payments))
                    return new Response<ClientLoanDTO>(null, 407); //Forbidden.No existe el metodo de pago 

                Account account = _accountRepository.FindByAccountNumber(loanAppDTO.ToAccountNumber);
                if (account == null)
                    return new Response<ClientLoanDTO>(null, 405); //Forbidden.No existe la cuenta
            
                Client client = _clientRepository.FindByEmail(email);
                if(client.Id != account.ClientId)
                    return new Response<ClientLoanDTO>(null, 406); //Forbidden. La cuentra no le pertenece al usuario autentificado

                ClientLoan clientLoan = RealizarMovimientos(loanAppDTO, email, account, client, loan);

                return new Response<ClientLoanDTO>(new ClientLoanDTO(clientLoan, loan.Name), 200);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private ClientLoan RealizarMovimientos(LoanApplicationDTO loanAppDTO, string email, Account account, Client client, Loan loan)
        {
            using (var DBtransaction = _loanRepository.BeginTransaction())
            try
            {
                //Se crea y guarda CLientLoan
                ClientLoan clientLoan = new ClientLoan
                {
                    Amount = loanAppDTO.Amount * 1.2,
                    ClientId = client.Id,
                    LoanId = loan.Id,
                    Payments = loanAppDTO.Payments
                }; 
                _clientLoanRepository.Save(clientLoan);

                //Se crea y guarda la transaccion del prestamo
                Transaction transaction = new Transaction
                {
                    AccountId = account.Id,
                    Amount = loanAppDTO.Amount,
                    Date = DateTime.Now,
                    Description = "Loan Approved",
                    Type = TransactionType.CREDIT
                };
                _transactionRepository.Save(transaction);

                //Se actualiza el balance de la cuenta
                account.Balance += loanAppDTO.Amount;
                _accountRepository.Save(account);

                DBtransaction.Commit();

                    return clientLoan;
            }
            catch (Exception)
            {
                DBtransaction.Rollback();
                throw;
            }

        }
    }
}
