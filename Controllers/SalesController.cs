using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheChienHouse.Models;
using TheChienHouse.Services;
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
        public async Task<ActionResult<SaleCreateResponse>> PostSale(SaleCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            List<LineItem> lineItems = _lineItemService.CreateLineItemsAsync(request.LineItems).Result;
            Sale sale = await _saleService.CreateSaleAsync(lineItems, request.SaleDiscount);
            return CreatedAtAction(nameof(PostSale), MapToResponse(sale));
        }

        private SaleCreateResponse MapToResponse(Sale sale)
        {
            return new SaleCreateResponse
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
