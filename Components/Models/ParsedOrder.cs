using System.Collections.Generic;

namespace Bestella.Models
{
    public class ParsedOrder
    {
        public long Id { get; set; }
        public string Name { get; set; } = "";
        public string Address { get; set; } = "";
        public string Date { get; set; } = "";
        public string Phone { get; set; } = "";
        public List<OrderItem> Items { get; set; } = new();
    }
}