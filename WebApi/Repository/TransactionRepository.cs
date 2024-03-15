using WebApi.Data;
using WebApi.Models;
using WebApi.Repository.Interface;

namespace WebApi.Repository
{
    public class TransactionRepository : ITransactionRepository, IDisposable
    {
        private IFinanceContext context;

        public TransactionRepository(IFinanceContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            this.context = context;
        }

        public IEnumerable<Transaction> GetAllTransactions()
        {

            if (context.Transaction == null)
            {
                throw new NullReferenceException();
            }

            return context.Transaction.OrderBy(t => t.Date);
        }

        public IEnumerable<Transaction> GetTransactionsByDate(DateOnly date)
        {

            if (context.Transaction == null)
            {
                throw new NullReferenceException();
            }

            return context.Transaction.Where(t => t.Date == @date);
        }

        public IEnumerable<Transaction> GetTransactionsByPeriod(DateOnly startDate, DateOnly endDate)
        {

            if (context.Transaction == null)
            {
                throw new NullReferenceException();
            }

            return context.Transaction.Where(t => t.Date >= @startDate && t.Date <= @endDate).OrderBy(t => t.Date);
        }

        public Transaction? GetTransactionById(int id)
        {
            if (context.Transaction == null)
            {
                throw new ArgumentNullException(nameof(context.Transaction));
            }

            return context.Transaction.Find(id);
        }

        public void InsertTransaction(Transaction transact)
        {
            if (transact == null)
            {
                throw new ArgumentNullException(nameof(transact));
            }

            context.Transaction.Add(transact);
        }

        public void DeleteTransaction(int id)
        {
            Transaction? transact = context.Transaction.Find(id);

            if (transact == null)
            {
                throw new ArgumentNullException(nameof(transact));
            }
            context.Transaction.Remove(transact);
        }

        public void UpdateTransaction(Transaction transact)
        {
            if (transact == null)
            {
                throw new ArgumentNullException(nameof(transact));
            }

            context.Transaction.Update(transact);

        }

        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
