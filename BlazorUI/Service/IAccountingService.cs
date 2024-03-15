using BlazorUI.Data;

namespace BlazorUI.Service
{
    public interface IAccountingService
    {
        Task<FinancialReport?> GetTransactionsForPeriodAsync(DateOnly startDate, DateOnly endDate, string token);
    }
}
