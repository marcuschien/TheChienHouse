using TheChienHouse.Models;
using static TheChienHouse.Models.ContactFormDTO;

namespace TheChienHouse.Services
{
    public interface IContactFormService
    {
        Task<ContactFormCreateResponse> CreateContactFormAsync(ContactFormCreateRequest request);
        Task<bool> DeleteContactFormAsync(Guid id);
        Task<bool> DeleteContactFormsByClientIdAsync(Guid clientId);
        Task<ContactForm?> GetContactFormByIdAsync(Guid id);
        Task<IEnumerable<ContactForm>> GetContactFormsAsync(Guid? clientId, DateTime? startDate, DateTime? endDate);
    }
}
