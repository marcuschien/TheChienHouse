using Microsoft.EntityFrameworkCore;
using TheChienHouse.Models;
using static LineItemDTO;

namespace TheChienHouse.Services
{
    public class LineItemService : ILineItemService
    {
        private readonly RetailContext _context;
        private readonly ILogger<LineItemService> _logger;
        private readonly IMenuItemService _menuItemService;

        public LineItemService(RetailContext context, ILogger<LineItemService> logger, IMenuItemService menuItemService)
        {
            _context = context;
            _logger = logger;
            _menuItemService = menuItemService;
        }

        public async Task<LineItem> CreateLineItemAsync(LineItemCreateRequest lineItemRequest)
        {
            LineItem lineItem = new LineItem
            {
                Id = Guid.NewGuid(),
                MenuItemForSale = lineItemRequest.MenuItemForSale,
                Quantity = lineItemRequest.Quantity,
                Discount = lineItemRequest.Discount,
                CreatedAt = DateTime.UtcNow
            };
            _context.LineItems.Add(lineItem);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Created line item {LineItem}", lineItem);
            return lineItem;
        }

        public async Task<List<LineItem>> CreateLineItemsAsync(List<LineItemCreateRequest> lineItemRequests)
        {
            List<LineItem> lineItems = new List<LineItem>();
            foreach (var request in lineItemRequests)
            {
                var lineItem = new LineItem
                {
                    Id = Guid.NewGuid(),
                    MenuItemForSale = request.MenuItemForSale,
                    Quantity = request.Quantity,
                    Discount = request.Discount,
                    CreatedAt = DateTime.UtcNow
                };
                await SetPricePerItem(lineItem);
                lineItems.Add(lineItem);
            }
            _logger.LogInformation("Created line items {LineItems}", lineItems);

            return lineItems;
        }

        public async Task<List<LineItem>> SetLineItemSaleId(List<LineItem> lineItems, Guid saleId)
        {
            foreach (var lineItem in lineItems)
            {
                lineItem.SaleId = saleId;
            }
            _context.LineItems.AddRange(lineItems);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Added line items with SaleId {SaleId} to DB", saleId);
            return lineItems;
        }

        public async Task<LineItem?> SetPricePerItem(LineItem lineItem)
        {
            var menuItem = await _menuItemService.GetMenuItemByIdAsync(lineItem.MenuItemForSale);
            if (menuItem == null)
            {
                _logger.LogWarning("Menu item with ID {MenuItemId} not found for line item {LineItemId}", lineItem.MenuItemForSale, lineItem.Id);
                return null;
            }
            lineItem.PricePerItem = menuItem.Price;
            return lineItem;
        }
    }
}
