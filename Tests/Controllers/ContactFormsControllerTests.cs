using Microsoft.AspNetCore.Mvc;
using Moq;
using TheChienHouse.Controllers;
using TheChienHouse.Models;
using TheChienHouse.Services;
using Xunit;
using static TheChienHouse.Models.ContactFormDTO;

namespace TheChienHouse.Tests.Controllers
{
    public class ContactFormsControllerTests
    {
        private readonly Mock<IContactFormService> _mockContactFormService;
        private readonly ContactFormsController _controller;
        //TODO: Eventually I should migrate these to a test database seeding strategy.
        private static Guid _testClientId = Guid.NewGuid();
        private static readonly ContactForm _testForm = new ContactForm()
        {
            Id = Guid.NewGuid(),
            ClientId = _testClientId,
            FirstName = "TestFirstName",
            LastName = "TestLastName",
            Email = "TestEmail@Email.com",
            PhoneNumber = "IShouldDoACheckToMakeSureThisIsAllNumbers",
            Subject = "TestSubject",
            Message = "TestMessage"
        };
        private static readonly List<ContactForm> _testForms = new List<ContactForm>()
        {
            new ContactForm() { Id = Guid.NewGuid(), ClientId = _testClientId, FirstName = "TestName1", LastName = "TestLastName", Email = "TestEmail1@Email.com", PhoneNumber = "IShouldDoACheckToMakeSureThisIsAllNumbers", Subject = "TestSubject1", Message = "TestMessage1"},
            new ContactForm() { Id = Guid.NewGuid(), ClientId = _testClientId, FirstName = "TestName2", LastName = "TestLastName", Email = "TestEmail2@Email.com", PhoneNumber = "IShouldDoACheckToMakeSureThisIsAllNumbers", Subject = "TestSubject2", Message = "TestMessage2" },
            new ContactForm() { Id = Guid.NewGuid(), ClientId = _testClientId, FirstName = "TestName3", LastName = "TestLastName", Email = "TestEmail3@Email.com", PhoneNumber = "IShouldDoACheckToMakeSureThisIsAllNumbers", Subject = "TestSubject3", Message = "TestMessage3"},
            new ContactForm() { Id = Guid.NewGuid(), ClientId = _testClientId, FirstName = "TestName4", LastName = "TestLastName", Email = "TestEmail4@Email.com", PhoneNumber = "IShouldDoACheckToMakeSureThisIsAllNumbers", Subject = "TestSubject4", Message = "TestMessage4"},
        };
        private static readonly ContactFormCreateResponse _testResponse = new ContactFormCreateResponse(
            _testForm.Id,
            _testClientId,
            _testForm.FirstName,
            _testForm.LastName,
            _testForm.Email,
            _testForm.PhoneNumber,
            _testForm.Subject,
            _testForm.Message,
            _testForm.CreatedAt);
        private static readonly ContactFormCreateRequest _testRequest = new ContactFormCreateRequest(
            _testClientId,
            _testForm.FirstName,
            _testForm.LastName,
            _testForm.Email,
            _testForm.PhoneNumber,
            _testForm.Subject,
            _testForm.Message);

        public ContactFormsControllerTests()
        {
            _mockContactFormService = new Mock<IContactFormService>();
            _controller = new ContactFormsController( _mockContactFormService.Object );
        }

        [Fact]
        public async Task GetContactFormById_Success()
        {
            _mockContactFormService.Setup(service => service.GetContactFormByIdAsync(_testClientId)).ReturnsAsync(_testForm);

            var result = await _controller.GetContactForm(_testClientId);

            var actionResult = Assert.IsType<ActionResult<ContactForm>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var returnValue = Assert.IsType<ContactForm>(createdAtActionResult.Value);
            Assert.Equal(_testForm, returnValue);
        }
    }
}
