using Microsoft.AspNetCore.Mvc;
using TheChienHouse.Services;
using TheChienHouse.Models;
using static TheChienHouse.Models.ContactFormDTO;

namespace TheChienHouse.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactFormsController : ControllerBase
    {
        private readonly IContactFormService _contactFormService;

        public ContactFormsController(IContactFormService contactFormService)
        {
            _contactFormService = contactFormService;
        }

        //Get all forms by client id and/or within a date range
        //GET: api/ContactForms/forms?clientId={clientId}&startDate={startDate}&endDate={endDate}
        [HttpGet("forms")]
        public async Task<ActionResult<ContactFormsResponse>> GetContactForms([FromQuery] ContactFormsQueryRequest? request = null)
        {
            var forms = await _contactFormService.GetContactFormsAsync(request?.ClientId, request?.StartDate, request?.EndDate);
            
            return Ok(new ContactFormsResponse(forms));
        }

        //GET: api/ContactForms/form/{id}
        [HttpGet("form/{id}")]
        public async Task<ActionResult<ContactFormResponse>> GetContactForm([FromRoute] ContactFormRequest request)
        {
            ContactForm? form = await _contactFormService.GetContactFormByIdAsync(request.Id);
            if (form == null)
            {
                return NotFound();
            }

            return Ok(MapToResponse(form));
        }

        //POST: api/ContactForms
        [HttpPost]
        public async Task<ActionResult<ContactFormResponse>> CreateContactForm(ContactFormCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            ContactForm form = await _contactFormService.CreateContactFormAsync(request.ClientId, request.FirstName, request.LastName, request.ClientEmail, request.ClientPhoneNumber, request.Subject, request.Message);
            return CreatedAtAction(nameof(GetContactForm), MapToResponse(form));
        }

        //Delete contact form by id
        //DELETE: api/ContactForms/form/{id}
        [HttpDelete("form/{id}")]
        public async Task<IActionResult> DeleteContactForm([FromRoute] ContactFormRequest request)
        {
            bool isDeleted = await _contactFormService.DeleteContactFormAsync(request.Id);
            if (!isDeleted)
            {
                return NotFound(new { message = $"Contact form with ID '{request.Id}' was not found." });
            }
            return NoContent();
        }

        //Delete contact forms by client id
        //DELETE: api/ContactForms/client/{clientId}
        [HttpDelete("client/{clientId}")]
        public async Task<IActionResult> DeleteContactFormsByClientId([FromRoute] ContactFormsDeleteByClientIdRequest request)
        {
            bool isDeleted = await _contactFormService.DeleteContactFormsByClientIdAsync(request.ClientId);
            if (!isDeleted)
            {
                return NotFound(new { message = $"No contact forms found for client ID '{request.ClientId}'." });
            }
            return NoContent();
        }

        public ContactFormResponse MapToResponse(ContactForm contactForm)
        {
            return new ContactFormResponse(
                contactForm.Id,
                contactForm.ClientId,
                contactForm.FirstName,
                contactForm.LastName,
                contactForm.Email,
                contactForm.PhoneNumber,
                contactForm.Subject,
                contactForm.Message,
                contactForm.CreatedAt
            );
        }
    }
}
