using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Moq;
using TheChienHouse.Controllers;
using TheChienHouse.Models;
using TheChienHouse.Services;
using Xunit;
using static TheChienHouse.Models.CateringFormDTO;


//TODO: FIX ME. Currently these tests are failing because the same catering form is trying to be added to the in-memory DB multiple times (on each test run).
namespace TheChienHouse.Tests.Services
{
    public class CateringFormServiceTests
    {
        private readonly CateringFormService _cateringService;
        private readonly RetailContext _context;
        private readonly Mock<ILogger<CateringFormService>> _mockLogger;
        //TODO: Eventually I should migrate these to a test database. 
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
            new CateringForm { Id = Guid.NewGuid(), CateringType = CateringType.Delivery, DietaryRestrictions = [DietaryRestrictions.None], EventDate=new DateTime(2025, 10, 8), ClientName = "Kennedy", ClientId = _testClientId, Status = Status.Completed },
            new CateringForm { Id = Guid.NewGuid(), CateringType = CateringType.Delivery, DietaryRestrictions = [DietaryRestrictions.None], EventDate=new DateTime(2025, 10, 8), ClientName = "Kennedy", ClientId = _testClientId, Status = Status.Pending },
            new CateringForm { Id = Guid.NewGuid(), CateringType = CateringType.Delivery, DietaryRestrictions = [DietaryRestrictions.None], EventDate=new DateTime(2025, 10, 6), ClientName = "Kennedy", ClientId = _testClientId, Status = Status.Confirmed },
            new CateringForm { Id = Guid.NewGuid(), CateringType = CateringType.Delivery, DietaryRestrictions = [DietaryRestrictions.None], EventDate=new DateTime(2025, 10, 8), ClientName = "NotKennedy", ClientId = Guid.NewGuid(), Status = Status.Cancelled }
        };
        private static readonly CateringFormCreateRequest _testCreateRequest = new CateringFormCreateRequest(
                _testCateringForm.CateringType,
                _testCateringForm.DietaryRestrictions,
                _testCateringForm.ClientId,
                _testCateringForm.EventDate,
                _testCateringForm.ClientName,
                _testCateringForm.ClientEmail,
                _testCateringForm.ClientPhoneNumber,
                _testCateringForm.Status
        );

        private static readonly CateringFormUpdateRequest _testUpdateRequest = new CateringFormUpdateRequest(
                _testCateringForm.Id,
                _testCateringForm.CateringType,
                _testCateringForm.DietaryRestrictions,
                _testCateringForm.ClientId,
                _testCateringForm.EventDate,
                _testCateringForm.ClientName,
                _testCateringForm.ClientEmail,
                _testCateringForm.ClientPhoneNumber,
                _testCateringForm.Status
            );

        public CateringFormServiceTests() //TODO: Figure out how to populate DB context with test data just ONCE so that duplicate items aren't trying to be added. 
        {
            var options = new DbContextOptionsBuilder<RetailContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new RetailContext(options);
            _mockLogger = new Mock<ILogger<CateringFormService>>();
            _cateringService = new CateringFormService(_context, _mockLogger.Object);

            foreach (CateringForm cateringForm in _testCateringForms)
            {
                _context.CateringForms.Add(cateringForm);
            }
            _context.CateringForms.Add(_testCateringForm);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetCateringFormById_Success()
        {
            var result = await _cateringService.GetCateringFormByIdAsync(_testCateringForm.Id);
            
            Assert.NotNull(result);
            Assert.Equal(_testCateringForm, result);
        }
        
        [Fact]
        public async Task GetCateringFormById_NotFound()
        {
            var result = await _cateringService.GetCateringFormByIdAsync(_testCateringForm.Id); //TODO: Fix me. Give a non valid ID
            
            Assert.Null(result); //TODO: This should return something to indicate that the form was not found. Will need to update the service to do so.
        }

        [Fact]
        public async Task GetCateringForms_AllFilters_Success()
        {
            var result = await _cateringService.GetCateringFormsAsync(_testClientId, Status.Confirmed, new DateTime(2025, 10, 7), new DateTime(2025, 10, 10));
            
            Assert.NotNull(result);
            Assert.Equal(_testCateringForm, result.First());
        }

        [Fact]
        public async Task GetCateringForms_AllFilters_NoFormsFound()
        {
            var result = await _cateringService.GetCateringFormsAsync(_testClientId, Status.Confirmed, new DateTime(2006, 10, 7), new DateTime(2007, 10, 10));
            
            Assert.Null(result);//TODO: This should return something to indicate that no forms were found. Will need to update the service to do so. 
        }

        [Fact]
        public async Task GetCateringFormsByClientId_Success()
        {
            var result = await _cateringService.GetCateringFormsAsync(_testCateringForm.Id, null, null, null);

            Assert.NotNull(result);
            Assert.Equal(4, result.Count());
            Assert.Equal(_testCateringForms, result);
        }

        [Fact]
        public async Task GetCateringFormsByClientId_ClientIdNotFound()
        {
            var result = await _cateringService.GetCateringFormsAsync(_testClientId, null, null, null); // TODO: Give a client id that doesnt' exist

            Assert.Null(result); //TODO: This should return something to indicate that the clientId was not found. Will need to update the service to do so. How is this different from no filters?
        }

        [Fact]
        public async Task GetCateringForms_NoFilters() // Do we want to allow this? What is the expected behaviour? 
        {
            var result = await _cateringService.GetCateringFormsAsync(null, null, null, null);
        }

        [Fact]
        public async Task GetCateringFormsByDateRange_Success()
        {
            //Arrange
            DateTime startDate = new DateTime(2025, 10, 7);
            DateTime endDate = new DateTime(2025, 10, 9);
            //Act
            var result = await _cateringService.GetCateringFormsAsync(null, null, startDate, endDate);

            Assert.NotNull(result);
            Assert.Equal([_testCateringForms[0], _testCateringForms[1], _testCateringForms[3]], result.ToList());
        }

        [Fact]
        public async Task GetCateringFormsByDateRange_InvalidRange()
        {
            //Arrange
            DateTime startDate = new DateTime(2023, 12, 31);
            DateTime endDate = new DateTime(2023, 1, 1);
            //Act
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _cateringService.GetCateringFormsAsync(null, null, startDate, endDate));
            //Assert
            Assert.Equal("Start date must be earlier than or equal to end date.", exception.Message);
        }

