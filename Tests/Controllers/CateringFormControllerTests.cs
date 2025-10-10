using System.Reflection;
using System.Runtime.Intrinsics.X86;
using System.Runtime.Serialization;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TheChienHouse.Controllers;
using TheChienHouse.Models;
using TheChienHouse.Services;
using Xunit;
using static System.Net.Mime.MediaTypeNames;
using static TheChienHouse.Models.CateringFormDTO;

namespace TheChienHouse.Tests.Controllers
{
    public class CateringFormControllerTests
    {
        private readonly Mock<ICateringFormService> _mockCateringService;
        private readonly CateringFormsController _cateringController;
        //TODO: Eventually I should migrate these to a test database seeding strategy.
        private static readonly Guid _testClientId = Guid.NewGuid();
        private static readonly CateringForm _testCateringForm = new CateringForm
        {
            Id = Guid.NewGuid(),
            CateringType = CateringType.Delivery,
            DietaryRestrictions = [DietaryRestrictions.None],
            EventDate = new DateTime(2025, 10, 9),
            ClientName = "Kennedy",
            ClientId = Guid.NewGuid(),
            Status = Status.Pending
        };
        private static readonly List<CateringForm> _testCateringForms = new List<CateringForm>
        {
            new CateringForm { Id = Guid.NewGuid(), CateringType = CateringType.Delivery, DietaryRestrictions = [DietaryRestrictions.None], EventDate=new DateTime(2025, 10, 8), ClientName = "Kennedy", ClientId = _testClientId, Status = Status.Confirmed },
            new CateringForm { Id = Guid.NewGuid(), CateringType = CateringType.Delivery, DietaryRestrictions = [DietaryRestrictions.None], EventDate=new DateTime(2025, 10, 8), ClientName = "Kennedy", ClientId = _testClientId, Status = Status.Pending },
            new CateringForm { Id = Guid.NewGuid(), CateringType = CateringType.Delivery, DietaryRestrictions = [DietaryRestrictions.None], EventDate=new DateTime(2025, 10, 6), ClientName = "Kennedy", ClientId = _testClientId, Status = Status.Confirmed },
            new CateringForm { Id = Guid.NewGuid(), CateringType = CateringType.Delivery, DietaryRestrictions = [DietaryRestrictions.None], EventDate=new DateTime(2025, 10, 8), ClientName = "NotKennedy", ClientId = Guid.NewGuid(), Status = Status.Confirmed }
        };
        private static readonly CateringFormCreateResponse _testCreateResponse = new CateringFormCreateResponse(
            _testCateringForm.Id,
            _testCateringForm.CateringType,
            _testCateringForm.DietaryRestrictions,
            _testCateringForm.ClientId,
            _testCateringForm.EventDate,
            _testCateringForm.ClientName,
            _testCateringForm.ClientEmail,
            _testCateringForm.ClientPhoneNumber,
            _testCateringForm.Status,
            _testCateringForm.CreatedAt,
            _testCateringForm.UpdatedAt
        );
        private static readonly CateringFormCreateRequest _testCreateRequest = new CateringFormCreateRequest(
                _testCateringForm.Id,
                _testCateringForm.CateringType,
                _testCateringForm.DietaryRestrictions,
                _testCateringForm.ClientId,
                _testCateringForm.EventDate,
                _testCateringForm.ClientName,
                _testCateringForm.ClientEmail,
                _testCateringForm.ClientPhoneNumber,
                _testCateringForm.Status,
                _testCateringForm.CreatedAt,
                _testCateringForm.UpdatedAt
        );

        public CateringFormControllerTests()
        {
            _mockCateringService = new Mock<ICateringFormService>();
            _cateringController = new CateringFormsController(_mockCateringService.Object);
        }

        [Fact]
        public async Task GetCateringFormById_Success()
        {
            // Arrange
            _mockCateringService.Setup(service => service.GetCateringFormByIdAsync(_testCateringForm.Id))
                .ReturnsAsync(_testCateringForm); 
            // Act
            var result = await _cateringController.GetCateringForm(_testCateringForm.Id);
            // Assert
            var actionResult = Assert.IsType<ActionResult<CateringForm>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var returnValue = Assert.IsType<CateringForm>(createdAtActionResult.Value);
            Assert.Equal(_testCateringForm, returnValue);
        }

