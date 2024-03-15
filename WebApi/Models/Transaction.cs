using WebApi.Models.Enum;

namespace WebApi.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public TransactionType Type { get; set; }
        public decimal Value { get; set; }
        public DateOnly Date { get; set; }
    }
}
