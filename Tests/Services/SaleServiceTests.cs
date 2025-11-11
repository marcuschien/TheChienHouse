using Microsoft.EntityFrameworkCore;
using Moq;
using TheChienHouse.Models;
using TheChienHouse.Services;
using Xunit;

namespace TheChienHouse.Tests.Services
{
    public class SaleServiceTests : IDisposable
    {
        private readonly RetailContext _context;
        private readonly Mock<ILogger<SaleService>> _mockLogger;
        private readonly SaleService _saleService;
        private readonly Mock<ILineItemService> _lineItemService;
        private readonly List<LineItem> _testLineItems;
        private readonly Sale _testSale;
        private readonly List<Sale> _testSales;
        public SaleServiceTests() 
        {
            //Create Test DB and mock components
            var options = new DbContextOptionsBuilder<RetailContext>()
                .UseInMemoryDatabase(databaseName: $"SaleTestDb_{Guid.NewGuid()}")
                .Options;
            _context = new RetailContext(options);
            _mockLogger = new Mock<ILogger<SaleService>>();
            _lineItemService = new Mock<ILineItemService>();
            _testLineItems = new List<LineItem>
            {
                new LineItem
                {
                    Id = Guid.NewGuid(),
                    MenuItemForSale = Guid.NewGuid(),
                    Quantity = 2,
                    Discount = 0,
                    CreatedAt = DateTime.UtcNow
                },
                new LineItem
                {
                    Id = Guid.NewGuid(),
                    MenuItemForSale = Guid.NewGuid(),
                    Quantity = 1,
                    Discount = 0,
                    CreatedAt = DateTime.UtcNow
                }
            };
            _saleService = new SaleService(_context, _mockLogger.Object, _lineItemService.Object);
            _testSale = new Sale
            {
                Id = Guid.NewGuid(),
                LineItems = new List<LineItem>
                {
                    new LineItem
                    {
                        Id = Guid.NewGuid(),
                        MenuItemForSale = Guid.NewGuid(),
                        Quantity = 2,
                        Discount = 0,
                        CreatedAt = DateTime.UtcNow
                    },
                    new LineItem
                    {
                        Id = Guid.NewGuid(),
                        MenuItemForSale = Guid.NewGuid(),
                        Quantity = 1,
                        Discount = 0,
                        CreatedAt = DateTime.UtcNow
                    }
                },
                Discount = 0,
                CreatedAt = DateTime.UtcNow
            };
            _testSales = new List<Sale>
            {
                new Sale
                {
                    Id = Guid.NewGuid(),
                    LineItems = _testLineItems,
                    Discount = 0,
                    CreatedAt = DateTime.UtcNow
                },
                new Sale
                {
                    Id = Guid.NewGuid(),
                    LineItems = _testLineItems,
                    Discount = 5,
                    CreatedAt = DateTime.UtcNow
                },
                new Sale
                {
                    Id = Guid.NewGuid(),
                    LineItems = _testLineItems,
                    Discount = 10,
                    CreatedAt = DateTime.UtcNow
                },
                new Sale
                {
                    Id = Guid.NewGuid(),
                    LineItems = _testLineItems,
                    Discount = 0,
                    CreatedAt = DateTime.UtcNow
                }
            };
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task CreateSale_Success()
        {
            _lineItemService.Setup(s => s.SetLineItemSaleId(It.IsAny<List<LineItem>>(), It.IsAny<Guid>()))
                .ReturnsAsync((List<LineItem> items, Guid saleId) =>
                {
                    foreach (var item in items)
                    {
                        item.SaleId = saleId;
                    }
                    return items;
                });
            var result = await _saleService.CreateSaleAsync(_testLineItems, 0);
            Assert.NotNull(result);
            Assert.True(VerifyLineItems(_testLineItems, result.LineItems));
            Assert.Equal(0, result.Discount);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("No sale discount applied to sale")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Added sale")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetAllSales_Success()
        {
            //Seed the DB
            _context.Sales.AddRange(_testSales);
            await _context.SaveChangesAsync();
            var result = await _saleService.GetAllSalesAsync();
            Assert.NotNull(result);
            Assert.True(VerifySales(_testSales, result));

        }

        public bool VerifySales(List<Sale> expected, List<Sale> result)
        {
            if (expected.Count != result.Count)
                return false;
            for (int i = 0; i < expected.Count; i++)
            {
                if (expected[i].Id != result[i].Id ||
                    expected[i].Discount != result[i].Discount ||
                    !VerifyLineItems(expected[i].LineItems, result[i].LineItems))
                {
                    return false;
                }
            }
            return true;
        }

        public bool VerifyLineItems(List<LineItem> expected, List<LineItem> result)
        {
            if (expected.Count != result.Count)
                return false;
            for (int i = 0; i < expected.Count; i++)
            {
                if (expected[i].Id != result[i].Id ||
                    expected[i].MenuItemForSale != result[i].MenuItemForSale ||
                    expected[i].Quantity != result[i].Quantity ||
                    expected[i].Discount != result[i].Discount)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
