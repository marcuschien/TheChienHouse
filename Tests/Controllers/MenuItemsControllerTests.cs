using Microsoft.AspNetCore.Mvc;
using Moq;
using TheChienHouse.Controllers;
using TheChienHouse.Models;
using TheChienHouse.Services;
using Xunit;

namespace TheChienHouse.Tests.Controllers
{
    public class MenuItemsControllerTests
    {
        private readonly Mock<IMenuItemService> _mockMenuItemService;
        private readonly MenuItemsController _controller;
        public MenuItemsControllerTests()
        {
            _mockMenuItemService = new Mock<IMenuItemService>();
            _controller = new MenuItemsController(_mockMenuItemService.Object);
        }
        private readonly MenuItem _testItem = new MenuItem
        {
            Id = Guid.NewGuid(),
            Name = "Test Dish",
            Price = 9.99M,
            DishType = DishType.Main
        };

        [Fact]
        public async Task GetMenuItemById_Success()
        {
            // Arrange
            _mockMenuItemService.Setup(service => service.GetMenuItemByIdAsync(_testItem.Id))
                .ReturnsAsync(_testItem);
            // Act
            var result = await _controller.GetMenuItem(_testItem.Id);
            // Assert
            var actionResult = Assert.IsType<ActionResult<MenuItem>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var returnValue = Assert.IsType<MenuItem>(createdAtActionResult.Value);
            Assert.Equal(_testItem, returnValue);
        }

        [Fact]
        public async Task GetMenuItemById_NotFound()
        {
            _mockMenuItemService.Setup(service => service.GetMenuItemByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((MenuItem?)null);

            var result = await _controller.GetMenuItem(Guid.NewGuid());

            var actionResult = Assert.IsType<ActionResult<MenuItem>>(result);
            var notFoundResult = Assert.IsType<NotFoundResult>(actionResult.Result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task GetMenuItems_Success()
        {

        }

        [Fact]
        public async Task GetMenuItems_ByName_Success()
        {

        }

        [Fact]
        public async Task GetMenuItems_ByType_Success()
        {

        }

        [Fact]
        public async Task DeleteMenuItem_Success()
        {

        }

        [Fact]
        public async Task PostMenuItem_Success()
        {

        }

    }
}
