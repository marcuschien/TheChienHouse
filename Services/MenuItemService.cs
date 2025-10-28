using TheChienHouse.Models;
using Microsoft.EntityFrameworkCore;
using static TheChienHouse.Models.MenuItemDTO;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TheChienHouse.Services
{
    public class MenuItemService : IMenuItemService
    {
        private readonly RetailContext _context;
        private readonly ILogger<MenuItemService> _logger; //Used for tracing, helpful in prod issues
        public MenuItemService(RetailContext context, ILogger<MenuItemService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<MenuItem> CreateMenuItemAsync(MenuItemCreateRequest request)
        {
            var menuItem = new MenuItem
            {
                Id = Guid.NewGuid(), // Generate a new unique ID for the menu item
                Name = request.Name,
                Price = request.Price,
                DishType = request.DishType,
            };

            _context.MenuItems.Add(menuItem);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Created menu item {MenuItemName}", menuItem.Name);

            return menuItem;
        }

        //TODO: Create a PUT route to update a dish given ID or Name or DishType. (Might want to update the price of all apps by $1 or something)
        
        public async Task<IEnumerable<MenuItem>> GetMenuItemsAsync(string? name = null, DishType? dishType = null) // TODO: Introduce parameters int page and int pagesize to allow for pagination. Why do I want this to be async?
        {
            IEnumerable<MenuItem> menuItems = await _context.MenuItems.ToListAsync(); //Optimization: If name or dishType is provided, filter at the database level instead of in-memory. 
            if (name != null)
            {
                menuItems = menuItems.Where(mi => mi.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            }
            if (dishType != null)
            {
                menuItems = menuItems.Where(mi => mi.DishType == dishType);
            }
            return menuItems;
        }

        public async Task<MenuItem?> GetMenuItemByIdAsync(Guid id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);

            if (menuItem == null)
            {
                _logger.LogError("Menu Item {MenuItemId} not found", id);
                return null; //Add some exception here. Response should throw a different error code, depending on what we want the response to be. 
            }

            return menuItem;
        }

        public async Task<IEnumerable<MenuItem>> UpdateMenuItemsAsync(MenuItemUpdateRequest request)
        {
            // Query all menu items with the specified old name and old dish type
            var menuItems = await _context.MenuItems
                .Where(mi => mi.Name == request.OldName && mi.DishType == request.OldType)
                .ToListAsync();

            if (menuItems == null || menuItems.Count == 0)
            {
                _logger.LogError("Menu Items with Name {MenuItemName} and DishType {DishType} not found", request.OldName, request.OldType);
                throw new KeyNotFoundException($"Menu Items with Name {request.OldName} and Dish Type {request.OldType} not found.");
            }

            // Update all matching menu items
            foreach (var menuItem in menuItems)
            {
                if (!string.IsNullOrEmpty(request.NewName))
                {
                    menuItem.Name = request.NewName;
                }
                if (request.NewPrice.HasValue)
                {
                    menuItem.Price = request.NewPrice.Value;
                }
                if (request.NewType.HasValue)
                {
                    menuItem.DishType = request.NewType.Value;
                }
                menuItem.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Updated {Count} menu items with Name {MenuItemName} and DishType {DishType}", menuItems.Count, request.OldName, request.OldType);
            return menuItems;
        }
    }
}
