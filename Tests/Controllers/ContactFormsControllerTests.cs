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
        private static readonly ContactFormResponse _testResponse = new ContactFormResponse(
            _testForm.Id,
            _testClientId,
            _testForm.FirstName,
            _testForm.LastName,
            _testForm.Email,
            _testForm.PhoneNumber,
            _testForm.Subject,
            _testForm.Message,
            _testForm.CreatedAt);
        private static readonly ContactFormCreateRequest _testCreateRequest = new ContactFormCreateRequest(
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
            _controller = new ContactFormsController(_mockContactFormService.Object);
        }

        [Fact]
        public async Task GetContactFormById_Success()
        {
            _mockContactFormService.Setup(service => service.GetContactFormByIdAsync(_testClientId)).ReturnsAsync(_testForm);

            var result = await _controller.GetContactForm(new ContactFormRequest(_testClientId));

            var actionResult = Assert.IsType<ActionResult<ContactFormResponse>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<ContactFormResponse>(okResult.Value);
            Assert.Equal(_testForm.Id, returnValue.Id);
            Assert.Equal(_testForm.ClientId, returnValue.ClientId);
            Assert.Equal(_testForm.FirstName, returnValue.FirstName);
            Assert.Equal(_testForm.LastName, returnValue.LastName);
            Assert.Equal(_testForm.Email, returnValue.Email);
            Assert.Equal(_testForm.PhoneNumber, returnValue.PhoneNumber);
            Assert.Equal(_testForm.Subject, returnValue.Subject);
            Assert.Equal(_testForm.Message, returnValue.Message);
        }

        [Fact]
        public async Task GetContactForms_ByClientId_Success()
        {
            _mockContactFormService.Setup(service => service.GetContactFormsAsync(_testClientId, null, null)).ReturnsAsync(_testForms);
            var result = await _controller.GetContactForms(new ContactFormsQueryRequest(_testClientId, null, null));
            var actionResult = Assert.IsType<ActionResult<ContactFormsResponse>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<ContactFormsResponse>(okResult.Value);
            var forms = returnValue.Forms.ToList();
            Assert.Equal(_testForms.Count, forms.Count);
            for (int i = 0; i < forms.Count; i++)
            {
                Assert.Equal(_testForms[i].Id, forms[i].Id);
            }
        }

        [Fact]
        public async Task GetContactForms_ByDateRange_Success()
        {
            DateTime startDate = DateTime.UtcNow.AddDays(-10);
            DateTime endDate = DateTime.UtcNow;
            _mockContactFormService.Setup(service => service.GetContactFormsAsync(null, startDate, endDate)).ReturnsAsync(_testForms);
            var result = await _controller.GetContactForms(new ContactFormsQueryRequest(null, startDate, endDate));
            var actionResult = Assert.IsType<ActionResult<ContactFormsResponse>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<ContactFormsResponse>(okResult.Value);
            var forms = returnValue.Forms.ToList();
            Assert.Equal(_testForms.Count, forms.Count);
        }

        [Fact]
        public async Task CreateContactForm_Success()
        {
            _mockContactFormService.Setup(service => service.CreateContactFormAsync(_testCreateRequest.ClientId,_testCreateRequest.FirstName,_testCreateRequest.LastName,_testCreateRequest.ClientEmail,_testCreateRequest.ClientPhoneNumber,_testCreateRequest.Subject,_testCreateRequest.Message)).ReturnsAsync(_testForm);
            var result = await _controller.CreateContactForm(_testCreateRequest);
            var actionResult = Assert.IsType<ActionResult<ContactFormResponse>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var returnValue = Assert.IsType<ContactFormResponse>(createdAtActionResult.Value);
            Assert.Equal(_testResponse, returnValue);
        }

        [Fact]
        public async Task DeleteContactForm_Success()
        {
            //TODO: Figure out how to validate the form was deleted
            _mockContactFormService.Setup(service => service.DeleteContactFormAsync(_testClientId)).ReturnsAsync(true);
            var result = await _controller.DeleteContactForm(new ContactFormRequest(_testClientId));
            Assert.IsType<NoContentResult>(result);
        }
        [Fact]
        public async Task DeleteContactForm_NotFound()
        {
            _mockContactFormService.Setup(service => service.DeleteContactFormAsync(_testClientId)).ReturnsAsync(false);
            var result = await _controller.DeleteContactForm(new ContactFormRequest(_testClientId));
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task DeleteContactFormsByClientId_Success()
        {
            //TODO: Figure out how to validate the forms were deleted
            _mockContactFormService.Setup(service => service.DeleteContactFormsByClientIdAsync(_testClientId)).ReturnsAsync(true);
            var result = await _controller.DeleteContactFormsByClientId(new ContactFormsDeleteByClientIdRequest(_testClientId));
            Assert.IsType<NoContentResult>(result);
        }
        
        //TODO: Write some failure tests. Break your code. 
    }
}