using TheChienHouse.Models;
using static TheChienHouse.Models.EventFormDTO;

namespace TheChienHouse.Services
{
    public interface IEventFormService
    {
        Task<EventForm> CreateEventFormAsync(EventType eventType, List<DietaryRestrictions> dietaryRestrictions, Guid? clientId, DateTime eventDate, string firstName, string? lastName, Status status, string location, string email, string? phoneNumber, decimal budgetPP, int numGuests, string? notes);
        Task<EventForm?> GetEventFormByIdAsync(Guid id);
        Task<EventForm?> UpdateEventFormAsync(Guid formId, EventType eventType, List<DietaryRestrictions> dietaryRestrictions, Guid? clientId, DateTime eventDate, string firstName, string? lastName, Status status, string location, string email, string? phoneNumber, decimal budgetPP, int numGuests, string? notes);
        Task<bool> DeleteEventFormAsync(Guid id);
        Task<IEnumerable<EventForm>> GetEventFormsAsync(Guid? clientId = null, Status? status = null, DateTime? startDate = null, DateTime? endDate = null);
    }
}
