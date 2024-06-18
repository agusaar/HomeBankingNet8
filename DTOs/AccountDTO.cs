using HomeBankingNet8.Models;

namespace HomeBankingNet8.DTOs
{
    public class AccountDTO
    {
        public long Id { get; set; }

        public string Number { get; set; }

        public DateTime CreationDate { get; set; }

        public double Balance { get; set; }

        public ICollection<TransactionDTO> Transactions { get; set; }
        public AccountDTO(Account ac)
        {
            Id = ac.Id;
            Number = ac.Number;
            CreationDate = ac.CreationDate;
            Balance = ac.Balance;
            Transactions = ac.Transactions.Select(tr => new TransactionDTO(tr)).ToList();
        }
    }
}
