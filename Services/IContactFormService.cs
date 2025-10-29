using TheChienHouse.Models;
using static TheChienHouse.Models.ContactFormDTO;

namespace TheChienHouse.Services
{
    public interface IContactFormService
    {
        Task<ContactForm> CreateContactFormAsync(Guid? clientId, string firstName, string? lastName, string email, string? phoneNumber, string subject, string message);
        Task<bool> DeleteContactFormAsync(Guid id);
        Task<bool> DeleteContactFormsByClientIdAsync(Guid clientId);
        Task<ContactForm?> GetContactFormByIdAsync(Guid id);
        Task<IEnumerable<ContactForm>> GetContactFormsAsync(Guid? clientId, DateTime? startDate, DateTime? endDate);
    }
}
