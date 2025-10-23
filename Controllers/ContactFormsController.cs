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
        //GET: api/ContactForms?clientId={clientId}&startDate={startDate}&endDate={endDate}
        [HttpGet("forms/{clientId}&{startDate}&{endDate}")]
        public async Task<ActionResult<IEnumerable<ContactForm>>> GetContactForms(Guid? clientId = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            IEnumerable<ContactForm> response = await _contactFormService.GetContactFormsAsync(clientId, startDate, endDate);
            return CreatedAtAction(nameof(GetContactForms), response);
        }

        //GET: api/ContactForms/{id}
        [HttpGet("form/{id}")]
        public async Task<ActionResult<ContactForm>> GetContactForm(Guid id)
        {
            ContactForm? response = await _contactFormService.GetContactFormByIdAsync(id);
            if (response == null)
            {
                return NotFound();
            }
            return CreatedAtAction(nameof(GetContactForm), response);
        }

        //POST: api/ContactForms
        [HttpPost]
        public async Task<ActionResult<ContactForm>> CreateContactForm(ContactFormCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            ContactFormCreateResponse response = await _contactFormService.CreateContactFormAsync(request);
            return CreatedAtAction(nameof(GetContactForm), new { id = response.Id }, response);
        }

        //Delete contact form by id
        //DELETE: api/ContactForms/{id}
        [HttpDelete("form/{id}")]
        public async Task<IActionResult> DeleteContactForm(Guid id)
        {
            bool isDeleted = await _contactFormService.DeleteContactFormAsync(id);
            if (!isDeleted)
            {
                return NotFound(new { message = $"Contact form with ID '{id}' was not found." });
            }
            return NoContent();
        }

        //Delete contact forms by client id
        //DELETE: api/ContactForms/client/{clientId}
        [HttpDelete("client/{clientId}")]
        public async Task<IActionResult> DeleteContactFormsByClientId(Guid clientId)
        {
            bool isDeleted = await _contactFormService.DeleteContactFormsByClientIdAsync(clientId);
            if (!isDeleted)
            {
                return NotFound(new { message = $"No contact forms found for client ID '{clientId}'." });
            }
            return NoContent();
        }
    }
}
