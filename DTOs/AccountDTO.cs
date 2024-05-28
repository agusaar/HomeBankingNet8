using WebApplication1.Models;

namespace WebApplication1.DTOs
{
    public class AccountDTO
    {
        public long Id { get; set; }

        public string Number { get; set; }

        public DateTime CreationDate { get; set; }

        public double Balance { get; set; }
        public AccountDTO(Account ac)
        {
            Id = ac.Id;
            Number = ac.Number;
            CreationDate = ac.CreationDate;
            Balance = ac.Balance;
        }
    }
}
