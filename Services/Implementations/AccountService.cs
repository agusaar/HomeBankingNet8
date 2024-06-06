using HomeBankingNet8.DTOs;
using HomeBankingNet8.Models;
using HomeBankingNet8.Repositories.Interfaces;
using HomeBankingNet8.Services.Interfaces;
using Microsoft.Identity.Client;

namespace HomeBankingNet8.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public AccountDTO CreateAccount(long ClientId)
        {
            var accounts = _accountRepository.GetAccountsByClient(ClientId);
            if (accounts.Count() >= 3)
                return null;
            else
            {
                Boolean existeAccNum = true;
                string accNum = "VIN-";
                Random random = new Random();
                int numeroRand;
                while (existeAccNum)
                {
                    for (var i = 0; i < 8; i++)
                    {
                        numeroRand = random.Next(0, 10);
                        accNum += numeroRand;
                    }
                    if (_accountRepository.FindByAccountNumber(accNum) != null)
                        accNum = "VIN-";
                    else
                        existeAccNum = false;
                }

                Account newAccount = new Account { ClientId = ClientId, CreationDate = DateTime.Now, Number = accNum, Balance = 0, Transactions = [] };
                _accountRepository.Save(newAccount);
                return new AccountDTO(newAccount);
            }
        }

        public AccountDTO GetAccountById(long AccountId)
        {
                try
                {
                    Account acc = _accountRepository.FindById(AccountId);
                    if (acc == null)
                        return null;
                    else
                        return new AccountDTO(acc);
                }
                catch (Exception)
                {

                    throw;
                }
        }

        public List<AccountDTO> GetAllAccounts()
        {
            try
            {
                var accounts = _accountRepository.GetAllAccounts();
                var accountsDTO = accounts.Select(acc => new AccountDTO(acc)).ToList();
                return accountsDTO;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
