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
using static TheChienHouse.Models.EventFormDTO;

namespace TheChienHouse.Tests.Controllers
{
    public class EventFormControllerTests
    {
        private readonly Mock<IEventFormService> _mockEventFormService;
        private readonly EventFormsController _eventFormsController;
        //TODO: Eventually I should migrate these to a test database seeding strategy.
        private static readonly Guid _testClientId = Guid.NewGuid();
        private static readonly EventForm _testEventForm = new EventForm
        {
            Id = Guid.NewGuid(),
            EventType = EventType.Party,
            DietaryRestrictions = [DietaryRestrictions.None],
            EventDate = new DateTime(2025, 10, 9),
            ClientName = "Kennedy",
            ClientId = Guid.NewGuid(),
            Status = Status.Pending
        };
        private static readonly List<EventForm> _testEventForms = new List<EventForm>
        {
            new EventForm { Id = Guid.NewGuid(), EventType = EventType.Party, DietaryRestrictions = [DietaryRestrictions.None], EventDate=new DateTime(2025, 10, 8), ClientName = "Kennedy", ClientId = _testClientId, Status = Status.Confirmed },
            new EventForm { Id = Guid.NewGuid(), EventType = EventType.Party, DietaryRestrictions = [DietaryRestrictions.None], EventDate=new DateTime(2025, 10, 8), ClientName = "Kennedy", ClientId = _testClientId, Status = Status.Pending },
            new EventForm { Id = Guid.NewGuid(), EventType = EventType.Party, DietaryRestrictions = [DietaryRestrictions.None], EventDate=new DateTime(2025, 10, 6), ClientName = "Kennedy", ClientId = _testClientId, Status = Status.Confirmed },
            new EventForm { Id = Guid.NewGuid(), EventType = EventType.Party, DietaryRestrictions = [DietaryRestrictions.None], EventDate=new DateTime(2025, 10, 8), ClientName = "NotKennedy", ClientId = Guid.NewGuid(), Status = Status.Confirmed }
        };
        private static readonly EventFormCreateResponse _testCreateResponse = new EventFormCreateResponse(
            _testEventForm.Id,
            _testEventForm.EventType,
            _testEventForm.DietaryRestrictions,
            _testEventForm.ClientId,
            _testEventForm.EventDate,
            _testEventForm.ClientName,
            _testEventForm.ClientEmail,
            _testEventForm.ClientPhoneNumber,
            _testEventForm.Status,
            _testEventForm.CreatedAt,
            _testEventForm.UpdatedAt
        );
        private static readonly EventFormCreateRequest _testCreateRequest = new EventFormCreateRequest(
                _testEventForm.EventType,
                _testEventForm.DietaryRestrictions,
                _testEventForm.ClientId,
                _testEventForm.EventDate,
                _testEventForm.ClientName,
                _testEventForm.ClientEmail,
                _testEventForm.ClientPhoneNumber,
                _testEventForm.Status
        );

        private static readonly EventFormUpdateRequest _testUpdateRequest = new EventFormUpdateRequest(
                _testEventForm.Id,
                _testEventForm.EventType,
                _testEventForm.DietaryRestrictions,
                _testEventForm.ClientId,
                _testEventForm.EventDate,
                _testEventForm.ClientName,
                _testEventForm.ClientEmail,
                _testEventForm.ClientPhoneNumber,
                _testEventForm.Status
            );

        public EventFormControllerTests()
        {
            _mockEventFormService = new Mock<IEventFormService>();
            _eventFormsController = new EventFormsController(_mockEventFormService.Object);
        }

        [Fact]
        public async Task GetEventFormById_Success()
        {
            // Arrange
            _mockEventFormService.Setup(service => service.GetEventFormByIdAsync(_testEventForm.Id))
                .ReturnsAsync(_testEventForm); 
            // Act
            var result = await _eventFormsController.GetEventForm(_testEventForm.Id);
            // Assert
            var actionResult = Assert.IsType<ActionResult<EventForm>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var returnValue = Assert.IsType<EventForm>(createdAtActionResult.Value);
            Assert.Equal(_testEventForm, returnValue);
        }

