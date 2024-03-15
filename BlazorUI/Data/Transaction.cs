using System.ComponentModel.DataAnnotations;

namespace BlazorUI.Data
{
    public class Transaction
    {
        public int Id { get; set; }
        public TransactionType Type { get; set; }

        [Range(0.01, Double.MaxValue, ErrorMessage = "Value must be greater than zero")]
        public decimal Value { get; set; }
        public DateOnly Date { get; set; }
    }
    public enum TransactionType
    {
        Income,
        Expense
    }
}
