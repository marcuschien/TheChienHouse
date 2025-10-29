using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TheChienHouse.Models;
using static LineItemDTO;
using static TheChienHouse.Models.MenuItemDTO;
using static TheChienHouse.Models.SaleDTO;

namespace TheChienHouse.Services
{
    public class SaleService : ISaleService
    {
        private readonly RetailContext _context;
        private readonly ILogger<SaleService> _logger;
        private readonly ILineItemService _lineItemService;
        public SaleService(RetailContext context, ILogger<SaleService> logger, ILineItemService lineItemService) 
        {
            _context = context;
            _logger = logger;
            _lineItemService = lineItemService;
        }

        public async Task<Sale> CreateSaleAsync(List<LineItem> lineItems, decimal discount = 0)
        {
            var sale = new Sale
            {
                Id = Guid.NewGuid(),
                LineItems = lineItems,
                Discount = discount,
                CreatedAt = DateTime.UtcNow
            };
            lineItems = await _lineItemService.SetLineItemSaleId(lineItems, sale.Id);
            if (sale.Discount == 0)
            {
                _logger.LogInformation("No sale discount applied to sale {SaleId}", sale.Id);
            }
            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Added sale {SaleId} to DB", sale.Id);
            return sale;
        }
    }
}
