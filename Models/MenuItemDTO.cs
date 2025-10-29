using System.ComponentModel.DataAnnotations;

namespace TheChienHouse.Models
{
    public class MenuItemDTO
    {
        /* I would probably make multiple records for each type of request and response, i.e. Product
         * CreateRequest, ProductCreateResponse, ProductGetRequest, ProductGetResponse, etc. 
         * But for the scope of this assignment, I've just kept it as one for simplicity. 
         */
        public record CreateMenuItemRequest(
            string Name,
            [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
            decimal Price,
            DishType DishType
        );

        public record MenuItemRequest(
            Guid Id
        );

        public record GetMenuItemsRequest (
            string? Name = null,
            DishType? DishType = null
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

        public record UpdateMenuItemRequest(
            string OldName,
            DishType OldType,
            decimal? NewPrice = null,
            DishType? NewType = null,
            string? NewName = null
        );
    }
}