        [Fact]
        public async Task GetEventFormById_NotFound()
        {
            // Arrange
            var EventFormId = Guid.NewGuid();
            _mockEventFormService.Setup(service => service.GetEventFormByIdAsync(EventFormId))
                .ReturnsAsync((EventForm?)null); // Simulate not found
            // Act
            var result = await _eventFormsController.GetEventForm(EventFormId); //TODO: This should return something to indicate that the form was not found. Will need to update the service to do so.
            // Assert
            var actionResult = Assert.IsType<ActionResult<EventForm>>(result);
            var notFoundResult = Assert.IsType<NotFoundResult>(actionResult.Result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task GetEventForms_AllFilters_Success()
        {
            _mockEventFormService.Setup(service => service.GetEventFormsAsync(_testClientId, Status.Confirmed, new DateTime(2025, 10, 7), new DateTime(2025, 10, 10)))
                .ReturnsAsync(_testEventForms.Where(cf => cf.ClientId == _testClientId && cf.Status == Status.Confirmed && cf.EventDate >= new DateTime(2025, 10, 7) && cf.EventDate <= new DateTime(2025, 10, 10)).ToList());

            var result = await _eventFormsController.GetEventForms(_testClientId, Status.Confirmed, new DateTime(2025, 10, 7), new DateTime(2025, 10, 10));

            var actionResult = Assert.IsType<ActionResult<IEnumerable<EventForm>>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<EventForm>>(createdAtActionResult.Value);
            Assert.Single(returnValue);
            Assert.Equal(_testEventForms[0], returnValue.First());
        }

        [Fact]
        public async Task GetEventForms_AllFilters_NoFormsFound()
        {
            //Arrange
            DateTime startDate = new DateTime(2023, 1, 1);
            DateTime endDate = new DateTime(2023, 12, 31);
            _mockEventFormService.Setup(service => service.GetEventFormsAsync(_testClientId, Status.Confirmed, startDate, endDate))
                .ReturnsAsync(new List<EventForm>());
            //Act
            var result = await _eventFormsController.GetEventForms(null, null, startDate, endDate);
            //Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<EventForm>>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<EventForm>>(createdAtActionResult.Value);
            Assert.Empty(returnValue); //TODO: This should return something to indicate that no forms were found. Will need to update the service to do so. 
        }

        [Fact]
        public async Task GetEventFormsByClientId_Success()
        {
            // Arrange
            _mockEventFormService.Setup(service => service.GetEventFormsAsync(_testClientId, null, null, null))
                .ReturnsAsync(_testEventForms);

            // Act
            var result = await _eventFormsController.GetEventForms(_testClientId);
            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<EventForm>>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<EventForm>>(createdAtActionResult.Value);
            Assert.Equal(4, returnValue.Count());
            Assert.Equal(_testEventForms, returnValue);
        }

        [Fact]
        public async Task GetEventFormsByClientId_ClientIdNotFound()
        {
            //Arrange
            _mockEventFormService.Setup(service => service.GetEventFormsAsync(_testClientId, null, null, null))
                .ReturnsAsync(new List<EventForm>());
            //Act
            var result = await _eventFormsController.GetEventForms(_testClientId);
            //Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<EventForm>>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<EventForm>>(createdAtActionResult.Value);
            Assert.Empty(returnValue); //TODO: This should return something to indicate that the clientId was not found. Will need to update the service to do so. How is this different from no filters?
        }

        [Fact]
        public async Task GetEventForms_NoFilters() // Do we want to allow this? What is the expected behaviour? 
        {
            //Arrange
            _mockEventFormService.Setup(service => service.GetEventFormsAsync(null,null,null,null))
                .ReturnsAsync(new List<EventForm>());
            //Act
            var result = await _eventFormsController.GetEventForms();
            //Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<EventForm>>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<EventForm>>(createdAtActionResult.Value);
        }

        [Fact]
        public async Task GetEventFormsByDateRange_Success()
        {
            //Arrange
            DateTime startDate = new DateTime(2025, 10, 7);
            DateTime endDate = new DateTime(2025, 10, 9);
            //TODO: Add Event forms to in-memory DB and have service get it from there.
            _mockEventFormService.Setup(service => service.GetEventFormsAsync(null, null, startDate, endDate))
                .ReturnsAsync(_testEventForms);
            //Act
            var result = await _eventFormsController.GetEventForms(null, null, startDate, endDate); 
        }

        [Fact]
        public async Task GetEventFormsByDateRange_InvalidRange()
        {
            //Arrange
            DateTime startDate = new DateTime(2023, 12, 31);
            DateTime endDate = new DateTime(2023, 1, 1);
            _mockEventFormService.Setup(service => service.GetEventFormsAsync(null, null, startDate, endDate))
                .ThrowsAsync(new ArgumentException("Start date must be earlier than or equal to end date."));
            //Act
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _eventFormsController.GetEventForms(null, null, startDate, endDate));
            //Assert
            Assert.Equal("Start date must be earlier than or equal to end date.", exception.Message);
        }

        [Fact]
        public async Task GetEventFormsByDateRange_MissingStartOrEndDate()
        {
            //Arrange
            DateTime? startDate = null;
            DateTime endDate = new DateTime(2023, 12, 31);
            _mockEventFormService.Setup(service => service.GetEventFormsAsync(null, null, startDate, endDate))
                .ThrowsAsync(new ArgumentException("Both start date and end date must be provided for date range filtering."));
            //Act
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _eventFormsController.GetEventForms(null, null, startDate, endDate)); 
            //Assert
            Assert.Equal("Both start date and end date must be provided for date range filtering.", exception.Message);
        }

