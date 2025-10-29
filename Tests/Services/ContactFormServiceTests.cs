using Microsoft.EntityFrameworkCore;
using Moq;
using TheChienHouse.Models;
using TheChienHouse.Services;
using Xunit;
using static TheChienHouse.Models.ContactFormDTO;

namespace TheChienHouse.Tests.Services
{
    public class ContactFormServiceTests
    {
        private readonly ContactFormService _contactFormsService;
        private readonly RetailContext _context;
        private readonly Mock<ILogger<ContactFormService>> _mockLogger;
        //TODO: Contactually I should migrate these to a test database. 
        private static readonly Guid _testClientId = Guid.NewGuid();
        private static readonly ContactForm _testContactForm = new ContactForm
        {
            Id = Guid.NewGuid(),
            FirstName = "Kennedy",
            LastName = "Irving",
            Email = "Test@Test.com",
            PhoneNumber = "1234567890",
            Subject = "TestSubject",
            Message = "TestMessage",
            ClientId = _testClientId,
            CreatedAt = DateTime.UtcNow
        };
        private static readonly List<ContactForm> _testContactForms = new List<ContactForm>
        {
            new ContactForm { Id = Guid.NewGuid(), FirstName = "Marcus", LastName = "Chien", Email = "Test@TheChienHouse.com", PhoneNumber = "2066693774", Subject = "Hello World", Message = "I like yummy food", ClientId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow},
            new ContactForm { Id = Guid.NewGuid(), FirstName = "Jack", LastName = "Williams", Email = "Test@TheChienHouse.com", PhoneNumber = "2066693774", Subject = "Hello World", Message = "I like yummy food", ClientId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow},
            new ContactForm { Id = Guid.NewGuid(), FirstName = "Jack", LastName = "Naylor", Email = "Test@TheChienHouse.com", PhoneNumber = "2066693774", Subject = "Hello World", Message = "I like yummy food", ClientId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow},
            new ContactForm { Id = Guid.NewGuid(), FirstName = "Kennedy", LastName = "Irving", Email = "Test@TheChienHouse.com", PhoneNumber = "2066693774", Subject = "Hello World", Message = "I like yummy food", ClientId = _testClientId, CreatedAt = DateTime.UtcNow.AddDays(-2)},
        };
        private static readonly ContactFormCreateRequest _testCreateRequest = new ContactFormCreateRequest(
            _testClientId,
            _testContactForm.FirstName,
            _testContactForm.LastName,
            _testContactForm.Email,
            _testContactForm.PhoneNumber,
            _testContactForm.Subject,
            _testContactForm.Message
        );

        public ContactFormServiceTests() //TODO: Figure out how to populate DB context with test data just ONCE so that duplicate items aren't trying to be added. 
        {
            var options = new DbContextOptionsBuilder<RetailContext>()
                .UseInMemoryDatabase(databaseName: $"ContactFormTestDb_{Guid.NewGuid()}")
                .Options;
            _context = new RetailContext(options);
            _mockLogger = new Mock<ILogger<ContactFormService>>();
            _contactFormsService = new ContactFormService(_context, _mockLogger.Object);

            foreach (ContactForm contactForm in _testContactForms)
            {
                _context.ContactForms.Add(contactForm);
            }
            _context.ContactForms.Add(_testContactForm);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetContactFormById_Success()
        {
            var result = await _contactFormsService.GetContactFormByIdAsync(_testContactForm.Id);
            Assert.NotNull(result);
            Assert.Equal(_testContactForm, result);
        }

        [Fact]
        public async Task GetContactForms_ByClientId_Success()
        {
            var result = await _contactFormsService.GetContactFormsAsync(_testClientId, null, null);
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal(new List<ContactForm>([_testContactForms[3],_testContactForm]), result);
        }

        [Fact]
        public async Task GetContactForms_ByDateRange_Success()
        {
            DateTime startDate = DateTime.UtcNow.AddDays(-1);
            DateTime endDate = DateTime.UtcNow.AddDays(1);
            var result = await _contactFormsService.GetContactFormsAsync(null, startDate, endDate);
            Assert.NotNull(result);
            Assert.Equal(4, result.Count());
        }

        [Fact]
        public async Task CreateContactForm_Success()
        {
            var result = await _contactFormsService.CreateContactFormAsync(_testCreateRequest.ClientId,_testCreateRequest.FirstName,_testCreateRequest.LastName,_testCreateRequest.ClientEmail,_testCreateRequest.ClientPhoneNumber,_testCreateRequest.Subject,_testCreateRequest.Message);
            Assert.NotNull(result);
            Assert.True(ValidateContactForm(_testContactForm, result));
        }

        [Fact]
        public async Task DeleteContactForm_Success()
        {
            //TODO: Validate forms exist before deletion
            var result = await _contactFormsService.DeleteContactFormAsync(_testContactForm.Id);
            Assert.True(result);
            var deletedForm = await _contactFormsService.GetContactFormByIdAsync(_testContactForm.Id);
            Assert.Null(deletedForm);
        }

        [Fact]
        public async Task DeleteContactFormsByClientId_Success()
        {
            //TODO: Validate forms exist before deletion
            var result = await _contactFormsService.DeleteContactFormsByClientIdAsync(_testClientId);
            Assert.True(result);
            var deletedForms = await _contactFormsService.GetContactFormsAsync(_testClientId, null, null);
            Assert.Empty(deletedForms);
        }

        //TODO: Write more tests for failure cases and other scenarios. Follow EventFormServiceTests pattern.

        public bool ValidateContactForm(ContactForm expected, ContactForm result)
        {
            return expected.ClientId == result.ClientId &&
                   expected.FirstName == result.FirstName &&
                   expected.LastName == result.LastName &&
                   expected.Email == result.Email &&
                   expected.PhoneNumber == result.PhoneNumber &&
                   expected.Subject == result.Subject &&
                   expected.Message == result.Message;
        }
    }
}
