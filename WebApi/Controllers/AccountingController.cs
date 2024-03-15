using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Serilog;
using Microsoft.AspNetCore.Authorization;
using WebApi.Services.Interface;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("/accounting")]
    [Authorize]
    public class AccountingController : ControllerBase
    {
        IAccountingService _accountingService;
        public AccountingController(IAccountingService accountingService) 
        {
            _accountingService = accountingService;
        }

        [HttpGet("daily")]
        public IActionResult Get([FromQuery(Name = "date")] string dateString)
        {            
            var dateFormat = "yyyy-MM-dd";
            
            if (DateOnly.TryParseExact(dateString, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly date))
            {
                Log.Information($"Received request to get transactions for day: {dateString}");
                var report = _accountingService.GetTransactionsForDay(date);
                return Ok(report);
            }
            else
            {
                Log.Warning($"The date format is incorrect. User enter {dateString}.");
                return BadRequest($"The date format is incorrect. Please use the format: {dateFormat}.");
            }
        }

        [HttpGet("interval")]
        public IActionResult GetTransactionsForPeriod([FromQuery(Name = "startDate")] string startDateString, [FromQuery(Name = "endDate")] string endDateString)
        {
            var dateFormat = "yyyy-MM-dd";

            if (DateOnly.TryParseExact(startDateString, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly startDate) &&
                DateOnly.TryParseExact(endDateString, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly endDate))
            {
                if (startDate <= endDate)
                {
                    Log.Information($"Received request to get transactions for period from {startDateString} to {endDateString}");
                    var report = _accountingService.GetTransactionsForPeriod(startDate, endDate);
                    return Ok(report);
                }
                else
                {
                    Log.Warning("The start date must be earlier than the end date.");
                    return BadRequest("The start date must be earlier than the end date.");
                }
            }
            else
            {
                Log.Warning($"One or both dates are in an incorrect format. StartDate: {startDateString}, EndDate: {endDateString}");
                return BadRequest($"One or both dates are in an incorrect format. Please use the format: {dateFormat} for both dates.");
            }
        }
    }
}
