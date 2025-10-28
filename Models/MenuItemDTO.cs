using System.ComponentModel.DataAnnotations;

namespace TheChienHouse.Models
{
    public class MenuItemDTO
    {
        /* I would probably make multiple records for each type of request and response, i.e. Product
         * CreateRequest, ProductCreateResponse, ProductGetRequest, ProductGetResponse, etc. 
         * But for the scope of this assignment, I've just kept it as one for simplicity. 
         */
        public record MenuItemCreateRequest(
            string Name,
            [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
            decimal Price,
            DishType DishType
        );

        public record MenuItemGetRequest(
            long? Id
        );

        public record MenuItemResponse(
            Guid Id,
            string Name,
            [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
            decimal Price,
            DishType DishType,
            DateTime? ExpiryDate,
            DateTime CreatedAt,
            DateTime? UpdatedAt
        );

        public record MenuItemsResponse(
            IEnumerable<MenuItem> MenuItems
        );

        public record MenuItemUpdateRequest(
            string OldName,
            DishType OldType,
            decimal? NewPrice = null,
            DishType? NewType = null,
            string? NewName = null
        );
    }
}