        [Fact]
        public async Task GetEventFormsByStatus_Success()
        {
            //Arrange;
            var eventForms = new List<EventForm>
            {
                new EventForm { Id = Guid.NewGuid(), EventType = EventType.Party, DietaryRestrictions = [DietaryRestrictions.None], EventDate=new DateTime(), ClientName = "Kennedy", ClientId = _testClientId, Status = Status.Pending },
                new EventForm { Id = Guid.NewGuid(), EventType = EventType.Party, DietaryRestrictions = [DietaryRestrictions.None], EventDate=new DateTime(), ClientName = "Kennedy", ClientId = _testClientId, Status = Status.Confirmed },
                new EventForm { Id = Guid.NewGuid(), EventType = EventType.Party, DietaryRestrictions = [DietaryRestrictions.None], EventDate=new DateTime(2025, 10, 6), ClientName = "Kennedy", ClientId = _testClientId, Status = Status.Cancelled},
                new EventForm { Id = Guid.NewGuid(), EventType = EventType.Party, DietaryRestrictions = [DietaryRestrictions.None], EventDate=new DateTime(), ClientName = "Kennedy", ClientId = _testClientId, Status = Status.Completed }
                //Add new Status types here if more are added to the enum.
            };
            _mockEventFormService.Setup(service => service.GetEventFormsAsync(null, Status.Confirmed, null, null))
                .ReturnsAsync(eventForms.Where(cf => cf.Status == Status.Confirmed).ToList());

            var result = await _eventFormsController.GetEventForms(null, Status.Confirmed, null, null);

            var actionResult = Assert.IsType<ActionResult<IEnumerable<EventForm>>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<EventForm>>(createdAtActionResult.Value);
            Assert.Single(returnValue);
            Assert.Equal(eventForms[1], returnValue.First());

            //TODO: Add tests for each status type if we want to be thorough.
        }

        [Fact]
        public async Task GetEventFormsByStatus_InvalidStatus()
        {
            _mockEventFormService.Setup(service => service.GetEventFormsAsync(null, (Status)999, null, null))
                .ThrowsAsync(new ArgumentException("Invalid status value."));

            var result = await Assert.ThrowsAsync<ArgumentException>(() => _eventFormsController.GetEventForms(null, (Status)999, null, null)); // TODO: I should figure out how to handle this more gracefully.
            Assert.Equal("Invalid status value.", result.Message);
        }

        [Fact]
        public async Task CreateEventForm_Success()
        {
            _mockEventFormService.Setup(service => service.CreateEventFormAsync(It.IsAny<EventFormCreateRequest>()))
                .ReturnsAsync(_testCreateResponse);

            var result = await _eventFormsController.PostEventForm(_testCreateRequest);

            var actionResult = Assert.IsType<ActionResult<EventForm>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var returnValue = Assert.IsAssignableFrom<EventFormCreateResponse>(createdAtActionResult.Value);
            EventForm returnForm = ConvertResponseToForm(returnValue);
            Assert.True(ValidateEventForm(ConvertResponseToForm(returnValue), _testEventForm));
        }

        [Fact]
        public async Task CreateEventForm_MissingRequiredFields()
        {
            EventForm EventForm = new EventForm { Id = Guid.NewGuid(), EventType = EventType.Party, DietaryRestrictions = [DietaryRestrictions.None], EventDate = new DateTime(), ClientName = "Kennedy", ClientId = _testClientId, Status = Status.Pending };
            _mockEventFormService.Setup(service => service.CreateEventFormAsync(It.IsAny<EventFormCreateRequest>()))
                .ThrowsAsync(new ArgumentException("Missing required fields."));

            var result = await Assert.ThrowsAsync<ArgumentException>(() => _eventFormsController.PostEventForm(new EventFormCreateRequest( //How would this gracefully handle null arguments/missing fields instead of throwing an exception? Can we use the Front End to guarantee these parameters are always provided?
                _testEventForm.EventType,
                [],
                _testEventForm.ClientId,
                _testEventForm.EventDate,
                _testEventForm.ClientName,
                _testEventForm.ClientEmail,
                _testEventForm.ClientPhoneNumber,
                _testEventForm.Status
            )));

            Assert.Equal("Missing required fields.", result.Message);
        }

