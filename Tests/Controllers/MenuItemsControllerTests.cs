using Microsoft.AspNetCore.Mvc;
using Moq;
using TheChienHouse.Controllers;
using TheChienHouse.Models;
using TheChienHouse.Services;
using Xunit;
using static TheChienHouse.Models.MenuItemDTO;

namespace TheChienHouse.Tests.Controllers
{
    public class MenuItemsControllerTests
    {
        private readonly Mock<IMenuItemService> _mockMenuItemService;
        private readonly MenuItemsController _controller;
        private readonly MenuItem _testItem;
        private readonly List<MenuItem> _testItems;
        public MenuItemsControllerTests()
        {
            //Create Test Components
            _mockMenuItemService = new Mock<IMenuItemService>();
            _controller = new MenuItemsController(_mockMenuItemService.Object);

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
                new MenuItem { Id = Guid.NewGuid(), Name = "Appetizer 1", Price = 5.99M, DishType = DishType.Appetizer, CreatedAt = DateTime.UtcNow },
                new MenuItem { Id = Guid.NewGuid(), Name = "Main Course 1", Price = 15.99M, DishType = DishType.Main, CreatedAt = DateTime.UtcNow },
                new MenuItem { Id = Guid.NewGuid(), Name = "Main Course 2", Price = 18.99M, DishType = DishType.Main, CreatedAt = DateTime.UtcNow },
                new MenuItem { Id = Guid.NewGuid(), Name = "Dessert 1", Price = 7.99M, DishType = DishType.Dessert, CreatedAt = DateTime.UtcNow }
            };
        }

        [Fact]
        public async Task GetMenuItemById_Success()
        {
            // Arrange
            _mockMenuItemService.Setup(service => service.GetMenuItemByIdAsync(_testItem.Id))
                .ReturnsAsync(_testItem);

            // Act
            var result = await _controller.GetMenuItem(new MenuItemRequest(_testItem.Id));

            // Assert
            var actionResult = Assert.IsType<ActionResult<MenuItemResponse>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var returnValue = Assert.IsType<MenuItemResponse>(createdAtActionResult.Value);
            Assert.Equal(_testItem.Id, returnValue.Id);
            Assert.Equal(_testItem.Name, returnValue.Name);
            Assert.Equal(_testItem.Price, returnValue.Price);
            Assert.Equal(_testItem.DishType, returnValue.DishType);
        }

        [Fact]
        public async Task GetMenuItemById_NotFound()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();
            _mockMenuItemService.Setup(service => service.GetMenuItemByIdAsync(nonExistentId))
                .ReturnsAsync((MenuItem?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _controller.GetMenuItem(new MenuItemRequest(nonExistentId)));

            Assert.Equal("Menu Item not found.", exception.Message);
        }

        [Fact]
        public async Task GetMenuItems_Success()
        {
            // Arrange
            _mockMenuItemService.Setup(service => service.GetMenuItemsAsync(null, null))
                .ReturnsAsync(_testItems);

            // Act
            var result = await _controller.GetMenuItems(new GetMenuItemsRequest());

            // Assert
            var actionResult = Assert.IsType<ActionResult<MenuItemsResponse>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var returnValue = Assert.IsType<MenuItemsResponse>(createdAtActionResult.Value);
            Assert.Equal(4, returnValue.MenuItems.Count());
        }

        [Fact]
        public async Task GetMenuItems_ByName_Success()
        {
            // Arrange
            var expectedItems = _testItems.Where(i => i.Name == "Main Course 1").ToList();
            _mockMenuItemService.Setup(service => service.GetMenuItemsAsync("Main Course 1", null))
                .ReturnsAsync(expectedItems);

            // Act
            var result = await _controller.GetMenuItems(new GetMenuItemsRequest("Main Course 1", null));

            // Assert
            var actionResult = Assert.IsType<ActionResult<MenuItemsResponse>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var returnValue = Assert.IsType<MenuItemsResponse>(createdAtActionResult.Value);
            Assert.Single(returnValue.MenuItems);
            Assert.Equal("Main Course 1", returnValue.MenuItems.First().Name);
        }

