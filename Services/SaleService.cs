﻿using TheChienHouse.Models;
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

        public async Task<SaleCreateResponse> CreateSaleAsync(SaleCreateRequest request)
        {
            var sale = new Sale
            {
                LineItems = request.LineItems,
                SaleDiscount = request.SaleDiscount
            };
            sale.Total = CalculateTotals(request.LineItems); // Validate this is pass by reference so that the line item totals are updated in DB
            if (sale.SaleDiscount != null)
            {
                sale.Total = CalculateDiscounts((decimal)sale.SaleDiscount, request.LineItems, sale.Total); // Validate this is pass by reference and not value. i.e. line item discounts are updated and saved to the sale object.
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

        public decimal CalculateTotals(List<LineItem> lineItems)
        {
            decimal saleTotal = 0;
            foreach (LineItem lineItem in lineItems) // Calculate the Line item totals and save the values to their respective line items
            {
                MenuItemResponse? result = _menuItemService.GetMenuItemByIdAsync(lineItem.menuItemId).Result;
                if(result == null)
                {
                    _logger.LogError("Could not create sale because menu item {MenuItemId} does not exist.", lineItem.menuItemId);
                    return 0; // Throw a global exception here.
                }
                // Add sanity check for totals.
                lineItem.TotalCost = Math.Round(result.Price * lineItem.Quantity, 2);
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

        private static SaleCreateResponse MapToResponse(Sale sale) =>
            new(sale.Id, sale.LineItems, sale.Total, sale.SaleDiscount, sale.CreatedAt, sale.UpdatedAt);
    }
}
