using static LightspeedTakeHome.Models.ProductDTO;

namespace LightspeedTakeHome.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductResponse>> GetProductsAsync(); // TODO: Introduce parameters int page and int pagesize to allow for pagination
        Task<ProductResponse> CreateProductAsync(ProductCreateRequest request);
        Task<ProductResponse?> GetProductByIdAsync(long id);
    }
}
