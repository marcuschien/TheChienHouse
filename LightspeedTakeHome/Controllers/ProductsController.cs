using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LightspeedTakeHome.Models;
using LightspeedTakeHome.Services;
using static LightspeedTakeHome.Models.ProductDTO;

namespace LightspeedTakeHome.Controllers
{
    [Route("api/[controller]")]
    [ApiController] // This attribute indicates that the controller responds to web API requests
    public class ProductsController : ControllerBase
    {
        private readonly RetailContext _context; // Using in memory database
        private readonly IProductService _productService; // Utilizing the service layer to fetch requested data

        public ProductsController(RetailContext context, IProductService productService)
        {
            _context = context;
            _productService = productService;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            IEnumerable<ProductResponse> response = await _productService.GetProductsAsync();
            return CreatedAtAction(nameof(GetProducts), response); // Response could possibly be null, how would we handle a product being null? 
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(long id)
        {
            ProductResponse? response = await _productService.GetProductByIdAsync(id);
            return CreatedAtAction(nameof(GetProduct),response); // Response could possibly be null, need to handle this case. 
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(ProductCreateRequest request)
        {
            ProductResponse response = await _productService.CreateProductAsync(request);
            return CreatedAtAction(nameof(PostProduct), new { id = response.Id }, response);
        }
    }
}
