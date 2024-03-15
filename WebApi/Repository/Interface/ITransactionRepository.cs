using WebApi.Models;

namespace WebApi.Repository.Interface
{
    public interface ITransactionRepository : IDisposable
    {
        IEnumerable<Transaction> GetAllTransactions();
        IEnumerable<Transaction> GetTransactionsByDate(DateOnly date);
        IEnumerable<Transaction> GetTransactionsByPeriod(DateOnly startDate, DateOnly endDate);
        public Transaction? GetTransactionById(int id);
        public void InsertTransaction(Transaction transact);
        public void DeleteTransaction(int id);
        public void UpdateTransaction(Transaction transact);
        public void Save();
    }
}
