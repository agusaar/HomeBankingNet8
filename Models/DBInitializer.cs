namespace WebApplication1.Models
{
    public class DBInitializer
    {
        public static void initialize(HomeBankingContext context)
        {
            if (!context.Clients.Any())
            {
                var clients = new Client[]
                {
                    new Client { Email = "agusaar@gmail.com", FirstName="Agustin", LastName="Rojas", Password="123456"},
                    new Client { Email = "martin@gmail.com", FirstName="Martin", LastName="Perez", Password="7890"},
                    new Client { Email = "laura@gmail.com", FirstName="Laura", LastName="Alvarez", Password="admin1234"},
                    new Client { Email = "javier@gmail.com", FirstName="Javier", LastName="Fernandez", Password="123456"}
                };

                context.Clients.AddRange(clients);
                context.SaveChanges();
            }

            if (!context.Account.Any())
            {
                var accountAgus = context.Clients.FirstOrDefault(c => c.Email == "agusaar@gmail.com");
                if (accountAgus != null)
                {
                    var accounts = new Account[]
                    {
                        new Account {ClientId = accountAgus.Id, CreationDate = DateTime.Now, Number = "VIN001", Balance = 1000 },
                        new Account {ClientId = accountAgus.Id, CreationDate = DateTime.Now, Number = "VIN002", Balance = 25000 }
                    };
                    context.Account.AddRange(accounts);
                    context.SaveChanges();

                }
            }
        }
    }
}
