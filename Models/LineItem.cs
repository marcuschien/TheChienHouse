using System.ComponentModel.DataAnnotations;

namespace TheChienHouse.Models
{
    public class LineItem
    {
        public Guid Id { get; set; }
        public Guid SaleId { get; set; }
        [Required(ErrorMessage = "Item name is required")]
        public required Guid MenuItemForSale { get; set; }
        [Range(1, long.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public required long Quantity { get; set; }
        public decimal PricePerItem { get; set; }
        public decimal SubTotal => PricePerItem * Quantity;
        public decimal Discount { get; set; } = 0; // This is x percent off. 
        public decimal TotalCost => SubTotal - (SubTotal*Discount);
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; set; } = null;

        //Do I want to add an expiry date here?
    }
}
