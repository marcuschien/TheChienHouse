using Microsoft.EntityFrameworkCore;
using TheChienHouse.Models;
using static TheChienHouse.Models.ContactFormDTO;

namespace TheChienHouse.Services
{
    public class ContactFormService : IContactFormService
    {
        private readonly RetailContext _context;
        private readonly ILogger<ContactFormService> _logger;

        public ContactFormService(RetailContext context, ILogger<ContactFormService> logger) 
        {
            _context = context;
            _logger = logger;
        }
        public async Task<ContactForm> CreateContactFormAsync(Guid? clientId, string firstName, string? lastName, string email, string? phoneNumber, string subject, string message)
        {
            ContactForm contactForm = new ContactForm
            {
                Id = Guid.NewGuid(),
                ClientId = clientId,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = phoneNumber,
                Subject = subject,
                Message = message,
                CreatedAt = DateTime.UtcNow
            };
            _context.ContactForms.Add(contactForm);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Created new contact form with ID: {ContactFormId}", contactForm.Id);

            return contactForm;
        }

        public async Task<bool> DeleteContactFormAsync(Guid id)
        {
            ContactForm? form = await _context.FindAsync<ContactForm>(id);
            if (form == null)
            {
                _logger.LogWarning("Contact form with ID: {ContactFormId} not found for deletion", id);
                return false;
            }
            else
            {
                _context.ContactForms.Remove(form);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Deleted contact form with ID: {ContactFormId}", id);
                return true;
            }
        }

        public async Task<bool> DeleteContactFormsByClientIdAsync(Guid clientId)
        {
            IEnumerable<ContactForm> forms = _context.ContactForms.Where(cf => cf.ClientId == clientId);
            if (!forms.Any())
            {
                _logger.LogWarning("No contact forms found for client ID: {ClientId} to delete", clientId);
                return false;
            }
            else
            {
                _context.ContactForms.RemoveRange(forms);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Deleted contact forms for client ID: {ClientId}", clientId);
                return true;
            }
        }

        public async Task<ContactForm?> GetContactFormByIdAsync(Guid id)
        {
            ContactForm? form = await _context.FindAsync<ContactForm>(id);
            if (form == null) {
                _logger.LogWarning("Contact form with ID: {ContactFormId} not found", id);
                return null;
            }
            else
            {
                return form;
            }
        }

        public async Task<IEnumerable<ContactForm>> GetContactFormsAsync(Guid? clientId, DateTime? startDate, DateTime? endDate)
        {
            //Optimization?: Would it be better to filter at the database level instead of in-memory? i.e. write a different query for each filter type? It would certainly mean we're not fetching the entire forms table every time.
            IEnumerable<ContactForm> contactForms = await _context.ContactForms.ToListAsync();
            if (clientId.HasValue)
            {
                contactForms = contactForms.Where(cf => cf.ClientId == clientId);
            }
            if (startDate.HasValue || endDate.HasValue)
            {
                if (!startDate.HasValue || !endDate.HasValue)
                {
                    throw new ArgumentException("Both start date and end date must be provided for date range filtering.");
                }
                if (startDate > endDate)
                {
                    throw new ArgumentException("Start date must be earlier than or equal to end date.");
                }
                contactForms = contactForms.Where(cf => cf.CreatedAt >= startDate && cf.CreatedAt <= endDate);
            }
            return contactForms;
        }
    }
}
