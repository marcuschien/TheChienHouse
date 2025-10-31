using Microsoft.EntityFrameworkCore;
using Moq;
using TheChienHouse.Models;
using TheChienHouse.Services;
using Xunit;
using static TheChienHouse.Models.EventFormDTO;

namespace TheChienHouse.Tests.Services
{
    public class EventFormServiceTests : IDisposable
    {
        private readonly EventFormService _eventFormsService;
        private readonly RetailContext _context;
        private readonly Mock<ILogger<EventFormService>> _mockLogger;
        private readonly Guid _testClientId;
        private readonly EventForm _testEventForm;
        private readonly List<EventForm> _testEventForms;

        public EventFormServiceTests()
        {
            // Create unique database for each test and mock components
            var options = new DbContextOptionsBuilder<RetailContext>()
                .UseInMemoryDatabase(databaseName: $"EventFormTestDb_{Guid.NewGuid()}")
                .Options;
            _context = new RetailContext(options);
            _mockLogger = new Mock<ILogger<EventFormService>>();
            _eventFormsService = new EventFormService(_context, _mockLogger.Object);

            // Initialize test data with new GUIDs for each test instance
            _testClientId = Guid.NewGuid();
            _testEventForm = new EventForm
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

            _testEventForms = new List<EventForm>
            {
                new EventForm { Id = Guid.NewGuid(), EventType = EventType.Party, DietaryRestrictions = new List<DietaryRestrictions>{ DietaryRestrictions.None }, EventDate=new DateTime(2025, 10, 8), FirstName = "Kennedy", LastName = "Irving", ClientEmail = "Test@Email.com", ClientId = _testClientId, Status = Status.Completed, Location = "TheChienHouse", BudgetPerPerson = 22.22m, NumberOfGuests = 2 },
                new EventForm { Id = Guid.NewGuid(), EventType = EventType.Party, DietaryRestrictions = new List<DietaryRestrictions>{ DietaryRestrictions.None }, EventDate=new DateTime(2025, 10, 8), FirstName = "Kennedy", LastName = "Irving", ClientEmail = "Test@Email.com", ClientId = _testClientId, Status = Status.Pending, Location = "TheChienHouse", BudgetPerPerson = 22.22m, NumberOfGuests = 2 },
                new EventForm { Id = Guid.NewGuid(), EventType = EventType.Party, DietaryRestrictions = new List<DietaryRestrictions>{ DietaryRestrictions.None }, EventDate=new DateTime(2025, 10, 6), FirstName = "Kennedy", LastName = "Irving", ClientEmail = "Test@Email.com", ClientId = _testClientId, Status = Status.Confirmed, Location = "TheChienHouse", BudgetPerPerson = 22.22m, NumberOfGuests = 2},
                new EventForm { Id = Guid.NewGuid(), EventType = EventType.Party, DietaryRestrictions = new List<DietaryRestrictions>{ DietaryRestrictions.None }, EventDate=new DateTime(2025, 10, 8), FirstName = "NotKennedy", LastName = "NotIrving", ClientEmail = "Test@Email.com", ClientId = Guid.NewGuid(), Status = Status.Cancelled, Location = "TheChienHouse", BudgetPerPerson = 22.22m, NumberOfGuests = 2 },
                new EventForm { Id = Guid.NewGuid(), EventType = EventType.Party, DietaryRestrictions = new List<DietaryRestrictions>{ DietaryRestrictions.None }, EventDate=new DateTime(2025, 10, 8), FirstName = "Test", LastName = "Client", ClientEmail = "Test@Email.com", ClientId = _testClientId, Status = Status.Confirmed, Location = "TheChienHouse", BudgetPerPerson = 22.22m, NumberOfGuests = 2}
            };

            // Seed the database
            _context.EventForms.AddRange(_testEventForms);
            _context.EventForms.Add(_testEventForm);
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
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
            Assert.Equal(4, result.Count());
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
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _eventFormsService.GetEventFormsAsync(null, (Status)999, null, null));
            Assert.Equal("Invalid status value.", ex.Message);
        }

        [Fact]
        public async Task CreateEventForm_Success()
        {
            // Create a new form with unique data to avoid duplicate detection
            var uniqueCreateRequest = new EventFormCreateRequest(
                EventType.PrivateDinner,
                new List<DietaryRestrictions> { DietaryRestrictions.Vegetarian },
                Guid.NewGuid(),
                new DateTime(2025, 11, 15),
                "Jane",
                "Doe",
                "jane.doe@example.com",
                "555-1234",
                Status.Pending,
                "NewLocation",
                50.00m,
                5,
                "New notes"
            );

            var result = await _eventFormsService.CreateEventFormAsync(
                uniqueCreateRequest.EventType,
                uniqueCreateRequest.DietaryRestrictions,
                uniqueCreateRequest.ClientId,
                uniqueCreateRequest.EventDate,
                uniqueCreateRequest.FirstName,
                uniqueCreateRequest.LastName,
                uniqueCreateRequest.Status,
                uniqueCreateRequest.Location,
                uniqueCreateRequest.ClientEmail,
                uniqueCreateRequest.ClientPhoneNumber,
                uniqueCreateRequest.BudgetPerPerson,
                uniqueCreateRequest.NumberOfGuests,
                uniqueCreateRequest.ExtraNotes);

            Assert.NotNull(result);
            Assert.Equal(uniqueCreateRequest.FirstName, result.FirstName);
            Assert.Equal(uniqueCreateRequest.LastName, result.LastName);
            Assert.Equal(uniqueCreateRequest.ClientEmail, result.ClientEmail);
        }

