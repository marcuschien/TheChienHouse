using Microsoft.AspNetCore.Mvc;
using Moq;
using TheChienHouse.Controllers;
using TheChienHouse.Models;
using TheChienHouse.Services;
using Xunit;
using static LineItemDTO;
using static TheChienHouse.Models.SaleDTO;

namespace TheChienHouse.Tests.Controllers
{
    public class SalesControllerTests
    {
        private readonly Mock<ISaleService> _mockSaleService;
        private readonly Mock<ILineItemService> _mockLineItemService;
        private readonly SalesController _controller;
        private readonly Sale _testSale;
        private readonly List<LineItem> _testLineItems;
        private readonly List<Sale> _testSales;
        private readonly List<LineItemCreateRequest> _testLineItemsRequest;
        public SalesControllerTests()
        {
            //Create Test Components
            _mockSaleService = new Mock<ISaleService>();
            _mockLineItemService = new Mock<ILineItemService>();
            _controller = new SalesController(_mockSaleService.Object, _mockLineItemService.Object);
            //Create Test Data
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
            _testLineItemsRequest = new List<LineItemCreateRequest>
            {
                new LineItemCreateRequest(Guid.NewGuid(),2,0),
                new LineItemCreateRequest(Guid.NewGuid(),1,0)
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

        [Fact]
        public async Task GetSales_Success()
        {
            // Arrange
            _mockSaleService.Setup(service => service.GetAllSalesAsync())
                .ReturnsAsync(_testSales);
            // Act
            var result = await _controller.GetSales();
            // Assert
            var actionResult = Assert.IsType<ActionResult<List<SaleResponse>>>(result);
            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<List<SaleResponse>>(okObjectResult.Value);
            for (int i = 0; i < _testSales.Count; i++)
            {
                Assert.Equal(_testSales[i].Id, returnValue[i].Id);
            }
        }

        [Fact]
        public async Task CreateSale_Success()
        {
            // Arrange
            var testCreateRequest = new SaleRequest(_testLineItemsRequest, _testSale.Discount);
            _mockLineItemService.Setup(s => s.SetLineItemSaleId(It.IsAny<List<LineItem>>(), It.IsAny<Guid>()))
                .ReturnsAsync((List<LineItem> items, Guid saleId) =>
                {
                    foreach (var item in items)
                    {
                        item.SaleId = saleId;
                    }
                    return items;
                });
            _mockSaleService.Setup(service => service.CreateSaleAsync(It.IsAny<List<LineItem>>(), 0))
                .ReturnsAsync(_testSale);
            // Act
            var result = await _controller.PostSale(testCreateRequest);
            // Assert
            var actionResult = Assert.IsType<ActionResult<SaleResponse>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var returnValue = Assert.IsType<SaleResponse>(createdAtActionResult.Value);
            Assert.Equal(_testSale.Id, returnValue.Id);
        }
    }
}