        [Fact]
        public async Task GetCateringFormById_NotFound()
        {
            // Arrange
            var cateringFormId = Guid.NewGuid();
            _mockCateringService.Setup(service => service.GetCateringFormByIdAsync(cateringFormId))
                .ReturnsAsync((CateringForm?)null); // Simulate not found
            // Act
            var result = await _cateringController.GetCateringForm(cateringFormId); //TODO: This should return something to indicate that the form was not found. Will need to update the service to do so.
            // Assert
            var actionResult = Assert.IsType<ActionResult<CateringForm>>(result);
            var notFoundResult = Assert.IsType<NotFoundResult>(actionResult.Result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task GetCateringForms_AllFilters_Success()
        {
            _mockCateringService.Setup(service => service.GetCateringFormsAsync(_testClientId, Status.Confirmed, new DateTime(2025, 10, 7), new DateTime(2025, 10, 10)))
                .ReturnsAsync(_testCateringForms.Where(cf => cf.ClientId == _testClientId && cf.Status == Status.Confirmed && cf.EventDate >= new DateTime(2025, 10, 7) && cf.EventDate <= new DateTime(2025, 10, 10)).ToList());

            var result = await _cateringController.GetCateringForms(_testClientId, Status.Confirmed, new DateTime(2025, 10, 7), new DateTime(2025, 10, 10));

            var actionResult = Assert.IsType<ActionResult<IEnumerable<CateringForm>>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<CateringForm>>(createdAtActionResult.Value);
            Assert.Single(returnValue);
            Assert.Equal(_testCateringForms[0], returnValue.First());
        }

        [Fact]
        public async Task GetCateringForms_AllFilters_NoFormsFound()
        {
            //Arrange
            DateTime startDate = new DateTime(2023, 1, 1);
            DateTime endDate = new DateTime(2023, 12, 31);
            _mockCateringService.Setup(service => service.GetCateringFormsAsync(_testClientId, Status.Confirmed, startDate, endDate))
                .ReturnsAsync(new List<CateringForm>());
            //Act
            var result = await _cateringController.GetCateringForms(null, null, startDate, endDate);
            //Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<CateringForm>>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<CateringForm>>(createdAtActionResult.Value);
            Assert.Empty(returnValue); //TODO: This should return something to indicate that no forms were found. Will need to update the service to do so. 
        }

        [Fact]
        public async Task GetCateringFormsByClientId_Success()
        {
            // Arrange
            _mockCateringService.Setup(service => service.GetCateringFormsAsync(_testClientId, null, null, null))
                .ReturnsAsync(_testCateringForms);

            // Act
            var result = await _cateringController.GetCateringForms(_testClientId);
            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<CateringForm>>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<CateringForm>>(createdAtActionResult.Value);
            Assert.Equal(4, returnValue.Count());
            Assert.Equal(_testCateringForms, returnValue);
        }

        [Fact]
        public async Task GetCateringFormsByClientId_ClientIdNotFound()
        {
            //Arrange
            _mockCateringService.Setup(service => service.GetCateringFormsAsync(_testClientId, null, null, null))
                .ReturnsAsync(new List<CateringForm>());
            //Act
            var result = await _cateringController.GetCateringForms(_testClientId);
            //Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<CateringForm>>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<CateringForm>>(createdAtActionResult.Value);
            Assert.Empty(returnValue); //TODO: This should return something to indicate that the clientId was not found. Will need to update the service to do so. How is this different from no filters?
        }

        [Fact]
        public async Task GetCateringForms_NoFilters() // Do we want to allow this? What is the expected behaviour? 
        {
            //Arrange
            _mockCateringService.Setup(service => service.GetCateringFormsAsync(null,null,null,null))
                .ReturnsAsync(new List<CateringForm>());
            //Act
            var result = await _cateringController.GetCateringForms();
            //Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<CateringForm>>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<CateringForm>>(createdAtActionResult.Value);
        }

        [Fact]
        public async Task GetCateringFormsByDateRange_Success()
        {
            //Arrange
            DateTime startDate = new DateTime(2025, 10, 7);
            DateTime endDate = new DateTime(2025, 10, 9);
            //TODO: Add catering forms to in-memory DB and have service get it from there.
            _mockCateringService.Setup(service => service.GetCateringFormsAsync(null, null, startDate, endDate))
                .ReturnsAsync(_testCateringForms);
            //Act
            var result = await _cateringController.GetCateringForms(null, null, startDate, endDate); 
        }

        [Fact]
        public async Task GetCateringFormsByDateRange_InvalidRange()
        {
            //Arrange
            DateTime startDate = new DateTime(2023, 12, 31);
            DateTime endDate = new DateTime(2023, 1, 1);
            _mockCateringService.Setup(service => service.GetCateringFormsAsync(null, null, startDate, endDate))
                .ThrowsAsync(new ArgumentException("Start date must be earlier than or equal to end date."));
            //Act
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _cateringController.GetCateringForms(null, null, startDate, endDate));
            //Assert
            Assert.Equal("Start date must be earlier than or equal to end date.", exception.Message);
        }

        [Fact]
        public async Task GetCateringFormsByDateRange_MissingStartOrEndDate()
        {
            //Arrange
            DateTime? startDate = null;
            DateTime endDate = new DateTime(2023, 12, 31);
            _mockCateringService.Setup(service => service.GetCateringFormsAsync(null, null, startDate, endDate))
                .ThrowsAsync(new ArgumentException("Both start date and end date must be provided for date range filtering."));
            //Act
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _cateringController.GetCateringForms(null, null, startDate, endDate)); 
            //Assert
            Assert.Equal("Both start date and end date must be provided for date range filtering.", exception.Message);
        }

