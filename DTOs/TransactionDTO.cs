using HomeBankingNet8.Models;
using HomeBankingNet8.Utils;

namespace HomeBankingNet8.DTOs
{
    public class TransactionDTO
    {
        public long Id { get; set; }
        public TransactionType Type { get; set; }

        public double Amount { get; set; }

        public DateTime date { get; set; }

        public string Description { get; set; }

        public TransactionDTO(Transaction tr)
        {
            Id = tr.Id;
            Type = tr.Type;
            Amount = tr.Amount;
            date = tr.CreationDate;
            Description = tr.Description;
        }
    }
}
