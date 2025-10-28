using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Moq;
using TheChienHouse.Controllers;
using TheChienHouse.Models;
using TheChienHouse.Services;
using Xunit;
using static TheChienHouse.Models.EventFormDTO;


//TODO: FIX ME. Currently these tests are failing because the same event form is trying to be added to the in-memory DB multiple times (on each test run).
namespace TheChienHouse.Tests.Services
{
    public class EventFormServiceTests
    {
        private readonly EventFormService _eventFormsService;
        private readonly RetailContext _context;
        private readonly Mock<ILogger<EventFormService>> _mockLogger;
        //TODO: Eventually I should migrate these to a test database. 
        private static readonly Guid _testClientId = Guid.NewGuid();
        private static readonly EventForm _testEventForm = new EventForm
        {
            Id = Guid.NewGuid(),
            EventType = EventType.Party,
            DietaryRestrictions = new List<DietaryRestrictions> { DietaryRestrictions.None },
            EventDate = new DateTime(2025, 10, 9),
            FirstName = "Kennedy",
            LastName = "Irving",
            ClientId = Guid.NewGuid(),
            ClientEmail = "Test@Email.com",
            ClientPhoneNumber = "555-0000",
            Status = Status.Pending,
            Location = "TheChienHouse",
            BudgetPerPerson = 22.22m,
            NumberOfGuests = 2,
            ExtraNotes = "Test Notes"
        };
        private static readonly List<EventForm> _testEventForms = new List<EventForm>
        {
            new EventForm { Id = Guid.NewGuid(), EventType = EventType.Party, DietaryRestrictions = new List<DietaryRestrictions>{ DietaryRestrictions.None }, EventDate=new DateTime(2025, 10, 8), FirstName = "Kennedy", LastName = "Irving", ClientEmail = "Test@Email.com", ClientId = _testClientId, Status = Status.Completed, Location = "TheChienHouse", BudgetPerPerson = 22.22m, NumberOfGuests = 2 },
            new EventForm { Id = Guid.NewGuid(), EventType = EventType.Party, DietaryRestrictions = new List<DietaryRestrictions>{ DietaryRestrictions.None }, EventDate=new DateTime(2025, 10, 8), FirstName = "Kennedy", LastName = "Irving", ClientEmail = "Test@Email.com", ClientId = _testClientId, Status = Status.Pending, Location = "TheChienHouse", BudgetPerPerson = 22.22m, NumberOfGuests = 2 },
            new EventForm { Id = Guid.NewGuid(), EventType = EventType.Party, DietaryRestrictions = new List<DietaryRestrictions>{ DietaryRestrictions.None }, EventDate=new DateTime(2025, 10, 6), FirstName = "Kennedy", LastName = "Irving", ClientEmail = "Test@Email.com", ClientId = _testClientId, Status = Status.Confirmed, Location = "TheChienHouse", BudgetPerPerson = 22.22m, NumberOfGuests = 2},
            new EventForm { Id = Guid.NewGuid(), EventType = EventType.Party, DietaryRestrictions = new List<DietaryRestrictions>{ DietaryRestrictions.None }, EventDate=new DateTime(2025, 10, 8), FirstName = "NotKennedy", LastName = "NotIrving", ClientEmail = "Test@Email.com", ClientId = Guid.NewGuid(), Status = Status.Cancelled, Location = "TheChienHouse", BudgetPerPerson = 22.22m, NumberOfGuests = 2 }
        };
        private static readonly EventFormCreateRequest _testCreateRequest = new EventFormCreateRequest(
            _testEventForm.EventType,
            _testEventForm.DietaryRestrictions,
            _testEventForm.ClientId,
            _testEventForm.EventDate,
            _testEventForm.FirstName,
            _testEventForm.LastName,
            _testEventForm.ClientEmail,
            _testEventForm.ClientPhoneNumber,
            _testEventForm.Status, 
            _testEventForm.Location,
            _testEventForm.BudgetPerPerson,
            _testEventForm.NumberOfGuests,
            _testEventForm.ExtraNotes

        );

