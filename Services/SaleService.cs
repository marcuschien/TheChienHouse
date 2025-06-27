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
        private readonly IMenuItemService _menuItemService;
        public SaleService(RetailContext context, ILogger<SaleService> logger, IMenuItemService menuItemService) 
        {
            _context = context;
            _logger = logger;
            _menuItemService = menuItemService;
        }

        //TODO: Update menu items context so that the sold items are removed from DB
        public async Task<SaleCreateResponse> CreateSaleAsync(SaleCreateRequest request)
        {
            //TODO: Validate that the items exist in DB first. 
            List<LineItem> saleLineItems = new List<LineItem>(); // Create a list to hold the line items for the sale
            foreach (LineItemCreateRequest lineItem in request.LineItems) // Validate the line items and add it to the sale
            {
                if (lineItem.Quantity <= 0)
                {
                    _logger.LogError("Could not create sale because line item {LineItem} has a quantity less than or equal to 0.", lineItem);
                    throw new ArgumentException("Line item quantity must be greater than 0.");
                }
                if (string.IsNullOrWhiteSpace(lineItem.MenuItemForSale))
                {
                    _logger.LogError("Could not create sale because line item {LineItem} has no menu item specified.", lineItem);
                    throw new ArgumentException("Line item must have a menu item specified.");
                }
                LineItem tempItem = new LineItem()
                {
                    MenuItemForSale = lineItem.MenuItemForSale,
                    Quantity = lineItem.Quantity
                };
                saleLineItems.Add(tempItem); // Add the line item to the list
            }
            // TODO: Calculate the sale total and apply the sale discount if applicable.
            var sale = new Sale
            {
                LineItems = saleLineItems,
                SaleDiscount = request.SaleDiscount
            };
            sale.Total = CalculateTotals(sale.LineItems); // TODO: Validate the totals are updated in DB
            if (sale.SaleDiscount != null)
            {
                sale.Total = CalculateDiscounts((decimal)sale.SaleDiscount, sale.LineItems, sale.Total); // TODO: Validate this is pass by reference and not value. i.e. line item discounts are updated and saved to the sale object.
            }
            else
            {
                _logger.LogInformation("No sale discount applied to sale {SaleId}", sale.Id); //Sale ID might be null at this point, may need ot change log message. 
            }
            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created sale {SaleId}", sale.Id);

            return MapToResponse(sale);
        }

        //Calculate the total cost of the sale, remove sold line items from inventory, and sanity check the sale. 
        public decimal CalculateTotals(List<LineItem> lineItems)
        {
            decimal saleTotal = 0;
            foreach (LineItem lineItem in lineItems) // Calculate the Line item totals and save the values to their respective line items
            {
                IEnumerable<MenuItemResponse> result = _menuItemService.GetMenuItemsAsync(lineItem.MenuItemForSale).Result;
                if(result == null)
                {
                    _logger.LogError("Could not create sale because menu item {MenuItemName} does not exist.", lineItem.MenuItemForSale);
                    return 0; // TODO: Throw a global exception here. Need to cancel/invalidate the sale. Currently still goes through. 
                }
                if (lineItem.Quantity > result.Count())
                {
                    _logger.LogError("Could not create sale because there are not enough menu items {MenuItemName} available.", lineItem.MenuItemForSale);
                    // TODO: Do we want to allow partial success and sell the remaining inventory?
                    return 0; // TODO: Throw a global exception here.
                }
                List<MenuItem> itemsToRemove = new List<MenuItem>();
                
                RemoveFromDB(itemsToRemove);
                // TODO: Add sanity check for totals.
                lineItem.TotalCost = Math.Round(result.First<MenuItemResponse>().Price * lineItem.Quantity, 2);
                saleTotal += lineItem.TotalCost;
            }
            return saleTotal;
        }

        public decimal CalculateDiscounts(decimal saleDiscount, List<LineItem> lineItems, decimal saleTotal)
        {
            foreach (LineItem lineItem in lineItems) // Update the line item totals with the discount applied
            {
                lineItem.LineItemDiscount = Math.Round((lineItem.TotalCost / saleTotal) * saleDiscount, 2);
                lineItem.TotalCost -= lineItem.LineItemDiscount;   
            }
            saleTotal -= saleDiscount;
            return saleTotal;
        }

        private bool RemoveFromDB(List<MenuItem> itemsToRemove)
        {
            /*
             * Need to implement this method
             * For now will need to use linq query to remove from DBSet.
             * Once I add an actual DB using Azure or something, will need to write a command that calls that query to remove from DB
             * 
            for (int i = 0; i < lineItem.Quantity; i++) // Remove the sold items from the inventory
            {
                var tempItem = from item in _context.MenuItems
                               where item.Name == lineItem.MenuItemForSale
                               orderby item.CreatedAt ascending // Get the earliest created item to remove
                               select item;
                //TODO: Figure out how to remove the earliest created MenuItem of the given name from the inventory. The sale should include the MenuItem ID 
            }*/
            if (itemsToRemove != null)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        private static SaleCreateResponse MapToResponse(Sale sale) =>
            new(sale.Id, sale.LineItems, sale.Total, sale.SaleDiscount, sale.CreatedAt, sale.UpdatedAt);
    }
}
