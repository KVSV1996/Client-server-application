using Moq;
using WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using WebApi.Models.Enum;
using WebApi.Models;
using WebApi.Services.Interface;

namespace WebApi.Tests.ControllerTests
{
    [TestClass]
    public class AccountingControllerTests
    {
        private readonly Mock<IAccountingService> _mockAccountingService;
        private readonly AccountingController _controller;

        public AccountingControllerTests()
        {
            _mockAccountingService = new Mock<IAccountingService>();
            _controller = new AccountingController(_mockAccountingService.Object);
        }

        #region Get

        [TestMethod]
        public void Get_ValidDate_ReturnsOkWithDailyReport()
        {
            // Arrange
            var validDateString = "2024-01-18";
            var testDate = DateOnly.ParseExact(validDateString, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            var dailyReport = new FinancialReport
            {
                TotalIncome = 120,
                TotalExpense = 150,
                Transactions = new List<Transaction>
                {
                    new Transaction { Id = 1, Type = TransactionType.Income, Value = 20, Date = testDate },
                    new Transaction { Id = 2, Type = TransactionType.Income, Value = 100, Date = testDate },
                    new Transaction { Id = 3, Type = TransactionType.Expense, Value = 50, Date = testDate },
                    new Transaction { Id = 4, Type = TransactionType.Expense, Value = 100, Date = testDate }
                }
            };

            _mockAccountingService.Setup(s => s.GetTransactionsForDay(It.IsAny<DateOnly>()))
                .Returns(dailyReport);

            // Act
            var result = _controller.Get(validDateString);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            var report = okResult.Value as FinancialReport;
            Assert.IsNotNull(report);
            Assert.AreEqual(dailyReport.TotalIncome, report.TotalIncome);
            Assert.AreEqual(dailyReport.TotalExpense, report.TotalExpense);
            Assert.AreEqual(dailyReport.Transactions.Count(), report.Transactions.Count());
        }

        [TestMethod]
        public void Get_InvalidDate_ReturnsBadRequest()
        {
            // Arrange
            var invalidDateString = "20240118";
            _mockAccountingService.Setup(s => s.GetTransactionsForDay(It.IsAny<DateOnly>()))
                .Returns((FinancialReport)null);

            // Act
            var result = _controller.Get(invalidDateString);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.IsInstanceOfType(badRequestResult.Value, typeof(string));
            Assert.IsTrue(badRequestResult.Value.ToString().Contains("The date format is incorrect"));
        }

        #endregion

        #region GetTransactionsForPeriod

        [TestMethod]
        public void GetTransactionsForPeriod_ValidDates_ReturnsOkWithReport()
        {
            // Arrange
            var startDateString = "2024-01-18";
            var endDateString = "2024-01-19";
            var startDate = DateOnly.ParseExact(startDateString, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var endDate = DateOnly.ParseExact(endDateString, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            var report = new FinancialReport
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

            _mockAccountingService.Setup(s => s.GetTransactionsForPeriod(startDate, endDate))
                .Returns(report);

            // Act
            var result = _controller.GetTransactionsForPeriod(startDateString, endDateString);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            var returnedReport = okResult.Value as FinancialReport;
            Assert.IsNotNull(returnedReport);
            Assert.AreEqual(report.TotalIncome, returnedReport.TotalIncome);
            Assert.AreEqual(report.TotalExpense, returnedReport.TotalExpense);
            Assert.AreEqual(report.Transactions.Count(), returnedReport.Transactions.Count());
        }

        [TestMethod]
        public void GetTransactionsForPeriod_StartDateLaterThanEndDate_ReturnsBadRequest()
        {
            // Arrange
            var startDateString = "2024-01-19";
            var endDateString = "2024-01-18";

            // Act
            var result = _controller.GetTransactionsForPeriod(startDateString, endDateString);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("The start date must be earlier than the end date.", badRequestResult.Value);
        }

        [TestMethod]
        public void GetTransactionsForPeriod_IncorrectDateFormat_ReturnsBadRequest()
        {
            // Arrange
            var incorrectStartDateString = "2024-01";
            var incorrectEndDateString = "01-18";

            // Act
            var result = _controller.GetTransactionsForPeriod(incorrectStartDateString, incorrectEndDateString);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.IsTrue(badRequestResult.Value.ToString().Contains("One or both dates are in an incorrect format"));
        }

        #endregion

    }
}


