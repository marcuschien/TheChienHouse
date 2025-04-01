namespace LightspeedTakeHome.Models
{
    public class SaleDTO
    {
        public record SaleCreateRequest(
            List<LineItem> LineItems,
            decimal? SaleDiscount
        );

        public record SaleCreateResponse(
            long Id,
            List<LineItem> LineItems,
            decimal Total,
            decimal? Discount,
            DateTime CreatedAt,
            DateTime UpdatedAt
        );
    }
}
