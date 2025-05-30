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
        private readonly RetailContext _context; // Using in memory database
        private readonly IMenuItemService _menuItemService; // Utilizing the service layer to fetch requested data

        public MenuItemsController(RetailContext context, IMenuItemService menuItemService)
        {
            _context = context;
            _menuItemService = menuItemService;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MenuItem>>> GetMenuItems()
        {
            IEnumerable<MenuItemResponse> response = await _menuItemService.GetMenuItemsAsync();
            return CreatedAtAction(nameof(GetMenuItems), response); // Response could possibly be null, how would we handle a product being null? 
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MenuItem>> GetMenuItem(long id)
        {
            MenuItemResponse? response = await _menuItemService.GetMenuItemByIdAsync(id);
            return CreatedAtAction(nameof(GetMenuItem),response); // Response could possibly be null, need to handle this case. 
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MenuItem>> PostMenuItem(MenuItemCreateRequest request)
        {
            MenuItemResponse response = await _menuItemService.CreateMenuItemAsync(request);
            return CreatedAtAction(nameof(PostMenuItem), new { id = response.Id }, response);
        }
    }
}
