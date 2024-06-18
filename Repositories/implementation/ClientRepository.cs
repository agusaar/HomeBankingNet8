using Microsoft.EntityFrameworkCore;
using HomeBankingNet8.Models;
using HomeBankingNet8.Repositories.Interfaces;

namespace HomeBankingNet8.Repositories.implementation
{
    public class ClientRepository: RepositoryBase<Client>, IClientRepository
    {
        public ClientRepository(HomeBankingContext repositoryContext) : base(repositoryContext) { }

        public Client FindByEmail(string email)
        {
            return FindByCondition(client => client.Email.ToUpper() == email.ToUpper())
                .Include(client => client.Accounts)
                .Include(client => client.Cards)
                .Include(client => client.ClientLoans)
                .ThenInclude(clientLoans => clientLoans.Loan)
                .FirstOrDefault();
        }

        public Client FindById(long id)
        {
            return FindByCondition(client => client.Id == id)
                .Include(client => client.Accounts)
                .Include(client=> client.Cards)
                .Include(client => client.ClientLoans)
                .ThenInclude(clientLoan => clientLoan.Loan)
                .FirstOrDefault();
        }

        public IEnumerable<Client> GetAllClients()
        {
            return FindAll()
               .Include(client => client.Accounts)
               .Include(client => client.Cards)
               .Include(client => client.ClientLoans)
                .ThenInclude(clientLoan => clientLoan.Loan)
               .ToList();
        }

        public void Save(Client client)
        {
            Create(client);
            SaveChanges();
        }
    }
}