        private static readonly EventFormUpdateRequest _testUpdateRequest = new EventFormUpdateRequest(
                _testEventForm.Id,
                _testEventForm.EventType,
                _testEventForm.DietaryRestrictions,
                _testEventForm.ClientId,
                _testEventForm.EventDate,
                _testEventForm.FirstName,
                _testEventForm.LastName,
                _testEventForm.ClientEmail,
                _testEventForm.ClientPhoneNumber,
                _testEventForm.Status,
                _testEventForm.Location,
                _testEventForm.BudgetPerPerson,
                _testEventForm.NumberOfGuests,
                _testEventForm.ExtraNotes
            );

        public EventFormServiceTests() //TODO: Figure out how to populate DB context with test data just ONCE so that duplicate items aren't trying to be added. 
        {
            var options = new DbContextOptionsBuilder<RetailContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new RetailContext(options);
            _mockLogger = new Mock<ILogger<EventFormService>>();
            _eventFormsService = new EventFormService(_context, _mockLogger.Object);

            foreach (EventForm eventForm in _testEventForms)
            {
                _context.EventForms.Add(eventForm);
            }
            _context.EventForms.Add(_testEventForm);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetEventFormById_Success()
        {
            var result = await _eventFormsService.GetEventFormByIdAsync(_testEventForm.Id);
            
            Assert.NotNull(result);
            Assert.Equal(_testEventForm.Id, result.Id);
        }
        
        [Fact]
        public async Task GetEventFormById_NotFound()
        {
            var result = await _eventFormsService.GetEventFormByIdAsync(Guid.NewGuid()); // use a non-existent id
            
            Assert.Null(result);
        }

        [Fact]
        public async Task GetEventForms_AllFilters_Success()
        {
            var result = await _eventFormsService.GetEventFormsAsync(_testClientId, Status.Confirmed, new DateTime(2025, 10, 7), new DateTime(2025, 10, 10));
            
            Assert.NotNull(result);
            Assert.Contains(result, r => r.ClientId == _testClientId && r.Status == Status.Confirmed);
        }

        [Fact]
        public async Task GetEventForms_AllFilters_NoFormsFound()
        {
            var result = await _eventFormsService.GetEventFormsAsync(_testClientId, Status.Confirmed, new DateTime(2006, 10, 7), new DateTime(2007, 10, 10));
            
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetEventFormsByClientId_Success()
        {
            var result = await _eventFormsService.GetEventFormsAsync(_testClientId, null, null, null);

            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task GetEventFormsByClientId_ClientIdNotFound()
        {
            var result = await _eventFormsService.GetEventFormsAsync(Guid.NewGuid(), null, null, null); // client id that doesn't exist

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetEventForms_NoFilters() // Do we want to allow this? What is the expected behaviour? 
        {
            var result = await _eventFormsService.GetEventFormsAsync(null, null, null, null);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetEventFormsByDateRange_Success()
        {
            //Arrange
            DateTime startDate = new DateTime(2025, 10, 7);
            DateTime endDate = new DateTime(2025, 10, 9);
            //Act
            var result = await _eventFormsService.GetEventFormsAsync(null, null, startDate, endDate);

            Assert.NotNull(result);
            var expected = new List<EventForm> { _testEventForms[0], _testEventForms[1], _testEventForms[3] };
            // Ensure returned set contains the expected items (order may vary)
            Assert.True(expected.All(e => result.Any(r => r.Id == e.Id)));
        }

        [Fact]
        public async Task GetEventFormsByDateRange_InvalidRange()
        {
            //Arrange
            DateTime startDate = new DateTime(2023, 12, 31);
            DateTime endDate = new DateTime(2023, 1, 1);
            //Act
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _eventFormsService.GetEventFormsAsync(null, null, startDate, endDate));
            //Assert
            Assert.Equal("Start date must be earlier than or equal to end date.", exception.Message);
        }

        [Fact]
        public async Task GetEventFormsByDateRange_MissingStartOrEndDate()
        {
            //Arrange
            DateTime? startDate = null;
            DateTime endDate = new DateTime(2023, 12, 31);
            //Act
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _eventFormsService.GetEventFormsAsync(null, null, startDate, endDate));
            //Assert
            Assert.Equal("Both start date and end date must be provided for date range filtering.", exception.Message);
        }

        [Fact]
        public async Task GetEventFormsByStatus_Success()
        {
            var result = await _eventFormsService.GetEventFormsAsync(null, Status.Confirmed, null, null);

            Assert.Equal(_testEventForms[2].Id, result.First().Id);

            //TODO: Add tests for each status type if we want to be thorough.
        }

        [Fact]
        public async Task GetEventFormsByStatus_InvalidStatus()
        {
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _eventFormsService.GetEventFormsAsync(null, (Status)999, null, null)); // TODO: I should figure out how to handle this more gracefully.
            Assert.Equal("Invalid status value.", ex.Message);
        }

        [Fact]
        public async Task CreateEventForm_Success()
        {

            var result = await _eventFormsService.CreateEventFormAsync(
                _testCreateRequest.EventType,
                _testCreateRequest.DietaryRestrictions,
                _testCreateRequest.ClientId,
                _testCreateRequest.EventDate,
                _testCreateRequest.FirstName,
                _testCreateRequest.LastName,
                _testCreateRequest.Status,
                _testCreateRequest.Location,
                _testCreateRequest.ClientEmail,
                _testCreateRequest.ClientPhoneNumber,
                _testCreateRequest.BudgetPerPerson,
                _testCreateRequest.NumberOfGuests,
                _testCreateRequest.ExtraNotes);

            Assert.True(ValidateEventForm(result, _testEventForm));
        }

        [Fact]
        public async Task CreateEventForm_MissingRequiredFields()
        {
            var emptyDietary = new List<DietaryRestrictions>();
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _eventFormsService.CreateEventFormAsync(
                _testEventForm.EventType,
                emptyDietary,
                _testEventForm.ClientId,
                _testEventForm.EventDate,
                _testEventForm.FirstName,
                _testEventForm.LastName,
                _testEventForm.Status, 
                _testEventForm.Location,
                _testEventForm.ClientEmail,
                _testEventForm.ClientPhoneNumber,
                _testEventForm.BudgetPerPerson,
                _testEventForm.NumberOfGuests,
                _testEventForm.ExtraNotes));

            Assert.Equal("Missing required fields.", ex.Message);
        }

        [Fact]
        public async Task CreateEventForm_DuplicateSubmission()
        {
            var ex = await Assert.ThrowsAsync<Exception>(() => _eventFormsService.CreateEventFormAsync(
                _testEventForm.EventType,
                _testEventForm.DietaryRestrictions,
                _testEventForm.ClientId,
                _testEventForm.EventDate,
                _testEventForm.FirstName,
                _testEventForm.LastName,
                _testEventForm.Status,
                _testEventForm.Location,
                _testEventForm.ClientEmail,
                _testEventForm.ClientPhoneNumber,
                _testEventForm.BudgetPerPerson,
                _testEventForm.NumberOfGuests,
                _testEventForm.ExtraNotes));

            Assert.Equal("Duplicate submission found. Event form not created", ex.Message);
        }

        [Fact]
        public async Task UpdateEventForm_Success()
        {
            var result = await _eventFormsService.UpdateEventFormAsync(
                formId: _testUpdateRequest.Id,
                eventType: _testUpdateRequest.EventType,
                dietaryRestrictions: _testUpdateRequest.DietaryRestrictions,
                clientId: _testUpdateRequest.ClientId,
                eventDate: _testUpdateRequest.EventDate,
                firstName: _testUpdateRequest.FirstName,
                lastName: _testUpdateRequest.LastName,
                status: _testUpdateRequest.Status,
                location: _testUpdateRequest.Location,
                email: _testUpdateRequest.ClientEmail,
                phoneNumber: _testUpdateRequest.ClientPhoneNumber,
                budgetPP: _testUpdateRequest.BudgetPerPerson,
                numGuests: _testUpdateRequest.NumberOfGuests,
                notes: _testUpdateRequest.ExtraNotes);

            Assert.True(ValidateEventForm(result, _testEventForm));
        }

        [Fact]
        public async Task UpdateEventForm_InvalidId()
        {
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _eventFormsService.UpdateEventFormAsync(
                _testUpdateRequest.Id,
                _testUpdateRequest.EventType,
                _testUpdateRequest.DietaryRestrictions,
                _testUpdateRequest.ClientId,
                _testUpdateRequest.EventDate,
                _testUpdateRequest.FirstName,
                _testUpdateRequest.LastName,
                _testUpdateRequest.Status,
                _testUpdateRequest.Location,
                _testUpdateRequest.ClientEmail,
                _testUpdateRequest.ClientPhoneNumber,
                _testUpdateRequest.BudgetPerPerson,
                _testUpdateRequest.NumberOfGuests,
                _testUpdateRequest.ExtraNotes));
            //TODO: Figure out how to handle this more gracefully. Currently throws an NRE instead of an ArgumentException. Also this should not throw an ArgumentException, but something more specific to invalid Id.
            //Assert.Equal("Event form with the provided ID does not exist.", result.Message);
        }

