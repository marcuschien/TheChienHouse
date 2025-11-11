using System.ComponentModel.DataAnnotations;
using static LineItemDTO;

namespace TheChienHouse.Models
{
    public class SaleDTO
    {
        public record SaleRequest(
            List<LineItemCreateRequest> LineItems,
            [Range(0.01, double.MaxValue, ErrorMessage = "Discount must be greater than 0")]
            decimal SaleDiscount = 0
        );

        public record SaleResponse(
            Guid Id,
            List<LineItem> LineItems,
            [Range(0.01, double.MaxValue, ErrorMessage = "Sale total must be greater than 0")]
            decimal Total,
            [Range(0.01, double.MaxValue, ErrorMessage = "Discount must be greater than 0")]
            decimal? Discount,
            DateTime CreatedAt,
            DateTime? UpdatedAt
        );
    }
}
