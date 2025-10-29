using System.ComponentModel.DataAnnotations;

namespace TheChienHouse.Models
{
    public class Sale
    {
        public Guid Id { get; set; }
        public required List<LineItem> LineItems {get; set;}
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Discount { get; set; } = 0; //This is x percent off. 
        public decimal Total => LineItems.Sum(li => li.TotalCost) - Discount;
        [Range(0.01, double.MaxValue, ErrorMessage = "Discount must be greater than 0")]
        public DateTime CreatedAt { get; init; } 
        public DateTime? UpdatedAt { get; set; } = null;
    }
}
