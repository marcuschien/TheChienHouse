using Microsoft.EntityFrameworkCore;
using TheChienHouse.Models;
using static TheChienHouse.Models.EventFormDTO;

namespace TheChienHouse.Services
{
    public class EventFormService : IEventFormService
    {
        private readonly RetailContext _context;
        private readonly ILogger<EventFormService> _logger;

        public EventFormService(RetailContext context, ILogger<EventFormService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<EventForm> CreateEventFormAsync(EventType eventType, List<DietaryRestrictions> dietaryRestrictions, Guid? clientId, DateTime eventDate, string firstName, string? lastName, Status status, string location, string email, string? phoneNumber, decimal budgetPP, int numGuests, string? notes)
        {
            var newEventForm = new EventForm
            {
                Id = Guid.NewGuid(),
                EventType = eventType,
                DietaryRestrictions = dietaryRestrictions,
                ClientId = clientId,
                EventDate = eventDate,
                FirstName = firstName,
                LastName = lastName,
                ClientEmail = email,
                ClientPhoneNumber = phoneNumber,
                Status = status,
                CreatedAt = DateTime.UtcNow,
                Location = location,
                BudgetPerPerson = budgetPP,
                NumberOfGuests =numGuests,
                ExtraNotes = notes
            };
            _context.EventForms.Add(newEventForm);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Created new event form with ID: {EventFormId}", newEventForm.Id);

            return newEventForm;
        }

        public async Task<bool> DeleteEventFormAsync(Guid id)
        {
            EventForm? form = await _context.EventForms.FindAsync(id);
            if (form == null)
            {
                _logger.LogWarning("Event form with ID: {EventFormId} not found for deletion", id);
                return false;
            }
            else
            {
                _context.EventForms.Remove(form);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Deleted event form with ID: {EventFormId}", id);
                return true;
            }
        }

        public async Task<EventForm?> GetEventFormByIdAsync(Guid id)
        {
            EventForm? form = await _context.EventForms.FindAsync(id);
            if (form == null)
            {
                _logger.LogWarning("Event form with ID: {EventFormId} not found", id);
                return null;
            }
            else
            {
                return form;
            }
        }

        public async Task<IEnumerable<EventForm>> GetEventFormsAsync(Guid? clientId, Status? status, DateTime? startDate, DateTime? endDate) // Why do I want this to be async?
        {
            IEnumerable<EventForm> eventForms = await _context.EventForms.ToListAsync();
            if (clientId.HasValue)
            {
                eventForms = eventForms.Where(cf => cf.ClientId == clientId); //TODO: Fix me. I'm not filtering properly.
            }
            if (status.HasValue)
            {
                eventForms = eventForms.Where(cf => cf.Status == status);
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
                eventForms = eventForms.Where(cf => cf.EventDate >= startDate && cf.EventDate <= endDate);
            }
            return eventForms;
            //Optimization?: Would it be better to filter at the database level instead of in-memory? i.e. write a different query for each filter type? It would certainly mean we're not fetching the entire forms table every time.
        }

        public async Task<EventForm?> UpdateEventFormAsync(Guid formId, EventType eventType, List<DietaryRestrictions> dietaryRestrictions, Guid? clientId, DateTime eventDate, string firstName, string? lastName, Status status, string location, string email, string? phoneNumber, decimal budgetPP, int numGuests, string? notes)
        {
            EventForm? form = await _context.EventForms.FindAsync(formId);
            if (form == null)
            {
                _logger.LogWarning("Event form with ID: {EventFormId} not found for update", formId);
                return null;
            }
            else
            {
                form.EventType = eventType;
                form.DietaryRestrictions = dietaryRestrictions;
                form.ClientId = clientId;
                form.EventDate = eventDate;
                form.FirstName = firstName;
                form.LastName = lastName;
                form.ClientEmail = email;
                form.ClientPhoneNumber = phoneNumber;
                form.Status = status;
                form.UpdatedAt = DateTime.UtcNow;
                form.Location = location;
                form.BudgetPerPerson = budgetPP;
                form.NumberOfGuests = numGuests;
                form.ExtraNotes = notes;
                _context.EventForms.Update(form);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Updated event form with ID: {EventFormId}", formId);
                return form;
            }
        }

        
    }
}