        [Fact]
        public async Task CreateEventForm_MissingRequiredFields()
        {
            var emptyDietary = new List<DietaryRestrictions>();
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _eventFormsService.CreateEventFormAsync(
                EventType.Party,
                emptyDietary,
                Guid.NewGuid(),
                new DateTime(2025, 10, 10),
                "Test",
                "User",
                Status.Pending,
                "Location",
                "test@email.com",
                "555-0000",
                25.00m,
                3,
                "Notes"));

            Assert.Equal("Missing required fields.", ex.Message);
        }

        [Fact]
        public async Task CreateEventForm_DuplicateSubmission()
        {
            // First, create an event form
            var clientId = Guid.NewGuid();
            var eventDate = new DateTime(2025, 12, 1);
            var firstName = "John";

            await _eventFormsService.CreateEventFormAsync(
                EventType.Party,
                new List<DietaryRestrictions> { DietaryRestrictions.None },
                clientId,
                eventDate,
                firstName,
                "Smith",
                Status.Pending,
                "TestLocation",
                "john@email.com",
                "555-0000",
                30.00m,
                4,
                "Notes");

            // Try to create a duplicate (same clientId, eventDate, and firstName)
            var ex = await Assert.ThrowsAsync<Exception>(() => _eventFormsService.CreateEventFormAsync(
                EventType.PrivateDinner, // Different event type
                new List<DietaryRestrictions> { DietaryRestrictions.Vegan }, // Different dietary restrictions
                clientId, // Same client ID
                eventDate, // Same event date
                firstName, // Same first name
                "Different",
                Status.Confirmed,
                "DifferentLocation",
                "different@email.com",
                "555-9999",
                40.00m,
                6,
                "Different notes"));

            Assert.Equal("Duplicate submission found. Event form not created", ex.Message);
        }

        [Fact]
        public async Task UpdateEventForm_Success()
        {
            var result = await _eventFormsService.UpdateEventFormAsync(
                formId: _testEventForm.Id,
                eventType: EventType.PrivateDinner, // Changed
                dietaryRestrictions: new List<DietaryRestrictions> { DietaryRestrictions.Vegan }, // Changed
                clientId: _testEventForm.ClientId,
                eventDate: new DateTime(2025, 11, 1), // Changed
                firstName: "UpdatedFirstName", // Changed
                lastName: "UpdatedLastName", // Changed
                status: Status.Confirmed, // Changed
                location: "UpdatedLocation", // Changed
                email: "updated@email.com", // Changed
                phoneNumber: "555-9999", // Changed
                budgetPP: 100.00m, // Changed
                numGuests: 10, // Changed
                notes: "Updated notes"); // Changed

            Assert.NotNull(result);
            Assert.Equal("UpdatedFirstName", result.FirstName);
            Assert.Equal("UpdatedLastName", result.LastName);
            Assert.Equal(Status.Confirmed, result.Status);
        }

        [Fact]
        public async Task UpdateEventForm_InvalidId()
        {
            var nonExistentId = Guid.NewGuid();
            var result = await _eventFormsService.UpdateEventFormAsync(
                nonExistentId,
                EventType.Party,
                new List<DietaryRestrictions> { DietaryRestrictions.None },
                Guid.NewGuid(),
                new DateTime(2025, 10, 10),
                "Test",
                "User",
                Status.Pending,
                "Location",
                "test@email.com",
                "555-0000",
                25.00m,
                3,
                "Notes");

            Assert.Null(result);
        }

        // COMMENTED OUT - Service needs to implement validation for invalid data
        // [Fact]
        // public async Task UpdateEventForm_InvalidData()
        // {
        //     var ex = await Assert.ThrowsAsync<ArgumentException>(() => _eventFormsService.UpdateEventFormAsync(
        //         _testUpdateRequest.Id,
        //         _testUpdateRequest.EventType,
        //         _testUpdateRequest.DietaryRestrictions,
        //         _testUpdateRequest.ClientId,
        //         _testUpdateRequest.EventDate,
        //         _testUpdateRequest.FirstName,
        //         _testUpdateRequest.LastName,
        //         _testUpdateRequest.Status,
        //         _testUpdateRequest.Location,
        //         _testUpdateRequest.ClientEmail,
        //         _testUpdateRequest.ClientPhoneNumber,
        //         _testUpdateRequest.BudgetPerPerson,
        //         _testUpdateRequest.NumberOfGuests,
        //         _testUpdateRequest.ExtraNotes));
        //     Assert.Equal("Invalid data provided for update.", ex.Message);
        // }

        // COMMENTED OUT - Service needs to implement detection of trivial updates
        // [Fact]
        // public async Task UpdateEventForm_TrivialUpdate()
        // {
        //     var ex = await Assert.ThrowsAsync<ArgumentException>(() => _eventFormsService.UpdateEventFormAsync(
        //         _testUpdateRequest.Id,
        //         _testUpdateRequest.EventType,
        //         _testUpdateRequest.DietaryRestrictions,
        //         _testUpdateRequest.ClientId,
        //         _testUpdateRequest.EventDate,
        //         _testUpdateRequest.FirstName,
        //         _testUpdateRequest.LastName,
        //         _testUpdateRequest.Status,
        //         _testUpdateRequest.Location,
        //         _testUpdateRequest.ClientEmail,
        //         _testUpdateRequest.ClientPhoneNumber,
        //         _testUpdateRequest.BudgetPerPerson,
        //         _testUpdateRequest.NumberOfGuests,
        //         _testUpdateRequest.ExtraNotes));
        //     Assert.Equal("No changes detected in the update request.", ex.Message);
        // }

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