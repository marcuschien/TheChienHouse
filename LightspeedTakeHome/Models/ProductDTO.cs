using System.ComponentModel.DataAnnotations;

namespace LightspeedTakeHome.Models
{
    public class ProductDTO
    {
        /* I would probably make multiple records for each type of request and response, i.e. Product
         * CreateRequest, ProductCreateResponse, ProductGetRequest, ProductGetResponse, etc. 
         * But for the scope of this assignment, I've just kept it as one for simplicity. 
         */
        public record ProductCreateRequest(
            string Name,
            [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
            decimal Price
        );

        public record ProductGetRequest(
            long? Id
        );

        public record ProductResponse(
            long Id,
            string Name,
            [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
            decimal Price,
            DateTime CreatedAt,
            DateTime UpdatedAt
        );
    }
}
