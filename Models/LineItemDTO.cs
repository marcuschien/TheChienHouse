using System.ComponentModel.DataAnnotations;
using TheChienHouse.Models;

public class LineItemDTO
{
    public record LineItemCreateRequest
    {
        [Required(ErrorMessage = "Item name is required")]
        public string? MenuItemForSale { get; set; }

        [Range(1, long.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public long Quantity { get; set; }
    }
}
    