        [Fact]
        public async Task GetCateringFormsByStatus_Success()
        {
            //Arrange;
            var cateringForms = new List<CateringForm>
            {
                new CateringForm { Id = Guid.NewGuid(), CateringType = CateringType.Delivery, DietaryRestrictions = [DietaryRestrictions.None], EventDate=new DateTime(), ClientName = "Kennedy", ClientId = _testClientId, Status = Status.Pending },
                new CateringForm { Id = Guid.NewGuid(), CateringType = CateringType.Delivery, DietaryRestrictions = [DietaryRestrictions.None], EventDate=new DateTime(), ClientName = "Kennedy", ClientId = _testClientId, Status = Status.Confirmed },
                new CateringForm { Id = Guid.NewGuid(), CateringType = CateringType.Delivery, DietaryRestrictions = [DietaryRestrictions.None], EventDate=new DateTime(2025, 10, 6), ClientName = "Kennedy", ClientId = _testClientId, Status = Status.Cancelled},
                new CateringForm { Id = Guid.NewGuid(), CateringType = CateringType.Delivery, DietaryRestrictions = [DietaryRestrictions.None], EventDate=new DateTime(), ClientName = "Kennedy", ClientId = _testClientId, Status = Status.Completed }
                //Add new Status types here if more are added to the enum.
            };
            _mockCateringService.Setup(service => service.GetCateringFormsAsync(null, Status.Confirmed, null, null))
                .ReturnsAsync(cateringForms.Where(cf => cf.Status == Status.Confirmed).ToList());

            var result = await _cateringController.GetCateringForms(null, Status.Confirmed, null, null);

            var actionResult = Assert.IsType<ActionResult<IEnumerable<CateringForm>>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<CateringForm>>(createdAtActionResult.Value);
            Assert.Single(returnValue);
            Assert.Equal(cateringForms[1], returnValue.First());

            //TODO: Add tests for each status type if we want to be thorough.
        }

        [Fact]
        public async Task GetCateringFormsByStatus_InvalidStatus()
        {
            _mockCateringService.Setup(service => service.GetCateringFormsAsync(null, (Status)999, null, null))
                .ThrowsAsync(new ArgumentException("Invalid status value."));

            var result = await Assert.ThrowsAsync<ArgumentException>(() => _cateringController.GetCateringForms(null, (Status)999, null, null)); // TODO: I should figure out how to handle this more gracefully.
            Assert.Equal("Invalid status value.", result.Message);
        }

