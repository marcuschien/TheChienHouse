using System.ComponentModel.DataAnnotations;

namespace TheChienHouse.Models
{
    public class LineItem
    {
        public long Id { get; set; } // Make me a UUID
        public DishName MenuItemForSale { get; set; } // Using this to pull the Product from the DB. We should fetch the earliest created one
        [Range(1, long.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public required long Quantity { get; set; }
        public decimal TotalCost { get; set; }
        public decimal LineItemDiscount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // The timestamps are useful for tracing and debugging purposes 
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // Some customers find them useful as well
    }
}
