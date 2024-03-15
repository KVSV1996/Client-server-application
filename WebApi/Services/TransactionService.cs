using WebApi.Models;
using WebApi.Services.Interface;
using WebApi.Repository.Interface;


namespace WebApi.Services
{
    public class TransactionService : ITransactionService
    {
        ITransactionRepository _transactionRepository;
        public TransactionService(ITransactionRepository transactionRepository) 
        {
            _transactionRepository = transactionRepository ?? throw new ArgumentNullException(nameof(transactionRepository));
        }

        public IEnumerable<Transaction> GetAllTransactions() => _transactionRepository.GetAllTransactions();
        public Transaction? GetTransactionById(int id) => _transactionRepository.GetTransactionById(id);

        public void TransactionCreate(TransactionWithoutId transact)
        {
            _transactionRepository.InsertTransaction((Transaction)transact);
            _transactionRepository.Save();
        }

        public void TransactionUpdate(Transaction transact)
        {
            _transactionRepository.UpdateTransaction(transact);
            _transactionRepository.Save();
        }

        public void TransactionDelete(int id) 
        {
            int transactionCount = _transactionRepository.GetAllTransactions()
                .Where(s => s.Id == id)
                .Count();

            if (transactionCount == 0)
            {
                throw new ArgumentNullException(nameof(transactionCount));
            }

            _transactionRepository.DeleteTransaction(id);
            _transactionRepository.Save();
        }
    }
}