        [Fact]
        public async Task CreateCateringForm_Success()
        {
            _mockCateringService.Setup(service => service.CreateCateringFormAsync(It.IsAny<CateringFormCreateRequest>()))
                .ReturnsAsync(_testCreateResponse);

            var result = await _cateringController.PostCateringForm(_testCreateRequest);

            var actionResult = Assert.IsType<ActionResult<CateringForm>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var returnValue = Assert.IsAssignableFrom<CateringFormCreateResponse>(createdAtActionResult.Value);
            CateringForm returnForm = ConvertResponseToForm(returnValue);
            Assert.True(ValidateCateringForm(ConvertResponseToForm(returnValue), _testCateringForm));
        }

        [Fact]
        public async Task CreateCateringForm_MissingRequiredFields()
        {
            CateringForm cateringForm = new CateringForm { Id = Guid.NewGuid(), CateringType = CateringType.Delivery, DietaryRestrictions = [DietaryRestrictions.None], EventDate = new DateTime(), ClientName = "Kennedy", ClientId = _testClientId, Status = Status.Pending };
            _mockCateringService.Setup(service => service.CreateCateringFormAsync(It.IsAny<CateringFormCreateRequest>()))
                .ThrowsAsync(new ArgumentException("Missing required fields."));

            var result = await Assert.ThrowsAsync<ArgumentException>(() => _cateringController.PostCateringForm(new CateringFormCreateRequest( //How would this gracefully handle null arguments/missing fields instead of throwing an exception? Can we use the Front End to guarantee these parameters are always provided?
                _testCateringForm.Id,
                _testCateringForm.CateringType,
                [],
                _testCateringForm.ClientId,
                _testCateringForm.EventDate,
                _testCateringForm.ClientName,
                _testCateringForm.ClientEmail,
                _testCateringForm.ClientPhoneNumber,
                _testCateringForm.Status,
                _testCateringForm.CreatedAt,
                _testCateringForm.UpdatedAt
            )));

            Assert.Equal("Missing required fields.", result.Message);
        }

        [Fact]
        public async Task CreateCateringForm_DuplicateSubmission()
        {

            CateringForm cateringform = new CateringForm { Id = Guid.NewGuid(), CateringType = CateringType.Delivery, DietaryRestrictions = [DietaryRestrictions.None], EventDate = new DateTime(2025, 10, 9), ClientName = "Kennedy", ClientId = _testClientId, Status = Status.Pending };

            _mockCateringService.Setup(service => service.CreateCateringFormAsync(It.IsAny<CateringFormCreateRequest>()))
                .ThrowsAsync(new Exception("Duplicate submission found. Catering form not created"));

            var result = await Assert.ThrowsAsync<Exception>(() => _cateringController.PostCateringForm(new CateringFormCreateRequest(
                cateringform.Id,
                cateringform.CateringType,
                cateringform.DietaryRestrictions,
                cateringform.ClientId,
                cateringform.EventDate,
                cateringform.ClientName,
                cateringform.ClientEmail,
                cateringform.ClientPhoneNumber,
                cateringform.Status,
                cateringform.CreatedAt,
                cateringform.UpdatedAt
            )));

            Assert.Equal("Duplicate submission found. Catering form not created", result.Message);
        }

        [Fact]
        public async Task UpdateCateringForm_Success()
        {
            _mockCateringService.Setup(service => service.UpdateCateringFormAsync(It.IsAny<CateringFormCreateRequest>()))
                .ReturnsAsync(_testCreateResponse);
            var result = await _cateringController.UpdateCateringForm(_testCreateRequest);
            Assert.IsType<CreatedAtActionResult>(result.Result);
            var actionResult = Assert.IsType<ActionResult<CateringForm>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var returnValue = Assert.IsAssignableFrom<CateringFormCreateResponse>(createdAtActionResult.Value);
            Assert.True(ValidateCateringForm(ConvertResponseToForm(returnValue), _testCateringForm));
        }

