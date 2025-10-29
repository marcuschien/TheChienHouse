namespace TheChienHouse.Models
{
    public class ContactFormDTO
    {
        public record ContactFormCreateRequest
        (
            Guid? ClientId,
            string FirstName,
            string? LastName,
            string ClientEmail,
            string? ClientPhoneNumber,
            string Subject,
            string Message
        );

        public record ContactFormResponse(
            Guid Id,
            Guid? ClientId,
            string FirstName,
            string? LastName,
            string Email,
            string? PhoneNumber,
            string Subject,
            string Message,
            DateTime CreatedAt
        );

        public record ContactFormsResponse(IEnumerable<ContactForm> Forms);

        public record ContactFormRequest(Guid Id);

        public record ContactFormsQueryRequest(Guid? ClientId, DateTime? StartDate, DateTime? EndDate);

        public record ContactFormsDeleteByClientIdRequest(Guid ClientId);
    }
}
