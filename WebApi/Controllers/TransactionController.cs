using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using Serilog;
using Microsoft.AspNetCore.Authorization;
using WebApi.Services.Interface;


namespace WebApi.Controllers
{
    [ApiController]
    [Route("/transactions")]
    [Authorize]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                Log.Information("Getting all transactions");
                var transactions = _transactionService.GetAllTransactions();

                if (transactions.Any())
                {
                    return Ok(transactions);
                }
                else
                {
                    Log.Warning("No transactions found.");
                    return NotFound("No transactions found.");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while retrieving transactions.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            Log.Information($"Attempting to get transaction with ID: {id}");
            try
            {
                var transaction = _transactionService.GetTransactionById(id);
                if (transaction != null)
                {
                    Log.Information($"Transaction with ID: {id} retrieved successfully.");
                    return Ok(transaction);
                }
                else
                {
                    Log.Warning($"Transaction with ID: {id} not found.");
                    return NotFound($"Transaction with ID: {id} not found.");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"An error occurred while retrieving transaction with ID: {id}.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            Log.Information($"Attempting to delete transaction with ID: {id}");
            try
            {
                _transactionService.TransactionDelete(id);
                Log.Information($"Transaction with ID: {id} was deleted successfully.");
                return Ok($"Transaction with ID: {id} was deleted successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"An error occurred while deleting transaction with ID: {id}.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPut]
        public IActionResult Put(Transaction transaction)
        {
            Log.Information($"Attempting to update transaction with ID: {transaction.Id}");
            try
            {
                _transactionService.TransactionUpdate(transaction);
                Log.Information($"Transaction with ID: {transaction.Id} updated successfully.");
                return Ok($"Transaction with ID: {transaction.Id} updated successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"An error occurred while updating transaction with ID: {transaction.Id}.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost]
        public IActionResult  Post(TransactionWithoutId transaction)
        {
            Log.Information("Attempting to create a new transaction");

            if (transaction == null)
            {
                Log.Warning("Attempted to create a null transaction.");
                return BadRequest("Transaction cannot be null.");
            }

            try
            {
                _transactionService.TransactionCreate(transaction);
                 Log.Information($"A new transaction was created");
                return Ok("A new transaction was created.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while creating a new transaction.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
