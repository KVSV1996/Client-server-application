using Moq;
using WebApi.Models;
using WebApi.Models.Enum;
using WebApi.Repository.Interface;
using WebApi.Services;

namespace WebApi.Tests.ServiceTests
{
    [TestClass]
    public class AccountingServiceTests
    {
        private Mock<ITransactionRepository> _mockRepository;
        private AccountingService _service;

        public AccountingServiceTests()
        {
            _mockRepository = new Mock<ITransactionRepository>();
            _service = new AccountingService(_mockRepository.Object);
        }

        [TestMethod]
        public void GetTransactionsForDay_WhenCalled_ReturnsFinancialReportForSpecificDay()
        {
            // Arrange
            var testDate = DateOnly.Parse("2024-01-18");
            var transactionsForDay = new List<Transaction>
            {
                new Transaction { Id = 1, Type = TransactionType.Income, Value = 20, Date = testDate },
                new Transaction { Id = 2, Type = TransactionType.Income, Value = 100, Date = testDate },
                new Transaction { Id = 3, Type = TransactionType.Expense, Value = 50, Date = testDate },
                new Transaction { Id = 4, Type = TransactionType.Expense, Value = 100, Date = testDate }
            };

            _mockRepository.Setup(repo => repo.GetTransactionsByDate(testDate)).Returns(transactionsForDay.AsQueryable());

            // Act
            var report = _service.GetTransactionsForDay(testDate);

            // Assert
            Assert.IsNotNull(report);
            Assert.AreEqual(transactionsForDay.Where(t => t.Type == TransactionType.Income).Sum(t => t.Value), report.TotalIncome);
            Assert.AreEqual(transactionsForDay.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Value), report.TotalExpense);
            Assert.AreEqual(transactionsForDay.Count, report.Transactions.Count());
            CollectionAssert.AreEqual(transactionsForDay, report.Transactions.ToList());
        }

        [TestMethod]
        public void GetTransactionsForPeriod_WhenCalled_ReturnsFinancialReportForSpecificPeriod()
        {
            // Arrange
            var startDate = DateOnly.Parse("2024-01-18");
            var endDate = DateOnly.Parse("2024-01-19");
            var transactionsForPeriod = new List<Transaction>
            {
                new Transaction { Id = 1, Type = TransactionType.Income, Value = 20, Date = DateOnly.Parse("2024-01-18") },
                new Transaction { Id = 2, Type = TransactionType.Income, Value = 100, Date = DateOnly.Parse("2024-01-18") },
                new Transaction { Id = 3, Type = TransactionType.Expense, Value = 50, Date = DateOnly.Parse("2024-01-18") },
                new Transaction { Id = 4, Type = TransactionType.Expense, Value = 100, Date = DateOnly.Parse("2024-01-18") },
                new Transaction { Id = 5, Type = TransactionType.Income, Value = 20, Date = DateOnly.Parse("2024-01-19") },
                new Transaction { Id = 6, Type = TransactionType.Expense, Value = 50, Date = DateOnly.Parse("2024-01-19") },
                new Transaction { Id = 7, Type = TransactionType.Expense, Value = 100, Date = DateOnly.Parse("2024-01-19") }
            };

            _mockRepository.Setup(repo => repo.GetTransactionsByPeriod(startDate, endDate)).Returns(transactionsForPeriod.AsQueryable());

            // Act
            var report = _service.GetTransactionsForPeriod(startDate, endDate);

            // Assert
            Assert.IsNotNull(report);
            Assert.AreEqual(transactionsForPeriod.Where(t => t.Type == TransactionType.Income).Sum(t => t.Value), report.TotalIncome);
            Assert.AreEqual(transactionsForPeriod.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Value), report.TotalExpense);
            Assert.AreEqual(transactionsForPeriod.Count, report.Transactions.Count());
            CollectionAssert.AreEqual(transactionsForPeriod, report.Transactions.ToList());
        }
    }
}
