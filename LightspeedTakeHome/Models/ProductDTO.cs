namespace LightspeedTakeHome.Models
{
    public class ProductDTO
    {
        /* I would probably make multiple records for each type of request and response, i.e. Product
         * CreateRequest, ProductCreateResponse, ProductGetRequest, ProductGetResponse, etc. 
         * But for the scope of this assignment, I've just kept it as one for simplicity. 
         */
        public record ProductCreateRequest(
            string Name, // Can I make these required?
            decimal Price
        );

        public record ProductGetRequest(
            long? Id
        );

        public record ProductResponse(
            long Id,
            string Name,
            decimal Price,
            DateTime CreatedAt,
            DateTime UpdatedAt
        );
    }
}
