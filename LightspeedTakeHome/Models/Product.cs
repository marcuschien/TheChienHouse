using System.ComponentModel.DataAnnotations;

namespace LightspeedTakeHome.Models
{
    public class Product
    {
        public long Id { get; set; } // Make me a UUID

        [Required(ErrorMessage = "Product name is required")]
        [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters")]
        public string? Name { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // The timestamps are useful for tracing and debugging purposes 
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // Some customers find them useful as well
    }
}
