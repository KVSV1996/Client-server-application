using BlazorUI.Data;
using BlazorUI.Service;
using Moq;
using RichardSzalay.MockHttp;
using System.Net;
using System.Text.Json;

namespace BlazorUI.Test.ServiceTests
{
    [TestClass]
    public class AccountingServiceTests
    {
        private MockHttpMessageHandler _mockHttp;
        private HttpClient _httpClient;
        private AccountingService _accountingService;

        public AccountingServiceTests()
        {
            _mockHttp = new MockHttpMessageHandler();
            _httpClient = _mockHttp.ToHttpClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7177/");

            var clientFactory = Mock.Of<IHttpClientFactory>(mock =>
                mock.CreateClient(It.IsAny<string>()) == _httpClient);

            _accountingService = new AccountingService(clientFactory);
        }

        [TestMethod]
        public async Task GetTransactionsForPeriodAsync_ValidToken_ReturnsFinancialReport()
        {
            // Arrange
            var startDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-2));
            var endDate = DateOnly.FromDateTime(DateTime.Now);
            var token = "testToken";

            var expectedReport = new FinancialReport
            {
                TotalIncome = 140,
                TotalExpense = 300,
                Transactions = new List<Transaction>
                {
                    new Transaction { Id = 1, Type = TransactionType.Income, Value = 20, Date = new DateOnly(2024, 1, 18) },
                    new Transaction { Id = 2, Type = TransactionType.Income, Value = 100, Date = new DateOnly(2024, 1, 18) },
                    new Transaction { Id = 3, Type = TransactionType.Expense, Value = 50, Date = new DateOnly(2024, 1, 18) },
                    new Transaction { Id = 4, Type = TransactionType.Expense, Value = 100, Date = new DateOnly(2024, 1, 18) },
                    new Transaction { Id = 5, Type = TransactionType.Income, Value = 20, Date = new DateOnly(2024, 1, 19) },
                    new Transaction { Id = 6, Type = TransactionType.Expense, Value = 50, Date = new DateOnly(2024, 1, 19) },
                    new Transaction { Id = 7, Type = TransactionType.Expense, Value = 100, Date = new DateOnly(2024, 1, 19) }
                }
            };

            _mockHttp.When(HttpMethod.Get, "https://localhost:7177/accounting/interval*")
         .Respond("application/json", JsonSerializer.Serialize(expectedReport));

            // Act
            var result = await _accountingService.GetTransactionsForPeriodAsync(startDate, endDate, token);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedReport.TotalIncome, result.TotalIncome);
            Assert.AreEqual(expectedReport.TotalExpense, result.TotalExpense);
        }

        [TestMethod]
        public async Task GetTransactionsForPeriodAsync_ServerError_ThrowsException()
        {
            // Arrange
            var startDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-2));
            var endDate = DateOnly.FromDateTime(DateTime.Now);
            var token = "testToken";


            _mockHttp.When(HttpMethod.Get, "https://localhost:7177/accounting/interval*")
                     .Respond(HttpStatusCode.Unauthorized);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<HttpRequestException>(async () =>
                await _accountingService.GetTransactionsForPeriodAsync(startDate, endDate, token));
        }
    }
}
