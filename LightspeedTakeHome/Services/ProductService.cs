using LightspeedTakeHome.Models;
using Microsoft.EntityFrameworkCore;
using static LightspeedTakeHome.Models.ProductDTO;

namespace LightspeedTakeHome.Services
{
    public class ProductService : IProductService
    {
        private readonly RetailContext _context;
        private readonly ILogger<ProductService> _logger; //Used for tracing, helpful in prod issues
        public ProductService(RetailContext context, ILogger<ProductService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<ProductResponse> CreateProductAsync(ProductCreateRequest request)
        {
            var product = new Product
            {
                Name = request.Name,
                Price = request.Price
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created product {ProductId}", product.Id);

            return MapToResponse(product);
        }
        
        public async Task<IEnumerable<ProductResponse>> GetProductsAsync() // TODO: Introduce parameters int page and int pagesize to allow for pagination
        {
            // TODO: Put the returned answer into ProductResponse form to return to the controller
            List<Product> products = await _context.Products.ToListAsync();
            List<ProductResponse> response = new List<ProductResponse>();
            foreach (Product product in products)
            {
                response.Add(MapToResponse(product));
            }
            return response;
        }

        public async Task<ProductResponse?> GetProductByIdAsync(long id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                _logger.LogError("Product {ProductId} not found", id);
                return null; //Add some exception here. Response should throw a different error code, depending on what we want the response to be. 
            }

            return MapToResponse(product);
        }

        private static ProductResponse MapToResponse(Product product) =>
            new(product.Id, product.Name, product.Price, product.CreatedAt, product.UpdatedAt);

    }
}
