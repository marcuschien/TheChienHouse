using Microsoft.AspNetCore.Mvc;
using Moq;
using TheChienHouse.Controllers;
using TheChienHouse.Services;
using TheChienHouse.Models;
using Xunit;

namespace TheChienHouse.Tests.Controllers
{
    public class CateringFormControllerTests:ControllerBase
    {
        private readonly Mock<ICateringFormService> _mockCateringService;
        private readonly CateringFormsController _cateringController;

        public CateringFormControllerTests()
        {
            _mockCateringService = new Mock<ICateringFormService>();
            _cateringController = new CateringFormsController(_mockCateringService.Object);
        }

        [Fact]
        public async Task GetCateringFormByClientId_Success()
        {
            // Arrange
            var clientId = Guid.NewGuid(); // Can I make this global?
            var cateringForms = new List<CateringForm>
            {
                new CateringForm { Id = Guid.NewGuid(), CateringType = CateringType.Delivery, DietaryRestrictions = [DietaryRestrictions.None], EventDate=new DateTime(), ClientName = "Kennedy", ClientId = clientId, Status = Status.Pending },
                new CateringForm { Id = Guid.NewGuid(), CateringType = CateringType.Delivery, DietaryRestrictions = [DietaryRestrictions.None], EventDate=new DateTime(), ClientName = "Kennedy", ClientId = clientId, Status = Status.Confirmed }
            };
            _mockCateringService.Setup(service => service.GetCateringFormsByClientIdAsync(clientId))
                .ReturnsAsync(cateringForms);
            // Act
            var result = await _cateringController.GetCateringFormsByClientId(clientId);
            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<CateringForm>>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<CateringForm>>(createdAtActionResult.Value);
            Assert.Equal(2, returnValue.Count());
            Assert.Equal(cateringForms, returnValue);
        }

        ///Tests to Write:
        ///GET CateringForm by Id 
        ///     - Missing clientId/empty
        ///     - Invalid clientId i.e. dne
        ///     - No CateringForms found
        ///GET CateringForms by Date Range
        ///     - Success on valid date range
        ///     - Missing startDate/endDate
        ///     - Invalid date range (startDate > endDate)
        ///     - Large date range (e.g., multiple years)
        ///     - No CateringForms found
        ///GET CateringForms by Status
        ///     - Success for each Status Type
        ///     - Invalid status value
        ///     - No CateringForms found
        ///DELETE CateringForm
        ///     - Invalid CateringForm Id
        ///     - Successful deletion
        ///PUT Update CateringForm
        ///     - Successful update
        ///     - Invalid CateringForm Id
        ///     - Invalid update data (e.g., invalid status transition, past event date)
        ///     - Trivial update (no actual changes)
        ///POST Create CateringForm
        ///     - Successful creation
        ///     - Missing required fields
        ///     - Invalid data (e.g., past event date, invalid enum values)
        ///     - Duplicate submission (same clientId and event date)
        ///     - Boundary testing (e.g., maximum length for ClientName)

    }
}
