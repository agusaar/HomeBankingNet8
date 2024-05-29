using HomeBankingNet8.Utils;

namespace HomeBankingNet8.Models
{
    public class Transaction
    {
            public long Id { get; set; }
            public TransactionType Type { get; set; }

            public double Amount { get; set; }

            public DateTime CreationDate { get; set; }

            public string Description { get; set; }

            public Account Account { get; set; }

            public long AccountId { get; set; }
    }
}
