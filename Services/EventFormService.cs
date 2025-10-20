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
        public async Task<EventFormCreateResponse> CreateEventFormAsync(EventFormCreateRequest request)
        {
            var newEventForm = new EventForm
            {
                Id = Guid.NewGuid(),
                EventType = request.EventType,
                DietaryRestrictions = request.DietaryRestrictions,
                ClientId = request.ClientId,
                EventDate = request.EventDate,
                FirstName = request.FirstName,
                LastName = request.LastName,
                ClientEmail = request.ClientEmail,
                ClientPhoneNumber = request.ClientPhoneNumber,
                Status = request.Status,
                CreatedAt = DateTime.UtcNow,
                Location = request.Location,
                BudgetPerPerson = request.BudgetPerPerson,
                NumberOfGuests = request.NumberOfGuests,
                ExtraNotes = request.ExtraNotes
            };
            _context.EventForms.Add(newEventForm);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Created new event form with ID: {EventFormId}", newEventForm.Id);

            return MapToResponse(newEventForm);
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
            List<EventForm> eventForms = await _context.EventForms.ToListAsync();
            if (clientId.HasValue)
            {
                eventForms = (List<EventForm>)eventForms.Where(cf => cf.ClientId == clientId);
            }
            if (status.HasValue)
            {
                eventForms = (List<EventForm>)eventForms.Where(cf => cf.Status == status);
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
                eventForms = (List<EventForm>)eventForms.Where(cf => cf.EventDate >= startDate && cf.EventDate <= endDate);
            }
            return eventForms;
            //Optimization?: Would it be better to filter at the database level instead of in-memory? i.e. write a different query for each filter type?
        }

        public async Task<EventFormCreateResponse?> UpdateEventFormAsync(EventFormUpdateRequest request)
        {
            EventForm? form = await _context.EventForms.FindAsync(request.Id);
            if (form == null)
            {
                _logger.LogWarning("Event form with ID: {EventFormId} not found for update", request.Id);
                return null;
            }
            else
            {
                form.EventType = request.EventType;
                form.DietaryRestrictions = request.DietaryRestrictions;
                form.ClientId = request.ClientId;
                form.EventDate = request.EventDate;
                form.FirstName = request.FirstName;
                form.LastName = request.LastName;
                form.ClientEmail = request.ClientEmail;
                form.ClientPhoneNumber = request.ClientPhoneNumber;
                form.Status = request.Status;
                form.UpdatedAt = DateTime.UtcNow;
                _context.EventForms.Update(form);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Updated event form with ID: {EventFormId}", request.Id);
                return MapToResponse(form);
            }
        }

        private static EventFormCreateResponse MapToResponse(EventForm eventForm) => new(eventForm.Id, eventForm.EventType, eventForm.DietaryRestrictions, eventForm.ClientId, eventForm.EventDate, eventForm.FirstName, eventForm.LastName, eventForm.ClientEmail, eventForm.ClientPhoneNumber, eventForm.Status, eventForm.CreatedAt, eventForm.UpdatedAt, eventForm.Location, eventForm.BudgetPerPerson, eventForm.NumberOfGuests, eventForm.ExtraNotes);
    }
}
