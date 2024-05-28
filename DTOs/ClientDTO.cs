using System.Text.Json.Serialization;
using WebApplication1.Models;

namespace WebApplication1.DTOs
{
    public class ClientDTO
    {
        [JsonIgnore]
        public long Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public ICollection<AccountDTO> Accounts { get; set; }

        public ClientDTO(Client c)
        {
            Id = c.Id;
            FirstName = c.FirstName;
            LastName = c.LastName;
            Email = c.Email;
            Accounts = c.Accounts.Select(ac => new AccountDTO(ac)).ToList();
        }
    }
}