        [Fact]
        public async Task CreateEventForm_DuplicateSubmission()
        {

            EventForm Eventform = new EventForm { Id = Guid.NewGuid(), EventType = EventType.Party, DietaryRestrictions = [DietaryRestrictions.None], EventDate = new DateTime(2025, 10, 9), ClientName = "Kennedy", ClientId = _testClientId, Status = Status.Pending };

            _mockEventFormService.Setup(service => service.CreateEventFormAsync(It.IsAny<EventFormCreateRequest>()))
                .ThrowsAsync(new Exception("Duplicate submission found. Event form not created"));

            var result = await Assert.ThrowsAsync<Exception>(() => _eventFormsController.PostEventForm(new EventFormCreateRequest(
                Eventform.EventType,
                Eventform.DietaryRestrictions,
                Eventform.ClientId,
                Eventform.EventDate,
                Eventform.ClientName,
                Eventform.ClientEmail,
                Eventform.ClientPhoneNumber,
                Eventform.Status
            )));

            Assert.Equal("Duplicate submission found. Event form not created", result.Message);
        }

        [Fact]
        public async Task UpdateEventForm_Success()
        {
            _mockEventFormService.Setup(service => service.UpdateEventFormAsync(It.IsAny<EventFormUpdateRequest>()))
                .ReturnsAsync(_testCreateResponse);
            var result = await _eventFormsController.UpdateEventForm(_testUpdateRequest);
            Assert.IsType<CreatedAtActionResult>(result.Result);
            var actionResult = Assert.IsType<ActionResult<EventForm>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var returnValue = Assert.IsAssignableFrom<EventFormCreateResponse>(createdAtActionResult.Value);
            Assert.True(ValidateEventForm(ConvertResponseToForm(returnValue), _testEventForm));
        }

        [Fact]
        public async Task UpdateEventForm_InvalidId()
        {
            _mockEventFormService.Setup(service => service.UpdateEventFormAsync(It.IsAny<EventFormUpdateRequest>()))
                .ThrowsAsync(new ArgumentException("Event form with the provided ID does not exist."));
            var result = await Assert.ThrowsAsync<ArgumentException>(() => _eventFormsController.UpdateEventForm(_testUpdateRequest)); //TODO: Update me. Should return 404 with message "Event form with ID '{id}' was not found."
        }

        [Fact]
        public async Task UpdateEventForm_InvalidData()
        {
            //TODO: Figure out how to handle this more gracefully. Currently throws an NRE instead of an ArgumentException. Also this should not throw an ArgumentException, but something more specific to invalid data.
            _mockEventFormService.Setup(service => service.UpdateEventFormAsync(It.IsAny<EventFormUpdateRequest>()))
                .ThrowsAsync(new ArgumentException("Invalid data provided for update."));
            var result = await Assert.ThrowsAsync<ArgumentException>(() => _eventFormsController.UpdateEventForm(_testUpdateRequest));
            //Assert.Equal("Invalid data provided for update.", result.Message);
        }

        [Fact]
        public async Task UpdateEventForm_TrivialUpdate()
        {
            _mockEventFormService.Setup(service => service.UpdateEventFormAsync(It.IsAny<EventFormUpdateRequest>()))
                .ThrowsAsync(new ArgumentException("No changes detected in the update request."));
            var result = await Assert.ThrowsAsync<ArgumentException>(() => _eventFormsController.UpdateEventForm(_testUpdateRequest));
            //TODO: Figure out how to handle this more gracefully. Currently throws an NRE instead of an ArgumentException. Also this should not throw an ArgumentException, but something more specific to trivial update.
            //Assert.Equal("No changes detected in the update request.", result.Message);
        }

        public bool ValidateEventForm(EventForm returnValue, EventForm expectedForm)
        {
            return returnValue.Id == expectedForm.Id &&
                   returnValue.EventType == expectedForm.EventType &&
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

        public EventForm ConvertResponseToForm(EventFormCreateResponse response)
        {
            return new EventForm
            {
                Id = response.Id,
                EventType = response.EventType,
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
        ///GET EventForms by Date Range
        ///     - Large date range (e.g., multiple years)
        ///POST EventForm
        ///     - Input boundary testing (e.g., minimum and maximum values for fields)
        ///     - Invalid data types (e.g., strings instead of dates) // How would this gracefully handle null arguments / missing fields instead of throwing an exception? Can we use the Front End to guarantee these parameters are always provided ? What even is invalid data here?
    }
}
