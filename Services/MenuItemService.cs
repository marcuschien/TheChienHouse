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
        public async Task<MenuItemResponse> CreateMenuItemAsync(MenuItemCreateRequest request)
        {
            var menuItem = new MenuItem
            {
                Id = Guid.NewGuid().ToString(), // Generate a new unique ID for the menu item
                Name = request.Name,
                Price = request.Price,
                DishType = request.DishType,
            };

            _context.MenuItems.Add(menuItem);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Created menu item {MenuItemName}", menuItem.Name);

            return MapToResponse(menuItem);
        }

        //TODO: Create a PUT route to update a dish given ID or Name or DishType. (Might want to update the price of all apps by $1 or something)
        
        public async Task<IEnumerable<MenuItemResponse>> GetMenuItemsAsync(string? name = null, string? dishType = null) // TODO: Introduce parameters int page and int pagesize to allow for pagination. Why do I want this to be async?
        {
            List<MenuItem> menuItems = await _context.MenuItems.ToListAsync(); //Optimization: If name or dishType is provided, filter at the database level instead of in-memory. 
            Dictionary<string, MenuItem[]> dishes = new Dictionary<string, MenuItem[]>();
            List<MenuItemResponse> response = new List<MenuItemResponse>();
            foreach (MenuItem menuItem in menuItems)
            {
                if (dishes.ContainsKey(menuItem.Name))
                {
                    dishes[menuItem.Name] = dishes[menuItem.Name].Append(menuItem).ToArray();
                }
                else
                {
                    if (name != null)
                    {
                        if (menuItem.Name == name)
                        {
                            dishes.Add(menuItem.Name, [menuItem]);
                        }
                        else continue;
                    }
                    else if (dishType != null)
                    {
                        if (menuItem.DishType == dishType)
                        {
                            dishes.Add(menuItem.Name, [menuItem]);
                        }
                        else continue;
                    }
                    else
                    {
                        dishes.Add(menuItem.Name, [menuItem]);
                    }
                }
            }
            foreach (MenuItem[] dish in dishes.Values) 
            {
                MenuItem firstItem = dish.First();
                response.Add(new MenuItemResponse(firstItem.Id, firstItem.Name, firstItem.Price, firstItem.DishType, firstItem.CreatedAt, firstItem.UpdatedAt, dish.Length));
            }
            return response;
        }

        public async Task<MenuItemResponse?> GetMenuItemByIdAsync(long id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);

            if (menuItem == null)
            {
                _logger.LogError("Menu Item {MenuItemId} not found", id);
                return null; //Add some exception here. Response should throw a different error code, depending on what we want the response to be. 
            }

            return MapToResponse(menuItem);
        }

        public async Task<MenuItemResponse> UpdateMenuItemAsync(MenuItemUpdateRequest request)
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
                if (!string.IsNullOrWhiteSpace(request.NewType))
                {
                    menuItem.DishType = request.NewType;
                }
                menuItem.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            // Return the first updated item as a response (or adjust as needed)
            return MapToResponse(menuItems.First());
        }

        private static MenuItemResponse MapToResponse(MenuItem item) =>
            new(item.Id, item.Name, item.Price, item.DishType, item.CreatedAt, item.UpdatedAt);
        
        //private static MenuItemResponse MapToResponse(MenuItem[] items) =>
            //TODO: Handle the case where mutliple items are returned. Probably should do some kind of batching. 

    }
}
