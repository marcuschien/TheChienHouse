using TheChienHouse.Models;
using static TheChienHouse.Models.MenuItemDTO;

namespace TheChienHouse.Services
{
    public interface IMenuItemService
    {
        Task<IEnumerable<MenuItem>> GetMenuItemsAsync(string? dishName = null, DishType? dishType = null); // TODO: Introduce parameters int page and int pagesize to allow for pagination
        Task<MenuItem> CreateMenuItemAsync(MenuItemCreateRequest request);
        Task<MenuItem?> GetMenuItemByIdAsync(Guid id);
        Task<IEnumerable<MenuItem>> UpdateMenuItemsAsync(MenuItemUpdateRequest request);
    }
}
