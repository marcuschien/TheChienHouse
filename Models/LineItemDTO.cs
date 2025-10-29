using System.ComponentModel.DataAnnotations;
using TheChienHouse.Models;

public class LineItemDTO
{
    public record LineItemCreateRequest
    {
        [Required(ErrorMessage = "Item name is required")]
        public required Guid MenuItemForSale { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public required int Quantity { get; set; }
        public decimal Discount { get; set; } = 0; // This is x percent off.
    }

    public record LineItemCreateResponse(
        Guid Id,
        MenuItem MenuItemForSale,
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        int Quantity,
        decimal TotalCost,
        decimal Discount,
        DateTime CreatedAt,
        DateTime? UpdatedAt
    );
}
    
