using System.Text.Json.Serialization;
using HomeBankingNet8.Models;

namespace HomeBankingNet8.DTOs
{
    public class ClientDTO
    {
        [JsonIgnore]
        public long Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public ICollection<AccountClientDTO> Accounts { get; set; }
        public ICollection<ClientLoanDTO> Loans { get; set; }

        public ClientDTO(Client c)
        {
            Id = c.Id;
            FirstName = c.FirstName;
            LastName = c.LastName;
            Email = c.Email;
            Accounts = c.Accounts.Select(ac => new AccountClientDTO(ac)).ToList();
            Loans = c.ClientLoans.Select(cl => new ClientLoanDTO(cl)).ToList();
        }
    }
}