        [Fact]
        public async Task UpdateEventForm_InvalidData()
        {
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _eventFormsService.UpdateEventFormAsync(
                _testUpdateRequest.Id,
                _testUpdateRequest.EventType,
                _testUpdateRequest.DietaryRestrictions,
                _testUpdateRequest.ClientId,
                _testUpdateRequest.EventDate,
                _testUpdateRequest.FirstName,
                _testUpdateRequest.LastName,
                _testUpdateRequest.Status,
                _testUpdateRequest.Location,
                _testUpdateRequest.ClientEmail,
                _testUpdateRequest.ClientPhoneNumber,
                _testUpdateRequest.BudgetPerPerson,
                _testUpdateRequest.NumberOfGuests,
                _testUpdateRequest.ExtraNotes));
            //Assert.Equal("Invalid data provided for update.", result.Message);
        }

        [Fact]
        public async Task UpdateEventForm_TrivialUpdate()
        {
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _eventFormsService.UpdateEventFormAsync(
                _testUpdateRequest.Id,
                _testUpdateRequest.EventType,
                _testUpdateRequest.DietaryRestrictions,
                _testUpdateRequest.ClientId,
                _testUpdateRequest.EventDate,
                _testUpdateRequest.FirstName,
                _testUpdateRequest.LastName,
                _testUpdateRequest.Status,
                _testUpdateRequest.Location,
                _testUpdateRequest.ClientEmail,
                _testUpdateRequest.ClientPhoneNumber,
                _testUpdateRequest.BudgetPerPerson,
                _testUpdateRequest.NumberOfGuests,
                _testUpdateRequest.ExtraNotes));
            //TODO: Figure out how to handle this more gracefully. Currently throws an NRE instead of an ArgumentException. Also this should not throw an ArgumentException, but something more specific to trivial update.
            //Assert.Equal("No changes detected in the update request.", result.Message);
        }

        public bool ValidateEventForm(EventForm returnValue, EventForm expectedForm)
        {
            return returnValue.EventType == expectedForm.EventType &&
                   returnValue.DietaryRestrictions.SequenceEqual(expectedForm.DietaryRestrictions) &&
                   returnValue.ClientId == expectedForm.ClientId &&
                   returnValue.EventDate == expectedForm.EventDate &&
                   returnValue.FirstName == expectedForm.FirstName &&
                   returnValue.LastName == expectedForm.LastName &&
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
                FirstName = response.FirstName,
                LastName = response.LastName,
                ClientEmail = response.ClientEmail,
                ClientPhoneNumber = response.ClientPhoneNumber,
                Status = response.Status,
                CreatedAt = response.CreatedAt,
                UpdatedAt = response.UpdatedAt,
                Location = response.Location,
                BudgetPerPerson = response.BudgetPerPerson,
                NumberOfGuests = response.NumberOfGuests,
                ExtraNotes = response.ExtraNotes
            };
        }
    }
}
