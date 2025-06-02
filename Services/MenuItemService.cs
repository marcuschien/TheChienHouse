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
        
        public async Task<IEnumerable<MenuItemResponse>> GetMenuItemsAsync(DishName? name = null, DishType? dishType = null) // TODO: Introduce parameters int page and int pagesize to allow for pagination
        {
            List<MenuItem> menuItems = await _context.MenuItems.ToListAsync();
            Dictionary<DishName, MenuItem[]> dishes = new Dictionary<DishName, MenuItem[]>();
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

        private static MenuItemResponse MapToResponse(MenuItem item) =>
            new(item.Id, item.Name, item.Price, item.DishType, item.CreatedAt, item.UpdatedAt);
        
        //private static MenuItemResponse MapToResponse(MenuItem[] items) =>
            //TODO: Handle the case where mutliple items are returned. Probably should do some kind of batching. 

    }
}
