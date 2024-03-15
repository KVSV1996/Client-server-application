using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using WebApi.Data;
using WebApi.Models;
using WebApi.Models.Enum;
using WebApi.Repository;

namespace WebApi.Tests.RepositoryTests
{
    [TestClass]
    public class TransactionRepositoryTests : IDisposable
    {
        private readonly DbConnection _connection;
        private readonly DbContextOptions<FinanceContext> _contextOptions;

        public TransactionRepositoryTests()
        {
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            _contextOptions = new DbContextOptionsBuilder<FinanceContext>()
                .UseSqlite(_connection)
                .Options;

            using var context = new FinanceContext(_contextOptions);
            if (context.Database.EnsureCreated())
            {
                using var viewCommand = context.Database.GetDbConnection().CreateCommand();
                viewCommand.CommandText = @"
CREATE VIEW AllResources AS
SELECT Id
FROM Transaction;";

                viewCommand.ExecuteNonQuery();
            }

            context.AddRange(
                new Transaction { Id = 1, Type = TransactionType.Income, Value = 20, Date = DateOnly.Parse("2024-01-18") },
                new Transaction { Id = 2, Type = TransactionType.Expense, Value = 100, Date = DateOnly.Parse("2024-01-18") },
                new Transaction { Id = 3, Type = TransactionType.Income, Value = 20, Date = DateOnly.Parse("2024-01-19") },
                new Transaction { Id = 4, Type = TransactionType.Expense, Value = 50, Date = DateOnly.Parse("2024-01-19") }
            );
            context.SaveChanges();
        }

        FinanceContext CreateContext() => new FinanceContext(_contextOptions);

        public void Dispose() => _connection.Dispose();

        [TestMethod]
        public void GetAllTransactions_ReturnsAllTransactionsOrderedByDate()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new TransactionRepository(context);

            // Act
            var transactions = repository.GetAllTransactions();

            // Assert
            Assert.IsNotNull(transactions);
            Assert.AreEqual(4, transactions.Count());
            Assert.IsTrue(transactions.Select(t => t.Date).SequenceEqual(transactions.OrderBy(t => t.Date).Select(t => t.Date)));
        }

        [TestMethod]
        public void GetTransactionsByDate_ReturnsTransactionsForDate()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new TransactionRepository(context);
            var testDate = DateOnly.Parse("2024-01-18");

            // Act
            var transactions = repository.GetTransactionsByDate(testDate);

            // Assert
            Assert.IsNotNull(transactions);
            Assert.AreEqual(2, transactions.Count());
            Assert.IsTrue(transactions.All(t => t.Date == testDate));
        }

        [TestMethod]
        public void GetTransactionsByPeriod_ReturnsTransactionsForPeriod()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new TransactionRepository(context);
            var startDate = DateOnly.Parse("2024-01-18");
            var endDate = DateOnly.Parse("2024-01-19");

            // Act
            var transactions = repository.GetTransactionsByPeriod(startDate, endDate);

            // Assert
            Assert.IsNotNull(transactions);
            Assert.AreEqual(4, transactions.Count());
            Assert.IsTrue(transactions.First().Date >= startDate && transactions.Last().Date <= endDate);
            Assert.IsTrue(transactions.SequenceEqual(transactions.OrderBy(t => t.Date)));
        }

        [TestMethod]
        public void GetTransactionById_ExistingId_ReturnsTransaction()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new TransactionRepository(context);
            int testId = 1;

            // Act
            var transaction = repository.GetTransactionById(testId);

            // Assert
            Assert.IsNotNull(transaction);
            Assert.AreEqual(testId, transaction.Id);
            Assert.AreEqual(TransactionType.Income, transaction.Type);
            Assert.AreEqual(20, transaction.Value);
            Assert.AreEqual(DateOnly.Parse("2024-01-18"), transaction.Date);
        }

        [TestMethod]
        public void InsertTransaction_ValidTransaction_AddsTransaction()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new TransactionRepository(context);
            var newTransaction = new Transaction
            {
                Type = TransactionType.Income,
                Value = 150,
                Date = DateOnly.Parse("2024-01-20")
            };

            // Act
            repository.InsertTransaction(newTransaction);
            context.SaveChanges();

            // Assert
            var insertedTransaction = context.Transaction.Find(5);
            Assert.IsNotNull(insertedTransaction);
            Assert.AreEqual(newTransaction.Id, insertedTransaction.Id);
            Assert.AreEqual(newTransaction.Type, insertedTransaction.Type);
            Assert.AreEqual(newTransaction.Value, insertedTransaction.Value);
            Assert.AreEqual(newTransaction.Date, insertedTransaction.Date);
        }

        [TestMethod]
        public void DeleteTransaction_ExistingId_DeletesTransaction()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new TransactionRepository(context);
            int testId = 1;

            // Act
            repository.DeleteTransaction(testId);
            context.SaveChanges();

            // Assert
            var deletedTransaction = context.Transaction.Find(testId);
            Assert.IsNull(deletedTransaction);
        }

        [TestMethod]
        public void UpdateTransaction_ValidTransaction_UpdatesTransaction()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new TransactionRepository(context);
            var testTransaction = new Transaction{ Id = 1, Type = TransactionType.Expense, Value = 150, Date = DateOnly.Parse("2024-01-18") };

            // Act
            repository.UpdateTransaction(testTransaction);
            context.SaveChanges();

            // Assert
            var updatedTransaction = context.Transaction.Find(1);
            Assert.IsNotNull(updatedTransaction);
            Assert.AreEqual(testTransaction.Type, updatedTransaction.Type);
            Assert.AreEqual(testTransaction.Value, updatedTransaction.Value);
        }

    }
}
