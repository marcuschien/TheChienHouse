
using Microsoft.AspNetCore.Mvc;
using TheChienHouse.Models;
using TheChienHouse.Services;
using static LineItemDTO;
using static TheChienHouse.Models.SaleDTO;

namespace TheChienHouse.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly ISaleService _saleService;
        private readonly ILineItemService _lineItemService;

        public SalesController(ISaleService saleService, ILineItemService lineItemService)
        {
            _saleService = saleService;
            _lineItemService = lineItemService;
        }

        // POST: api/Sales
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SaleResponse>> PostSale(SaleRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            Dictionary<Guid, Tuple<int, decimal>> lineItemDict = new Dictionary<Guid, Tuple<int, decimal>>();
            List<LineItem> lineItems = await _lineItemService.CreateLineItemsAsync(lineItemDict);
            Sale sale = await _saleService.CreateSaleAsync(lineItems, request.SaleDiscount);
            return CreatedAtAction(nameof(PostSale), MapToResponse(sale));
        }

        [HttpGet]
        public async Task<ActionResult<List<SaleResponse>>> GetSales()
        {
            List<Sale> sales = await _saleService.GetAllSalesAsync();
            List<SaleResponse> response = sales.Select(sale => MapToResponse(sale)).ToList();
            return Ok(response);
        }

        private SaleResponse MapToResponse(Sale sale)
        {
            return new SaleResponse
            (
                sale.Id,
                sale.LineItems,
                sale.Total,
                sale.Discount,
                sale.CreatedAt,
                sale.UpdatedAt
            );
        }
    }
}
