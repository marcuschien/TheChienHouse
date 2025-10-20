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
            DietaryRestrictions = [DietaryRestrictions.None],
            EventDate = new DateTime(2025, 10, 9),
            ClientName = "Kennedy",
            ClientId = Guid.NewGuid(),
            Status = Status.Pending,
            Location = "TheChienHouse",
            BudgetPerPerson = 22.22m,
            NumberOfGuests = 2,
            ExtraNotes = "Test Notes"
        };
        private static readonly List<EventForm> _testEventForms = new List<EventForm>
        {
            new EventForm { Id = Guid.NewGuid(), EventType = EventType.Party, DietaryRestrictions = [DietaryRestrictions.None], EventDate=new DateTime(2025, 10, 8), ClientName = "Kennedy", ClientId = _testClientId, Status = Status.Completed, Location = "TheChienHouse", BudgetPerPerson = 22.22m, NumberOfGuests = 2 },
            new EventForm { Id = Guid.NewGuid(), EventType = EventType.Party, DietaryRestrictions = [DietaryRestrictions.None], EventDate=new DateTime(2025, 10, 8), ClientName = "Kennedy", ClientId = _testClientId, Status = Status.Pending, Location = "TheChienHouse", BudgetPerPerson = 22.22m, NumberOfGuests = 2 },
            new EventForm { Id = Guid.NewGuid(), EventType = EventType.Party, DietaryRestrictions = [DietaryRestrictions.None], EventDate=new DateTime(2025, 10, 6), ClientName = "Kennedy", ClientId = _testClientId, Status = Status.Confirmed, Location = "TheChienHouse", BudgetPerPerson = 22.22m, NumberOfGuests = 2},
            new EventForm { Id = Guid.NewGuid(), EventType = EventType.Party, DietaryRestrictions = [DietaryRestrictions.None], EventDate=new DateTime(2025, 10, 8), ClientName = "NotKennedy", ClientId = Guid.NewGuid(), Status = Status.Cancelled, Location = "TheChienHouse", BudgetPerPerson = 22.22m, NumberOfGuests = 2 }
        };
        private static readonly EventFormCreateRequest _testCreateRequest = new EventFormCreateRequest(
            _testEventForm.EventType,
            _testEventForm.DietaryRestrictions,
            _testEventForm.ClientId,
            _testEventForm.EventDate,
            _testEventForm.ClientName,
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
                _testEventForm.ClientName,
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
            Assert.Equal(_testEventForm, result);
        }
        
        [Fact]
        public async Task GetEventFormById_NotFound()
        {
            var result = await _eventFormsService.GetEventFormByIdAsync(_testEventForm.Id); //TODO: Fix me. Give a non valid ID
            
            Assert.Null(result); //TODO: This should return something to indicate that the form was not found. Will need to update the service to do so.
        }

        [Fact]
        public async Task GetEventForms_AllFilters_Success()
        {
            var result = await _eventFormsService.GetEventFormsAsync(_testClientId, Status.Confirmed, new DateTime(2025, 10, 7), new DateTime(2025, 10, 10));
            
            Assert.NotNull(result);
            Assert.Equal(_testEventForm, result.First());
        }

        [Fact]
        public async Task GetEventForms_AllFilters_NoFormsFound()
        {
            var result = await _eventFormsService.GetEventFormsAsync(_testClientId, Status.Confirmed, new DateTime(2006, 10, 7), new DateTime(2007, 10, 10));
            
            Assert.Null(result);//TODO: This should return something to indicate that no forms were found. Will need to update the service to do so. 
        }

        [Fact]
        public async Task GetEventFormsByClientId_Success()
        {
            var result = await _eventFormsService.GetEventFormsAsync(_testEventForm.Id, null, null, null);

            Assert.NotNull(result);
            Assert.Equal(4, result.Count());
            Assert.Equal(_testEventForms, result);
        }

        [Fact]
        public async Task GetEventFormsByClientId_ClientIdNotFound()
        {
            var result = await _eventFormsService.GetEventFormsAsync(_testClientId, null, null, null); // TODO: Give a client id that doesnt' exist

            Assert.Null(result); //TODO: This should return something to indicate that the clientId was not found. Will need to update the service to do so. How is this different from no filters?
        }

        [Fact]
        public async Task GetEventForms_NoFilters() // Do we want to allow this? What is the expected behaviour? 
        {
            var result = await _eventFormsService.GetEventFormsAsync(null, null, null, null);
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
            Assert.Equal([_testEventForms[0], _testEventForms[1], _testEventForms[3]], result.ToList());
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

            Assert.Equal(_testEventForms[1], result.First());

            //TODO: Add tests for each status type if we want to be thorough.
        }

        [Fact]
        public async Task GetEventFormsByStatus_InvalidStatus()
        {
            var result = await Assert.ThrowsAsync<ArgumentException>(() => _eventFormsService.GetEventFormsAsync(null, (Status)999, null, null)); // TODO: I should figure out how to handle this more gracefully.
            Assert.Equal("Invalid status value.", result.Message);
        }

        [Fact]
        public async Task CreateEventForm_Success()
        {

            var result = await _eventFormsService.CreateEventFormAsync(_testCreateRequest);

            Assert.True(ValidateEventForm(ConvertResponseToForm(result), _testEventForm));
        }

        [Fact]
        public async Task CreateEventForm_MissingRequiredFields()
        {
            var result = await Assert.ThrowsAsync<ArgumentException>(() => _eventFormsService.CreateEventFormAsync(new EventFormCreateRequest( //How would this gracefully handle null arguments/missing fields instead of throwing an exception? Can we use the Front End to guarantee these parameters are always provided?
                _testEventForm.EventType,
                [],
                _testEventForm.ClientId,
                _testEventForm.EventDate,
                _testEventForm.ClientName,
                _testEventForm.ClientEmail,
                _testEventForm.ClientPhoneNumber,
                _testEventForm.Status, 
                _testEventForm.Location,
                _testEventForm.BudgetPerPerson,
                _testEventForm.NumberOfGuests,
                _testEventForm.ExtraNotes
            )));

            Assert.Equal("Missing required fields.", result.Message);
        }

        [Fact]
        public async Task CreateEventForm_DuplicateSubmission()
        {
            var result = await Assert.ThrowsAsync<Exception>(() => _eventFormsService.CreateEventFormAsync(new EventFormCreateRequest(
                _testEventForm.EventType,
                _testEventForm.DietaryRestrictions,
                _testEventForm.ClientId,
                _testEventForm.EventDate,
                _testEventForm.ClientName,
                _testEventForm.ClientEmail,
                _testEventForm.ClientPhoneNumber,
                _testEventForm.Status,
                _testEventForm.Location,
                _testEventForm.BudgetPerPerson,
                _testEventForm.NumberOfGuests,
                _testEventForm.ExtraNotes
            )));

            Assert.Equal("Duplicate submission found. Event form not created", result.Message);
        }

        [Fact]
        public async Task UpdateEventForm_Success()
        {
            var result = await _eventFormsService.UpdateEventFormAsync(_testUpdateRequest);
            Assert.True(ValidateEventForm(ConvertResponseToForm(result), _testEventForm));
        }

        [Fact]
        public async Task UpdateEventForm_InvalidId()
        {
            var result = await Assert.ThrowsAsync<ArgumentException>(() => _eventFormsService.UpdateEventFormAsync(_testUpdateRequest));
            //TODO: Figure out how to handle this more gracefully. Currently throws an NRE instead of an ArgumentException. Also this should not throw an ArgumentException, but something more specific to invalid Id.
            //Assert.Equal("Event form with the provided ID does not exist.", result.Message);
        }

        [Fact]
        public async Task UpdateEventForm_InvalidData()
        {
            //TODO: Figure out how to handle this more gracefully. Currently throws an NRE instead of an ArgumentException. Also this should not throw an ArgumentException, but something more specific to invalid data.
            var result = await Assert.ThrowsAsync<ArgumentException>(() => _eventFormsService.UpdateEventFormAsync(_testUpdateRequest));
            //Assert.Equal("Invalid data provided for update.", result.Message);
        }

        [Fact]
        public async Task UpdateEventForm_TrivialUpdate()
        {
            var result = await Assert.ThrowsAsync<ArgumentException>(() => _eventFormsService.UpdateEventFormAsync(_testUpdateRequest));
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
                UpdatedAt = response.UpdatedAt,
                Location = response.Location,
                BudgetPerPerson = response.BudgetPerPerson,
                NumberOfGuests = response.NumberOfGuests,
                ExtraNotes = response.ExtraNotes
            };
        }
    }
}
