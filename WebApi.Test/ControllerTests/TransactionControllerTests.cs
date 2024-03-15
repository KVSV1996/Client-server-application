using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApi.Controllers;
using WebApi.Models;
using WebApi.Models.Enum;
using WebApi.Services.Interface;

namespace WebApi.Tests.ControllerTests
{
    [TestClass]
    public class TransactionControllerTests
    {
        private Mock<ITransactionService> _mockService;
        private TransactionController _controller;

        public TransactionControllerTests()
        {
            _mockService = new Mock<ITransactionService>();
            _controller = new TransactionController(_mockService.Object);
        }

        #region Get

        [TestMethod]
        public void Get_ReturnsAllTransactions_WhenTransactionsExist()
        {
            // Arrange
            var transactions = new List<Transaction>
            {
                new Transaction{ Type = TransactionType.Income, Value = 20, Date = DateOnly.Parse("2024-01-18") },
                new Transaction { Type = TransactionType.Income, Value = 100, Date = DateOnly.Parse("2024-01-18") }
            };

            _mockService.Setup(service => service.GetAllTransactions()).Returns(transactions);

            // Act
            var result = _controller.Get();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedTransactions = okResult.Value as IEnumerable<Transaction>;
            Assert.IsNotNull(returnedTransactions);
            Assert.AreEqual(2, returnedTransactions.Count());
            Assert.AreEqual(20, returnedTransactions.ElementAt(0).Value);
            Assert.AreEqual(100, returnedTransactions.ElementAt(1).Value);
        }

        [TestMethod]
        public void Get_ReturnsNotFound_WhenNoTransactionsExist()
        {
            // Arrange
            _mockService.Setup(service => service.GetAllTransactions())
                .Returns(new List<Transaction>()); 

            // Act
            var result = _controller.Get();

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual("No transactions found.", notFoundResult.Value);
        }

        [TestMethod]
        public void Get_ThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            _mockService.Setup(service => service.GetAllTransactions())
                .Throws(new Exception("An error occurred"));

            // Act
            var result = _controller.Get();

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
            Assert.AreEqual("An error occurred while processing your request.", statusCodeResult.Value);
        }

        #endregion
        #region GetById

        [TestMethod]
        public void Get_ExistingId_ReturnsTransaction()
        {
            // Arrange
            int testId = 1;
            var testTransaction = new Transaction { Id = testId, Type = TransactionType.Income, Value = 100, Date = DateOnly.Parse("2024-01-18") };
            _mockService.Setup(service => service.GetTransactionById(testId)).Returns(testTransaction);

            // Act
            var result = _controller.Get(testId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var transaction = okResult.Value as Transaction;
            Assert.IsNotNull(transaction);
            Assert.AreEqual(testTransaction, transaction);
        }

        [TestMethod]
        public void Get_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            int testId = 1;
            _mockService.Setup(service => service.GetTransactionById(testId)).Returns((Transaction)null);

            // Act
            var result = _controller.Get(testId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual($"Transaction with ID: {testId} not found.", notFoundResult.Value);
        }

        [TestMethod]
        public void Get_WhenExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            int testId = 1;
            _mockService.Setup(service => service.GetTransactionById(testId)).Throws(new Exception("An error occurred"));

            // Act
            var result = _controller.Get(testId);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
            Assert.AreEqual("An error occurred while processing your request.", statusCodeResult.Value);
        }

        #endregion
        #region Delete

        [TestMethod]
        public void Delete_ValidId_DeletesTransactionAndReturnsSuccess()
        {
            // Arrange
            int testId = 1;
            _mockService.Setup(service => service.TransactionDelete(testId));

            // Act
            var result = _controller.Delete(testId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual($"Transaction with ID: {testId} was deleted successfully.", okResult.Value);
        }

        [TestMethod]
        public void Delete_WhenExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            int testId = 1;
            _mockService.Setup(service => service.TransactionDelete(testId)).Throws(new Exception("An error occurred"));

            // Act
            var result = _controller.Delete(testId);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
            Assert.AreEqual("An error occurred while processing your request.", statusCodeResult.Value);
        }

        #endregion
        #region Put

        [TestMethod]
        public void Put_ValidTransaction_UpdatesTransactionAndReturnsSuccess()
        {
            // Arrange
            var testTransaction = new Transaction { Id = 1, Type = TransactionType.Income, Value = 100, Date = DateOnly.Parse("2024-01-18") };
            _mockService.Setup(service => service.TransactionUpdate(testTransaction));

            // Act
            var result = _controller.Put(testTransaction);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual($"Transaction with ID: {testTransaction.Id} updated successfully.", okResult.Value);
        }

        [TestMethod]
        public void Put_WhenExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            var testTransaction = new Transaction { Id = 1, Type = TransactionType.Income, Value = 100, Date = DateOnly.Parse("2024-01-18") };
            _mockService.Setup(service => service.TransactionUpdate(testTransaction)).Throws(new Exception("An error occurred"));

            // Act
            var result = _controller.Put(testTransaction);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
            Assert.AreEqual("An error occurred while processing your request.", statusCodeResult.Value);
        }

        #endregion
        #region Post

        [TestMethod]
        public void Post_ValidTransaction_CreatesTransactionAndReturnsSuccess()
        {
            // Arrange
            var testTransaction = new TransactionWithoutId { Type = TransactionType.Income, Value = 445, Date = DateOnly.Parse("2024-02-05") };
            _mockService.Setup(service => service.TransactionCreate(testTransaction));

            // Act
            var result = _controller.Post(testTransaction);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual("A new transaction was created.", okResult.Value);
        }

        [TestMethod]
        public void Post_NullTransaction_ReturnsBadRequest()
        {
            // Arrange
            TransactionWithoutId nullTransaction = null;

            // Act
            var result = _controller.Post(nullTransaction);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("Transaction cannot be null.", badRequestResult.Value);
        }

        [TestMethod]
        public void Post_WhenExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            var testTransaction = new TransactionWithoutId {};
            _mockService.Setup(service => service.TransactionCreate(testTransaction)).Throws(new Exception("An error occurred"));

            // Act
            var result = _controller.Post(testTransaction);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
            Assert.AreEqual("An error occurred while processing your request.", statusCodeResult.Value);
        }
        #endregion

    }
}
