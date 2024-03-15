namespace WebApi.Models
{
    public class FinancialReport
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public List<Transaction> Transactions { get; set; }
    }
}
