using System.Text.Json.Serialization;
using WebApi.Models.Enum;

namespace WebApi.Models
{
    public class TransactionWithoutId
    {
        [JsonIgnore]
        public int Id { get; set; }
        public TransactionType Type { get; set; }
        public decimal Value { get; set; }
        public DateOnly Date { get; set; }

        public static explicit operator Transaction(TransactionWithoutId transWithoutId)
        {
            return new Transaction
            {
                Id = transWithoutId.Id,
                Type = transWithoutId.Type,
                Value = transWithoutId.Value,
                Date = transWithoutId.Date
            };
        }
    }
}
