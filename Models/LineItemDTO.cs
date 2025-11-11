using System.ComponentModel.DataAnnotations;
using TheChienHouse.Models;

public class LineItemDTO
{
    public record LineItemCreateRequest
    (
        Guid MenuItemForSale,
        int Quantity,
        decimal Discount // This is x percent off.
    );

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
    
