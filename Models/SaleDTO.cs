using System.ComponentModel.DataAnnotations;

namespace TheChienHouse.Models
{
    public class SaleDTO
    {
        public record SaleCreateRequest(
            List<LineItem> LineItems,
            [Range(0.01, double.MaxValue, ErrorMessage = "Discount must be greater than 0")]
            decimal? SaleDiscount
        );

        public record SaleCreateResponse(
            long Id,
            List<LineItem> LineItems,
            [Range(0.01, double.MaxValue, ErrorMessage = "Sale total must be greater than 0")]
            decimal Total,
            [Range(0.01, double.MaxValue, ErrorMessage = "Discount must be greater than 0")]
            decimal? Discount,
            DateTime CreatedAt,
            DateTime UpdatedAt
        );
    }
}
