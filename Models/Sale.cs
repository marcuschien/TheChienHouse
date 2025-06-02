using System.ComponentModel.DataAnnotations;

namespace TheChienHouse.Models
{
    public class Sale
    {
        public long Id { get; set; } // Make me a UUID
        public required List<LineItem> LineItems {get; set;}
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Total {get; set;}
        [Range(0.01, double.MaxValue, ErrorMessage = "Discount must be greater than 0")]
        public decimal? SaleDiscount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // The timestamps are useful for tracing and debugging purposes 
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // Some customers find them useful as well
    }
}
