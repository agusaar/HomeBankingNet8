﻿using HomeBankingNet8.Utils;

namespace HomeBankingNet8.Models
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
                var client = context.Clients.FirstOrDefault(c => c.Email == "agusaar@gmail.com");
                if (client != null)
                {
                    var accounts = new Account[]
                    {
                        new Account {ClientId = client.Id, CreationDate = DateTime.Now.AddDays(-10), Number = "VIN001", Balance = 1000 },
                        new Account {ClientId = client.Id, CreationDate = DateTime.Now.AddDays(-100), Number = "VIN002", Balance = 25000 }
                    };
                    context.Account.AddRange(accounts);
                    context.SaveChanges();

                }

                client = null;
                client = context.Clients.FirstOrDefault(c => c.Email == "martin@gmail.com");
                if (client != null)
                {
                    var accounts = new Account[]
                    {
                        new Account {ClientId = client.Id, CreationDate = DateTime.Now.AddDays(-15), Number = "VIN004", Balance = 500 },
                        new Account {ClientId = client.Id, CreationDate = DateTime.Now.AddDays(-1), Number = "VIN103", Balance = 2000 }
                    };
                    context.Account.AddRange(accounts);
                    context.SaveChanges();

                }
            }

            if (!context.Transactions.Any())
            {
                var account = context.Account.FirstOrDefault(c => c.Number == "VIN103");
                if (account != null)
                {
                    var transactions = new Transaction[]
                    {
                        new Transaction { AccountId= account.Id, Amount = 10000, Date= DateTime.Now, Description = "Coca-Cola Botella de Vidrio", Type = TransactionType.CREDIT },
                        new Transaction { AccountId= account.Id, Amount = -2000, Date= DateTime.Now, Description = "Coca-Cola en lata", Type = TransactionType.DEBIT },
                        new Transaction { AccountId= account.Id, Amount = -3000, Date= DateTime.Now, Description = "Coca-Cola en botella de LEVITE", Type = TransactionType.DEBIT },
                    };
                    context.Transactions.AddRange(transactions);
                    context.SaveChanges();
                }

                account = null;
                account = context.Account.FirstOrDefault(c => c.Number == "VIN001");
                if (account != null)
                {
                    var transactions = new Transaction[]
                    {
                        new Transaction { AccountId= account.Id, Amount = 10000, Date= DateTime.Now, Description = "Martillo", Type = TransactionType.CREDIT },
                        new Transaction { AccountId= account.Id, Amount = -2000, Date= DateTime.Now, Description = "Sanguche de Miga", Type = TransactionType.DEBIT },
                        new Transaction { AccountId= account.Id, Amount = -3000, Date= DateTime.Now, Description = "Pizza", Type = TransactionType.DEBIT },
                    };
                    context.Transactions.AddRange(transactions);
                    context.SaveChanges();
                }

            }

            if (!context.Loans.Any())
            {
                var loans = new Loan[]
                {
                        new Loan { Name = "Estudiantil", MaxAmount = 500000, Payments = "12,24,36,48,60" },
                        new Loan { Name = "Agricola", MaxAmount = 1000000, Payments = "6,12,24" },
                        new Loan { Name = "Automotriz", MaxAmount = 300000, Payments = "6,12,24,36" },
                };

                context.Loans.AddRange(loans);
                context.SaveChanges();
                var client = context.Clients.FirstOrDefault(c => c.Email == "agusaar@gmail.com");
                if (client != null)
                {
                    var loan = context.Loans.FirstOrDefault(l => l.Name == "Estudiantil");
                    if (loan != null)
                    {
                        var clientLoan1 = new ClientLoan
                        {
                            Amount = 400000,
                            ClientId = client.Id,
                            LoanId = loan.Id,
                            Payments = "36"
                        };
                        context.ClientLoans.Add(clientLoan1);
                    }

                    loan = null;
                    loan = context.Loans.FirstOrDefault(l => l.Name == "Agricola");
                    if (loan != null)
                    {
                        var clientLoan2 = new ClientLoan
                        {
                            Amount = 900000,
                            ClientId = client.Id,
                            LoanId = loan.Id,
                            Payments = "24"
                        };
                        context.ClientLoans.Add(clientLoan2);
                    }

                    loan = null;
                    loan = context.Loans.FirstOrDefault(l => l.Name == "Automotriz");
                    if (loan != null)
                    {
                        var clientLoan3 = new ClientLoan
                        {
                            Amount = 100000,
                            ClientId = client.Id,
                            LoanId = loan.Id,
                            Payments = "36"
                        };
                        context.ClientLoans.Add(clientLoan3);
                    }
                    context.SaveChanges();
                }

            }
        }
    }
}