        [Fact]
        public async Task GetMenuItems_ByType_Success()
        {
            // Arrange
            var expectedItems = _testItems.Where(i => i.DishType == DishType.Main).ToList();
            _mockMenuItemService.Setup(service => service.GetMenuItemsAsync(null, DishType.Main))
                .ReturnsAsync(expectedItems);

            // Act
            var result = await _controller.GetMenuItems(new GetMenuItemsRequest(null, DishType.Main));

            // Assert
            var actionResult = Assert.IsType<ActionResult<MenuItemsResponse>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var returnValue = Assert.IsType<MenuItemsResponse>(createdAtActionResult.Value);
            Assert.Equal(2, returnValue.MenuItems.Count());
            Assert.All(returnValue.MenuItems, item => Assert.Equal(DishType.Main, item.DishType));
        }

        [Fact]
        public async Task PostMenuItem_Success()
        {
            // Arrange
            var createRequest = new CreateMenuItemRequest("New Dish", 12.99M, DishType.Appetizer);
            var createdItem = new MenuItem
            {
                Id = Guid.NewGuid(),
                Name = createRequest.Name,
                Price = createRequest.Price,
                DishType = createRequest.DishType,
                CreatedAt = DateTime.UtcNow
            };

            _mockMenuItemService.Setup(service => service.CreateMenuItemAsync(
                createRequest.Name,
                createRequest.Price,
                createRequest.DishType))
                .ReturnsAsync(createdItem);

            // Act
            var result = await _controller.PostMenuItem(createRequest);

            // Assert
            var actionResult = Assert.IsType<ActionResult<MenuItemResponse>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var returnValue = Assert.IsType<MenuItemResponse>(createdAtActionResult.Value);
            Assert.Equal(createdItem.Name, returnValue.Name);
            Assert.Equal(createdItem.Price, returnValue.Price);
            Assert.Equal(createdItem.DishType, returnValue.DishType);
        }

        [Fact]
        public async Task UpdateMenuItems_Success()
        {
            // Arrange
            var updateRequest = new UpdateMenuItemRequest("Old Name", DishType.Main, 20.00M, DishType.Main, "New Name");
            var updatedItems = new List<MenuItem>
            {
                new MenuItem { Id = Guid.NewGuid(), Name = "New Name", Price = 20.00M, DishType = DishType.Main, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            };

            _mockMenuItemService.Setup(service => service.UpdateMenuItemsAsync(
                updateRequest.OldName,
                updateRequest.OldType,
                updateRequest.NewPrice,
                updateRequest.NewType,
                updateRequest.NewName))
                .ReturnsAsync(updatedItems);

            // Act
            var result = await _controller.UpdateMenuItems(updateRequest);

            // Assert
            var actionResult = Assert.IsType<ActionResult<MenuItemsResponse>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var returnValue = Assert.IsType<MenuItemsResponse>(createdAtActionResult.Value);
            Assert.Single(returnValue.MenuItems);
            Assert.Equal("New Name", returnValue.MenuItems.First().Name);
        }

        [Fact]
        public async Task DeleteMenuItem_Success()
        {
            // Arrange
            _mockMenuItemService.Setup(service => service.DeleteMenuItemAsync(_testItem.Id))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteMenuItem(new MenuItemRequest(_testItem.Id));

            // Assert
            var actionResult = Assert.IsType<ActionResult<bool>>(result);
            Assert.True(actionResult.Value);
        }

        [Fact]
        public async Task DeleteMenuItem_NotFound()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();
            _mockMenuItemService.Setup(service => service.DeleteMenuItemAsync(nonExistentId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteMenuItem(new MenuItemRequest(nonExistentId));

            // Assert
            var actionResult = Assert.IsType<ActionResult<bool>>(result);
            Assert.False(actionResult.Value);
        }
    }
}
