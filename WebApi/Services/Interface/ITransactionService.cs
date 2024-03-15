using WebApi.Models;

namespace WebApi.Services.Interface
{
    public interface ITransactionService
    {
        IEnumerable<Transaction> GetAllTransactions();
        Transaction? GetTransactionById(int id);
        void TransactionCreate(TransactionWithoutId transact);
        void TransactionUpdate(Transaction transact);
        void TransactionDelete(int id);

    }
}
