﻿using static TheChienHouse.Models.MenuItemDTO;

namespace TheChienHouse.Services
{
    public interface IMenuItemService
    {
        Task<IEnumerable<MenuItemResponse>> GetMenuItemsAsync(); // TODO: Introduce parameters int page and int pagesize to allow for pagination
        Task<MenuItemResponse> CreateMenuItemAsync(MenuItemCreateRequest request);
        Task<MenuItemResponse?> GetMenuItemByIdAsync(long id);
    }
}
