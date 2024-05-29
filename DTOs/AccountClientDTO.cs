using HomeBankingNet8.Models;

namespace HomeBankingNet8.DTOs
{
    public class AccountClientDTO
    {
        public long Id { get; set; }

        public string Number { get; set; }

        public DateTime CreationDate { get; set; }

        public double Balance { get; set; }

        public AccountClientDTO(Account ac)
        {
            Id = ac.Id;
            Number = ac.Number;
            CreationDate = ac.CreationDate;
            Balance = ac.Balance;
        }
    }
}