        [Fact]
        public async Task GetCateringFormsByDateRange_MissingStartOrEndDate()
        {
            //Arrange
            DateTime? startDate = null;
            DateTime endDate = new DateTime(2023, 12, 31);
            //Act
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _cateringService.GetCateringFormsAsync(null, null, startDate, endDate));
            //Assert
            Assert.Equal("Both start date and end date must be provided for date range filtering.", exception.Message);
        }

        [Fact]
        public async Task GetCateringFormsByStatus_Success()
        {
            var result = await _cateringService.GetCateringFormsAsync(null, Status.Confirmed, null, null);

            Assert.Equal(_testCateringForms[1], result.First());

            //TODO: Add tests for each status type if we want to be thorough.
        }

        [Fact]
        public async Task GetCateringFormsByStatus_InvalidStatus()
        {
            var result = await Assert.ThrowsAsync<ArgumentException>(() => _cateringService.GetCateringFormsAsync(null, (Status)999, null, null)); // TODO: I should figure out how to handle this more gracefully.
            Assert.Equal("Invalid status value.", result.Message);
        }

        [Fact]
        public async Task CreateCateringForm_Success()
        {

            var result = await _cateringService.CreateCateringFormAsync(_testCreateRequest);

            Assert.True(ValidateCateringForm(ConvertResponseToForm(result), _testCateringForm));
        }

        [Fact]
        public async Task CreateCateringForm_MissingRequiredFields()
        {
            var result = await Assert.ThrowsAsync<ArgumentException>(() => _cateringService.CreateCateringFormAsync(new CateringFormCreateRequest( //How would this gracefully handle null arguments/missing fields instead of throwing an exception? Can we use the Front End to guarantee these parameters are always provided?
                _testCateringForm.CateringType,
                [],
                _testCateringForm.ClientId,
                _testCateringForm.EventDate,
                _testCateringForm.ClientName,
                _testCateringForm.ClientEmail,
                _testCateringForm.ClientPhoneNumber,
                _testCateringForm.Status
            )));

            Assert.Equal("Missing required fields.", result.Message);
        }

        [Fact]
        public async Task CreateCateringForm_DuplicateSubmission()
        {
            var result = await Assert.ThrowsAsync<Exception>(() => _cateringService.CreateCateringFormAsync(new CateringFormCreateRequest(
                _testCateringForm.CateringType,
                _testCateringForm.DietaryRestrictions,
                _testCateringForm.ClientId,
                _testCateringForm.EventDate,
                _testCateringForm.ClientName,
                _testCateringForm.ClientEmail,
                _testCateringForm.ClientPhoneNumber,
                _testCateringForm.Status
            )));

            Assert.Equal("Duplicate submission found. Catering form not created", result.Message);
        }

        [Fact]
        public async Task UpdateCateringForm_Success()
        {
            var result = await _cateringService.UpdateCateringFormAsync(_testUpdateRequest);
            Assert.True(ValidateCateringForm(ConvertResponseToForm(result), _testCateringForm));
        }

        [Fact]
        public async Task UpdateCateringForm_InvalidId()
        {
            var result = await Assert.ThrowsAsync<ArgumentException>(() => _cateringService.UpdateCateringFormAsync(_testUpdateRequest));
            //TODO: Figure out how to handle this more gracefully. Currently throws an NRE instead of an ArgumentException. Also this should not throw an ArgumentException, but something more specific to invalid Id.
            //Assert.Equal("Catering form with the provided ID does not exist.", result.Message);
        }

        [Fact]
        public async Task UpdateCateringForm_InvalidData()
        {
            //TODO: Figure out how to handle this more gracefully. Currently throws an NRE instead of an ArgumentException. Also this should not throw an ArgumentException, but something more specific to invalid data.
            var result = await Assert.ThrowsAsync<ArgumentException>(() => _cateringService.UpdateCateringFormAsync(_testUpdateRequest));
            //Assert.Equal("Invalid data provided for update.", result.Message);
        }

        [Fact]
        public async Task UpdateCateringForm_TrivialUpdate()
        {
            var result = await Assert.ThrowsAsync<ArgumentException>(() => _cateringService.UpdateCateringFormAsync(_testUpdateRequest));
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
    }
}
