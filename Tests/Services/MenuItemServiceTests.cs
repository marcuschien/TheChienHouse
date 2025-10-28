using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using TheChienHouse.Models;
using TheChienHouse.Services;
using Xunit;

namespace TheChienHouse.Tests.Services
{
    public class MenuItemServiceTests
    {
        private readonly RetailContext _context;
        private readonly Mock<ILogger<MenuItemService>> _mockLogger;
        private readonly MenuItemService _menuItemService;
        private readonly MenuItem _testItem = new MenuItem
        {
            Id = Guid.NewGuid(),
            Name = "Test Dish",
            Price = 9.99M,
            DishType = DishType.Main
        };
        public MenuItemServiceTests()
        {
            var options = new DbContextOptionsBuilder<RetailContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new RetailContext(options);
            _mockLogger = new Mock<ILogger<MenuItemService>>();
            _menuItemService = new MenuItemService(_context, _mockLogger.Object);
            
        }


        [Fact]
        public async Task GetMenuItemById_Success()
        {
            //Arrange
            _context.MenuItems.Add(_testItem);
            await _context.SaveChangesAsync();
            // Act
            var result = await _menuItemService.GetMenuItemByIdAsync(_testItem.Id);
            // Assert
            Assert.NotNull(result);
            Assert.Equal(_testItem.Id, result!.Id);
            Assert.Equal(_testItem.Name, result.Name);
        }
    }
}
