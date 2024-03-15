using Moq;
using WebApi.Models;
using WebApi.Models.Enum;
using WebApi.Repository.Interface;
using WebApi.Services;

namespace WebApi.Tests.ServiceTests
{
    [TestClass]
    public class TransactionServiceTests
    {
        private Mock<ITransactionRepository> _mockRepository;
        private TransactionService _service;

        
        public TransactionServiceTests()
        {
            _mockRepository = new Mock<ITransactionRepository>();
            _service = new TransactionService(_mockRepository.Object);
        }

        [TestMethod]
        public void GetAllTransactions_ReturnsAllTransactions()
        {
            // Arrange
            var testTransactions = new List<Transaction> 
            {
                new Transaction { Id = 1, Type = TransactionType.Income, Value = 20, Date = DateOnly.Parse("2024-01-18") },
                new Transaction { Id = 2, Type = TransactionType.Income, Value = 100, Date = DateOnly.Parse("2024-01-18") },
                new Transaction { Id = 3, Type = TransactionType.Expense, Value = 50, Date = DateOnly.Parse("2024-01-18") },
                new Transaction { Id = 4, Type = TransactionType.Expense, Value = 100, Date = DateOnly.Parse("2024-01-18") }
            };
            _mockRepository.Setup(repo => repo.GetAllTransactions()).Returns(testTransactions.AsQueryable());

            // Act
            var transactions = _service.GetAllTransactions();

            // Assert
            Assert.IsNotNull(transactions);
            Assert.AreEqual(testTransactions.Count, transactions.Count());
        }

        [TestMethod]
        public void GetTransactionById_ExistingId_ReturnsTransaction()
        {
            // Arrange
            int testId = 1;
            var testTransaction = new Transaction { Id = testId, Type = TransactionType.Income, Value = 20, Date = DateOnly.Parse("2024-01-18") };
            _mockRepository.Setup(repo => repo.GetTransactionById(testId)).Returns(testTransaction);

            // Act
            var transaction = _service.GetTransactionById(testId);

            // Assert
            Assert.IsNotNull(transaction);
            Assert.AreEqual(testId, transaction.Id);
            Assert.AreEqual(testTransaction.Value, transaction.Value);
        }

        [TestMethod]
        public void TransactionCreate_ValidTransaction_AddsTransactionAndSaves()
        {
            // Arrange
            var testTransactionWithoutId = new TransactionWithoutId { Type = TransactionType.Income, Value = 30, Date = DateOnly.Parse("2024-02-05") };
            _mockRepository.Setup(repo => repo.InsertTransaction(It.IsAny<Transaction>()));
            _mockRepository.Setup(repo => repo.Save());

            // Act
            _service.TransactionCreate(testTransactionWithoutId);

            // Assert
            _mockRepository.Verify(repo => repo.InsertTransaction(It.IsAny<Transaction>()), Times.Once());
            _mockRepository.Verify(repo => repo.Save(), Times.Once());
        }

        [TestMethod]
        public void TransactionUpdate_ValidTransaction_UpdatesTransactionAndSaves()
        {
            // Arrange
            var testTransaction = new Transaction { Id = 1, Type = TransactionType.Income, Value = 225, Date = DateOnly.Parse("2024-01-18") };
            _mockRepository.Setup(repo => repo.UpdateTransaction(It.IsAny<Transaction>()));
            _mockRepository.Setup(repo => repo.Save());

            // Act
            _service.TransactionUpdate(testTransaction);

            // Assert
            _mockRepository.Verify(repo => repo.UpdateTransaction(It.IsAny<Transaction>()), Times.Once());
            _mockRepository.Verify(repo => repo.Save(), Times.Once());
        }

        [TestMethod]
        public void TransactionDelete_ExistingId_DeletesTransactionAndSaves()
        {
            // Arrange
            int testId = 1;
            _mockRepository.Setup(repo => repo.GetAllTransactions()).Returns(new List<Transaction> { new Transaction { Id = testId } }.AsQueryable());
            _mockRepository.Setup(repo => repo.DeleteTransaction(testId));
            _mockRepository.Setup(repo => repo.Save());

            // Act
            _service.TransactionDelete(testId);

            // Assert
            _mockRepository.Verify(repo => repo.DeleteTransaction(testId), Times.Once());
            _mockRepository.Verify(repo => repo.Save(), Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TransactionDelete_NonExistingId_ThrowsException()
        {
            // Arrange
            int testId = 1;
            _mockRepository.Setup(repo => repo.GetAllTransactions()).Returns(new List<Transaction>().AsQueryable());

            // Act
            _service.TransactionDelete(testId);
        }
    }
}
