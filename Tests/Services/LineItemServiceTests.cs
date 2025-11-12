using Microsoft.EntityFrameworkCore;
using Moq;
using TheChienHouse.Models;
using TheChienHouse.Services;
using Xunit;

namespace TheChienHouse.Tests.Services
{
    public class LineItemServiceTests : IDisposable
    {
        private readonly RetailContext _context;
        private readonly ILineItemService _lineItemService;
        private readonly Mock<ILogger<LineItemService>> _mockLogger;
        private readonly Mock<IMenuItemService> _mockMenuItemService;
        private readonly Dictionary<Guid, Tuple<int, decimal>> _testItemsDict;
        private readonly List<LineItem> _testItems;
        

        public LineItemServiceTests()
        {
            var options = new DbContextOptionsBuilder<RetailContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new RetailContext(options);
            _mockLogger = new Mock<ILogger<LineItemService>>();
            _mockMenuItemService = new Mock<IMenuItemService>();
            _lineItemService = new LineItemService(_context, _mockLogger.Object, _mockMenuItemService.Object);
            _testItemsDict = new Dictionary<Guid, Tuple<int, decimal>>
            {
                { Guid.NewGuid(), Tuple.Create(2, 5.00M) },
                { Guid.NewGuid(), Tuple.Create(1, 0.00M) }
            };
            _testItems = new List<LineItem>
            {
                new LineItem { Id = Guid.NewGuid(), MenuItemForSale = Guid.NewGuid(), Quantity = 2, Discount = 0, CreatedAt = DateTime.UtcNow },
                new LineItem { Id = Guid.NewGuid(), MenuItemForSale = Guid.NewGuid(), Quantity = 1, Discount = 0, CreatedAt = DateTime.UtcNow }
            };
        }
        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task CreateLineItemAsync_SetsPropertiesCorrectly()
        {
            // Arrange
            var menuItemId = Guid.NewGuid();
            int quantity = 2;
            decimal discount = 5.00M;
            // Act
            var lineItem = await _lineItemService.CreateLineItemAsync(menuItemId, quantity, discount);
            // Assert
            Assert.NotNull(lineItem);
            Assert.Equal(menuItemId, lineItem.MenuItemForSale);
            Assert.Equal(quantity, lineItem.Quantity);
            Assert.Equal(discount, lineItem.Discount);
            Assert.True((DateTime.UtcNow - lineItem.CreatedAt).TotalSeconds < 5); // CreatedAt is recent
        }

        [Fact]
        public async Task CreateLineItemsAsync_CreatesMultipleLineItems()
        {
            // Act
            var lineItems = await _lineItemService.CreateLineItemsAsync(_testItemsDict);
            // Assert
            Assert.Equal(2, lineItems.Count);
            foreach (var lineItem in lineItems)
            {
                Assert.Contains(lineItem.MenuItemForSale, _testItemsDict.Keys);
                Assert.Equal(_testItemsDict[lineItem.MenuItemForSale].Item1, lineItem.Quantity);
                Assert.Equal(_testItemsDict[lineItem.MenuItemForSale].Item2, lineItem.Discount);
            }
        }

        [Fact]
        public async Task SetLineItemSaleId_SetsSaleIdForAllLineItems()
        {
            var saleId = Guid.NewGuid();
            // Act
            var updatedLineItems = await _lineItemService.SetLineItemSaleId(_testItems, saleId);
            // Assert
            foreach (var lineItem in updatedLineItems)
            {
                Assert.Equal(saleId, lineItem.SaleId);
            }
        }
    }
}
