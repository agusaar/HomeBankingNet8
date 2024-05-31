using HomeBankingNet8.Models;
using HomeBankingNet8.Utils;

namespace HomeBankingNet8.DTOs
{
    public class TransactionDTO
    {
        public long Id { get; set; }
        public string Type { get; set; }

        public double Amount { get; set; }

        public DateTime Date { get; set; }

        public string Description { get; set; }

        public TransactionDTO(Transaction tr)
        {
            Id = tr.Id;
            Type = tr.Type.ToString();
            Amount = tr.Amount;
            Date = tr.Date;
            Description = tr.Description;
        }
    }
}
