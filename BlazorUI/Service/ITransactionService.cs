using BlazorUI.Data;

namespace BlazorUI.Service
{
    public interface ITransactionService
    {
        Task UpdateTransactionAsync(Transaction transaction, string token);
        Task<bool> CreateTransactionAsync(Transaction transaction, string token);
        Task DeleteTransactionAsync(int id , string token);
    }
}
