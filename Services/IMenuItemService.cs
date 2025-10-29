using TheChienHouse.Models;
using static TheChienHouse.Models.MenuItemDTO;

namespace TheChienHouse.Services
{
    public interface IMenuItemService
    {
        Task<IEnumerable<MenuItem>> GetMenuItemsAsync(string? dishName = null, DishType? dishType = null); // TODO: Introduce parameters int page and int pagesize to allow for pagination
        Task<MenuItem> CreateMenuItemAsync(string name, decimal price, DishType dishType);
        Task<MenuItem?> GetMenuItemByIdAsync(Guid id);
        Task<IEnumerable<MenuItem>> UpdateMenuItemsAsync(string oldName, DishType oldType, decimal? newPrice = null, DishType? newType = null, string? newName = null);
        Task<bool> DeleteMenuItemAsync(Guid id);
    }
}
