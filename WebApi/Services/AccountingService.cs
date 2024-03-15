using WebApi.Models;
using WebApi.Models.Enum;
using WebApi.Repository.Interface;
using WebApi.Services.Interface;

namespace WebApi.Services
{
    public class AccountingService : IAccountingService
    {
        ITransactionRepository _transactionRepository;
        public AccountingService(ITransactionRepository transactionRepository) 
        {
            _transactionRepository = transactionRepository ?? throw new ArgumentNullException(nameof(transactionRepository));
        }
        public FinancialReport GetTransactionsForDay(DateOnly date)
        {
            var transactionsForDay = _transactionRepository.GetTransactionsByDate(date).ToList();

            var report = new FinancialReport
            {
                TotalIncome = transactionsForDay
                    .Where(t => t.Type == TransactionType.Income)
                    .Sum(t => t.Value),
                TotalExpense = transactionsForDay
                    .Where(t => t.Type == TransactionType.Expense)
                    .Sum(t => t.Value),
                Transactions = transactionsForDay
            };

            return report;
        }

        public FinancialReport GetTransactionsForPeriod(DateOnly startDate, DateOnly endDate)
        {
            var transactionsForPeriod = _transactionRepository.GetTransactionsByPeriod(startDate, endDate).ToList();

            var report = new FinancialReport
            {
                TotalIncome = transactionsForPeriod
                    .Where(t => t.Type == TransactionType.Income)
                    .Sum(t => t.Value),
                TotalExpense = transactionsForPeriod
                    .Where(t => t.Type == TransactionType.Expense)
                    .Sum(t => t.Value),
                Transactions = transactionsForPeriod
            };

            return report;
        }
    }
}
