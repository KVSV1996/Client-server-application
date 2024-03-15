using BlazorUI.Data;
using BlazorUI.Service;
using Moq;
using RichardSzalay.MockHttp;
using System.Net;

namespace BlazorUI.Test.ServiceTests
{
    [TestClass]
    public class TransactionServiceTests
    {
        private MockHttpMessageHandler _mockHttp;
        private HttpClient _httpClient;
        private TransactionService _transactionService;

        public TransactionServiceTests()
        {
            _mockHttp = new MockHttpMessageHandler();
            _httpClient = _mockHttp.ToHttpClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7177/");

            var clientFactory = Mock.Of<IHttpClientFactory>(mock =>
                mock.CreateClient(It.IsAny<string>()) == _httpClient);

            _transactionService = new TransactionService(clientFactory);
        }

        [TestMethod]
        public async Task UpdateTransactionAsync_ValidRequest_SendsPutRequest()
        {
            // Arrange
            var transaction = new Transaction { Id = 1, Type = TransactionType.Income, Value = 100, Date = DateOnly.Parse("2024-01-18") };
            var token = "testToken";
            _mockHttp.When(HttpMethod.Put, "https://localhost:7177/transactions")
                     .Respond(HttpStatusCode.OK);

            // Act
            await _transactionService.UpdateTransactionAsync(transaction, token);

            // Assert
            _mockHttp.VerifyNoOutstandingExpectation();
        }

        [TestMethod]
        public async Task UpdateTransactionAsync_ServerError_ThrowsException()
        {
            // Arrange
            var transaction = new Transaction { Id = 1, Type = TransactionType.Income, Value = 100, Date = DateOnly.Parse("2024-01-18") };
            var token = "testToken";
            _mockHttp.When(HttpMethod.Put, "https://localhost:7177/transactions")
                     .Respond(HttpStatusCode.BadRequest);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<HttpRequestException>(() =>
                _transactionService.UpdateTransactionAsync(transaction, token));
        }

        [TestMethod]
        public async Task CreateTransactionAsync_ValidTransaction_ReturnsTrue()
        {
            // Arrange
            var transaction = new Transaction { Id = 40, Type = TransactionType.Income, Value = 200, Date = DateOnly.Parse("2024-01-20") };
            var token = "testToken";
            _mockHttp.When(HttpMethod.Post, "https://localhost:7177/transactions")
                     .Respond(HttpStatusCode.OK);

            // Act
            var result = await _transactionService.CreateTransactionAsync(transaction, token);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task CreateTransactionAsync_ServerError_ReturnsFalse()
        {
            // Arrange
            var transaction = new Transaction { Id = 40, Type = TransactionType.Income, Value = 200, Date = DateOnly.Parse("2024-01-20") };
            var token = "testToken";
            _mockHttp.When(HttpMethod.Post, "https://localhost:7177/transactions")
                     .Respond(HttpStatusCode.BadRequest);

            // Act
            await Assert.ThrowsExceptionAsync<HttpRequestException>(() =>
                _transactionService.CreateTransactionAsync(transaction, token));
        }

        [TestMethod]
        public async Task DeleteTransactionAsync_ValidId_SendsDeleteRequest()
        {
            // Arrange
            var transactionId = 1;
            var token = "testToken";
            _mockHttp.When(HttpMethod.Delete, $"https://localhost:7177/transactions/{transactionId}")
                     .Respond(HttpStatusCode.OK);

            // Act
            await _transactionService.DeleteTransactionAsync(transactionId, token);

            // Assert
            _mockHttp.VerifyNoOutstandingExpectation();
        }

        [TestMethod]
        public async Task DeleteTransactionAsync_ServerError_ThrowsException()
        {
            // Arrange
            var transactionId = 1;
            var token = "testToken";
            _mockHttp.When(HttpMethod.Delete, $"https://localhost:7177/transactions/{transactionId}")
                     .Respond(HttpStatusCode.NotFound);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<HttpRequestException>(() =>
                _transactionService.DeleteTransactionAsync(transactionId, token));
        }
    }
}
