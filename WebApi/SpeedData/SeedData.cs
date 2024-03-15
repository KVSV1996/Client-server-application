using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Models;
using WebApi.Models.Enum;

namespace WebApi.SpeedData
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new FinanceContext(serviceProvider.GetRequiredService<DbContextOptions<FinanceContext>>()))
            {
                if (context.Transaction.Any())
                {
                    return;
                }

                context.Transaction.AddRange(
                     new Transaction
                     {
                         Type = TransactionType.Income,
                         Value = 20,
                         Date = DateOnly.Parse("2024-01-18")
                     },
                    new Transaction
                    {
                        Type = TransactionType.Income,
                        Value = 100,
                        Date = DateOnly.Parse("2024-01-18")
                    },
                    new Transaction
                    {
                        Type = TransactionType.Expense,
                        Value = 50,
                        Date = DateOnly.Parse("2024-01-18")
                    },
                    new Transaction
                    {
                        Type = TransactionType.Expense,
                        Value = 100,
                        Date = DateOnly.Parse("2024-01-18")
                    },
                    new Transaction
                    {
                        Type = TransactionType.Income,
                        Value = 20,
                        Date = DateOnly.Parse("2024-01-19")
                    },
                    new Transaction
                    {
                        Type = TransactionType.Expense,
                        Value = 50,
                        Date = DateOnly.Parse("2024-01-19")
                    },
                    new Transaction
                    {
                        Type = TransactionType.Expense,
                        Value = 100,
                        Date = DateOnly.Parse("2024-01-19")
                    },
                    new Transaction
                    {
                        Type = TransactionType.Income,
                        Value = 20,
                        Date = DateOnly.Parse("2024-01-20")
                    },
                    new Transaction
                    {
                        Type = TransactionType.Expense,
                        Value = 50,
                        Date = DateOnly.Parse("2024-01-20")
                    },
                    new Transaction
                    {
                        Type = TransactionType.Income,
                        Value = 100,
                        Date = DateOnly.Parse("2024-01-20")
                    },
                    new Transaction
                    {
                        Type = TransactionType.Expense,
                        Value = 20,
                        Date = DateOnly.Parse("2024-01-21")
                    },
                    new Transaction
                    {
                        Type = TransactionType.Income,
                        Value = 50,
                        Date = DateOnly.Parse("2024-01-21")
                    },
                    new Transaction
                    {
                        Type = TransactionType.Expense,
                        Value = 100,
                        Date = DateOnly.Parse("2024-01-21")
                    },
                    new Transaction
                    {
                        Type = TransactionType.Income,
                        Value = 20,
                        Date = DateOnly.Parse("2024-01-22")
                    },
                    new Transaction
                    {
                        Type = TransactionType.Income,
                        Value = 100,
                        Date = DateOnly.Parse("2024-01-22")
                    },
                    new Transaction
                    {
                        Type = TransactionType.Expense,
                        Value = 50,
                        Date = DateOnly.Parse("2024-01-22")
                    }
                    );

                context.SaveChanges();

            }
        }

    }
}
