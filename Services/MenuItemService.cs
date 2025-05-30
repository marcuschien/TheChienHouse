using TheChienHouse.Models;
using Microsoft.EntityFrameworkCore;
using static TheChienHouse.Models.MenuItemDTO;

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
                DishType = request.DishType
            };

            _context.MenuItems.Add(menuItem);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created menu item {MenuItemId}", menuItem.Id);

            return MapToResponse(menuItem);
        }
        
        public async Task<IEnumerable<MenuItemResponse>> GetMenuItemsAsync() // TODO: Introduce parameters int page and int pagesize to allow for pagination
        {
            // TODO: Put the returned answer into ProductResponse form to return to the controller
            List<MenuItem> menuItems = await _context.MenuItems.ToListAsync();
            List<MenuItemResponse> response = new List<MenuItemResponse>();
            foreach (MenuItem menuItem in menuItems)
            {
                response.Add(MapToResponse(menuItem));
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

        public async Task<IEnumerable<MenuItemResponse>> GetMenuItemsByDishTypeAsync(DishType dishType)
        {
            List<MenuItem> menuItems = await _context.MenuItems.ToListAsync();
            List<MenuItemResponse> response = new List<MenuItemResponse>();
            foreach (MenuItem menuItem in menuItems) //Is there a better way I can batch this or something?
            { // If I had a relational database I could just write a SQL query to get the items by category.
                if (menuItem.DishType != dishType) continue; // Filter by dish type 
                response.Add(MapToResponse(menuItem));
            }
            return response;
        }

        private static MenuItemResponse MapToResponse(MenuItem item) =>
            new(item.Id, item.Name, item.Price, item.DishType, item.CreatedAt, item.UpdatedAt);
        
        //private static MenuItemResponse MapToResponse(MenuItem[] items) =>
            //TODO: Handle the case where mutliple items are returned. Probably should do some kind of batching. 

    }
}
