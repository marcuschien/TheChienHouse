using TheChienHouse.Models;
using static TheChienHouse.Models.EventFormDTO;

namespace TheChienHouse.Services
{
    public interface IEventFormService
    {
        Task<EventFormCreateResponse> CreateEventFormAsync(EventFormCreateRequest request);
        Task<EventForm?> GetEventFormByIdAsync(Guid id);
        Task<EventFormCreateResponse?> UpdateEventFormAsync(EventFormUpdateRequest request);
        Task<bool> DeleteEventFormAsync(Guid id);
        Task<IEnumerable<EventForm>> GetEventFormsAsync(Guid? clientId = null, Status? status = null, DateTime? startDate = null, DateTime? endDate = null);
    }
}
