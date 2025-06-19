using System.ComponentModel.DataAnnotations.Schema;

namespace Bestella.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = "";
        public int Quantity { get; set; } = 1;

        public long ParsedOrderId { get; set; }

        public ParsedOrder ParsedOrder { get; set; } = null!;
    }
}