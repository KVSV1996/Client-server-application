using WebApi.Models;

namespace WebApi.Services.Interface
{
    public interface IAccountingService
    {
        FinancialReport GetTransactionsForDay(DateOnly date);
        FinancialReport GetTransactionsForPeriod(DateOnly startDate, DateOnly endDate);
    }
}
