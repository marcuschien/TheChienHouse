using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheChienHouse.Models;
using TheChienHouse.Services;
using static TheChienHouse.Models.MenuItemDTO;

namespace TheChienHouse.Controllers
{
    [Route("api/[controller]")]
    [ApiController] // This attribute indicates that the controller responds to web API requests
    public class MenuItemsController : ControllerBase
    {
        private readonly IMenuItemService _menuItemService; // Utilizing the service layer to fetch requested data

        public MenuItemsController(IMenuItemService menuItemService)
        {
            _menuItemService = menuItemService;
        }

        // GET: api/MenuItems
        [HttpGet]
        public async Task<ActionResult<MenuItemsResponse>> GetMenuItems(string? name = null, DishType? dishType = null)
        {
            IEnumerable<MenuItem> menuItems = await _menuItemService.GetMenuItemsAsync(name, dishType);
            MenuItemsResponse response = new MenuItemsResponse(menuItems);
            return CreatedAtAction(nameof(GetMenuItems), response); // Response could possibly be null, how would we handle a product being null? 
        }

        // GET: api/MenuItems/{MenuItemId}
        [HttpGet("{id}")]
        public async Task<ActionResult<MenuItemResponse>> GetMenuItem(Guid id)
        {
            MenuItem? menuItem = await _menuItemService.GetMenuItemByIdAsync(id);
            MenuItemResponse response = MapToResponse(menuItem);
            return CreatedAtAction(nameof(GetMenuItem),response); // Response could possibly be null, need to handle this case. 
        }

        // POST: api/MenuItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MenuItemResponse>> PostMenuItem(MenuItemCreateRequest request)
        {
            MenuItem menuItem = await _menuItemService.CreateMenuItemAsync(request);
            MenuItemResponse response = MapToResponse(menuItem);
            return CreatedAtAction(nameof(PostMenuItem), new { id = response.Id }, response);
        }

        [HttpPut]
        public async Task<ActionResult<MenuItemsResponse>> UpdateMenuItems(MenuItemUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            IEnumerable<MenuItem> menuItems = await _menuItemService.UpdateMenuItemsAsync(request);
            MenuItemsResponse response = new MenuItemsResponse(menuItems);
            return CreatedAtAction(nameof(UpdateMenuItems), response); 
        }

        private static MenuItemResponse MapToResponse(MenuItem? item)
        {
            if(item != null)
            {
                return new MenuItemResponse(
                    item.Id,
                    item.Name,
                    item.Price,
                    item.DishType,
                    item.ExpiryDate,
                    item.CreatedAt,
                    item.UpdatedAt
                );
            }
            else
            {
                throw new KeyNotFoundException("Menu Item not found.");
            }
        }
    }
}