        [Fact]
        public async Task UpdateCateringForm_InvalidId()
        {
            _mockCateringService.Setup(service => service.UpdateCateringFormAsync(It.IsAny<CateringFormCreateRequest>()))
                .ThrowsAsync(new ArgumentException("Catering form with the provided ID does not exist."));
            var result = await Assert.ThrowsAsync<ArgumentException>(() => _cateringController.UpdateCateringForm(_testCreateRequest));
            //TODO: Figure out how to handle this more gracefully. Currently throws an NRE instead of an ArgumentException. Also this should not throw an ArgumentException, but something more specific to invalid Id.
            //Assert.Equal("Catering form with the provided ID does not exist.", result.Message);
        }

        [Fact]
        public async Task UpdateCateringForm_InvalidData()
        {
            //TODO: Figure out how to handle this more gracefully. Currently throws an NRE instead of an ArgumentException. Also this should not throw an ArgumentException, but something more specific to invalid data.
            _mockCateringService.Setup(service => service.UpdateCateringFormAsync(It.IsAny<CateringFormCreateRequest>()))
                .ThrowsAsync(new ArgumentException("Invalid data provided for update."));
            var result = await Assert.ThrowsAsync<ArgumentException>(() => _cateringController.UpdateCateringForm(_testCreateRequest));
            //Assert.Equal("Invalid data provided for update.", result.Message);
        }

        [Fact]
        public async Task UpdateCateringForm_TrivialUpdate()
        {
            _mockCateringService.Setup(service => service.UpdateCateringFormAsync(It.IsAny<CateringFormCreateRequest>()))
                .ThrowsAsync(new ArgumentException("No changes detected in the update request."));
            var result = await Assert.ThrowsAsync<ArgumentException>(() => _cateringController.UpdateCateringForm(_testCreateRequest));
            //TODO: Figure out how to handle this more gracefully. Currently throws an NRE instead of an ArgumentException. Also this should not throw an ArgumentException, but something more specific to trivial update.
            //Assert.Equal("No changes detected in the update request.", result.Message);
        }

        public bool ValidateCateringForm(CateringForm returnValue, CateringForm expectedForm)
        {
            return returnValue.Id == expectedForm.Id &&
                   returnValue.CateringType == expectedForm.CateringType &&
                   returnValue.DietaryRestrictions.SequenceEqual(expectedForm.DietaryRestrictions) &&
                   returnValue.ClientId == expectedForm.ClientId &&
                   returnValue.EventDate == expectedForm.EventDate &&
                   returnValue.ClientName == expectedForm.ClientName &&
                   returnValue.ClientEmail == expectedForm.ClientEmail &&
                   returnValue.ClientPhoneNumber == expectedForm.ClientPhoneNumber &&
                   returnValue.Status == expectedForm.Status &&
                   returnValue.CreatedAt == expectedForm.CreatedAt &&
                   returnValue.UpdatedAt == expectedForm.UpdatedAt;
        }

        public CateringForm ConvertResponseToForm(CateringFormCreateResponse response)
        {
            return new CateringForm
            {
                Id = response.Id,
                CateringType = response.CateringType,
                DietaryRestrictions = response.DietaryRestrictions,
                ClientId = response.ClientId,
                EventDate = response.EventDate,
                ClientName = response.ClientName,
                ClientEmail = response.ClientEmail,
                ClientPhoneNumber = response.ClientPhoneNumber,
                Status = response.Status,
                CreatedAt = response.CreatedAt,
                UpdatedAt = response.UpdatedAt
            };
        }
        ///Tests to Write:
        ///GET CateringForms by Date Range
        ///     - Large date range (e.g., multiple years)
        ///POST CateringForm
        ///     - Input boundary testing (e.g., minimum and maximum values for fields)
        ///     - Invalid data types (e.g., strings instead of dates) // How would this gracefully handle null arguments / missing fields instead of throwing an exception? Can we use the Front End to guarantee these parameters are always provided ? What even is invalid data here?
    }
}
