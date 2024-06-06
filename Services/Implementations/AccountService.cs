using HomeBankingNet8.DTOs;
using HomeBankingNet8.Models;
using HomeBankingNet8.Repositories.Interfaces;
using HomeBankingNet8.Services.Interfaces;
using HomeBankingNet8.Utils;
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

        public Response<AccountClientDTO> CreateAccount(long ClientId)
        {
            var accounts = _accountRepository.GetAccountsByClient(ClientId);
            if (accounts.Count() >= 3)
                return new Response<AccountClientDTO>(null, 403);
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

                Account newAccount = new Account { ClientId = ClientId, CreationDate = DateTime.Now, Number = accNum, Balance = 0 };
                _accountRepository.Save(newAccount);
                return new Response<AccountClientDTO>(new AccountClientDTO(newAccount),200);
            }
        }

        public Response<AccountDTO> GetAccountById(long AccountId)
        {
                try
                {
                    Account acc = _accountRepository.FindById(AccountId);
                    if (acc == null)
                        return new Response<AccountDTO>(null,404);
                    else
                        return new Response<AccountDTO>(new AccountDTO(acc),200);
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
