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
        private readonly RetailContext _context;
        private readonly ISaleService _saleService;

        public SalesController(RetailContext context, ISaleService saleService)
        {
            _context = context;
            _saleService = saleService;
        }

        // POST: api/Sales
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Sale>> PostSale(SaleCreateRequest request)
        {
            SaleCreateResponse response = await _saleService.CreateSaleAsync(request);
            return CreatedAtAction(nameof(PostSale), new { id = response.Id }, response);
        }
    }
}
