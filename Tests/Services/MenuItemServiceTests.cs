using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using TheChienHouse.Models;
using TheChienHouse.Services;
using Xunit;

namespace TheChienHouse.Tests.Services
{
    public class MenuItemServiceTests : IDisposable
    {
        private readonly RetailContext _context;
        private readonly Mock<ILogger<MenuItemService>> _mockLogger;
        private readonly MenuItemService _menuItemService;
        private readonly MenuItem _testItem;
        private readonly List<MenuItem> _testItems;
        public MenuItemServiceTests()
        {
            //Create Test DB and mock components
            var options = new DbContextOptionsBuilder<RetailContext>()
                .UseInMemoryDatabase(databaseName: $"MenuItemTestDb_{Guid.NewGuid()}")
                .Options;
            _context = new RetailContext(options);
            _mockLogger = new Mock<ILogger<MenuItemService>>();
            _menuItemService = new MenuItemService(_context, _mockLogger.Object);

            //Create Test Data
            _testItem = new MenuItem
            {
                Id = Guid.NewGuid(),
                Name = "Test Dish",
                Price = 9.99M,
                DishType = DishType.Main,
                CreatedAt = DateTime.UtcNow
            };

            _testItems = new List<MenuItem>
            {
                new MenuItem { Id = Guid.NewGuid(), Name = "Test Appetizer 1", Price = 5.99M, DishType = DishType.Appetizer, CreatedAt = DateTime.UtcNow },
                new MenuItem { Id = Guid.NewGuid(), Name = "Test Main Course 1", Price = 15.99M, DishType = DishType.Main, CreatedAt = DateTime.UtcNow },
                new MenuItem { Id = Guid.NewGuid(), Name = "Test Main Course 2", Price = 18.99M, DishType = DishType.Main, CreatedAt = DateTime.UtcNow },
                new MenuItem { Id = Guid.NewGuid(), Name = "Test Dessert 1", Price = 7.99M, DishType = DishType.Dessert, CreatedAt = DateTime.UtcNow }


            };

            //Seed the DB
            _context.Add(_testItem);
            _context.AddRange(_testItems);
        }
        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task GetMenuItemById_Success()
        {
            var result = await _menuItemService.GetMenuItemByIdAsync(_testItem.Id);
            
            Assert.NotNull(result);
            Assert.Equal(_testItem.Id, result!.Id);
            Assert.Equal(_testItem.Name, result.Name);
        }
    }
}
