using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LightspeedTakeHome.Models;

namespace LightspeedTakeHome.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly RetailContext _context;

        public SalesController(RetailContext context)
        {
            _context = context;
        }

        // GET: api/Sales
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Sale>>> GetSales()
        {
            return await _context.Sales.ToListAsync();
        }

        // GET: api/Sales/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Sale>> GetSale(long id)
        {
            var sale = await _context.Sales.FindAsync(id);

            if (sale == null)
            {
                return NotFound();
            }

            return sale;
        }

        // PUT: api/Sales/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSale(long id, Sale sale)
        {
            if (id != sale.Id)
            {
                return BadRequest();
            }

            _context.Entry(sale).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SaleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Sales
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Sale>> PostSale(Sale sale)
        {
            
            List<double> lineItemTotals = new List<double>(); // Index order is guaranteed with Lists in C# so it should match up with the LineItems; there's probably a more robust way to do this
            // For each line item in the sale, calculate the total cost of the line item and add it to the total of the sale
            foreach (LineItem lineItem in sale.LineItems)
            {
                lineItem.ProductForSale = await _context.Products.FindAsync(lineItem.ProductId);
                //Write a sanity check for lineItem.ProductForSale
                lineItem.TotalCost = lineItem.ProductForSale.Price * lineItem.Quantity;
                lineItem.TotalCost = Math.Round(lineItem.TotalCost, 2);
                lineItemTotals.Add(lineItem.TotalCost);
                sale.Total += lineItem.TotalCost;
            }
            if (sale.SaleDiscount != null)
            {
                foreach (LineItem lineItem in sale.LineItems)
                {
                    lineItem.LineItemDiscount = Math.Round((lineItem.TotalCost / sale.Total) * (double)sale.SaleDiscount, 2);
                    lineItem.TotalCost -= lineItem.LineItemDiscount;
                }
                sale.Total -= sale.SaleDiscount.Value;
            }
            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetSale", new { id = sale.Id}, sale);
        }

        // DELETE: api/Sales/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSale(long id)
        {
            var sale = await _context.Sales.FindAsync(id);
            if (sale == null)
            {
                return NotFound();
            }

            _context.Sales.Remove(sale);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SaleExists(long id)
        {
            return _context.Sales.Any(e => e.Id == id);
        }
    }
}
