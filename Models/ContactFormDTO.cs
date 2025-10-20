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
        public record ContactFormCreateResponse(
            Guid Id,
            Guid? ClientId,
            string FirstName,
            string? LastName,
            string ClientEmail,
            string? ClientPhoneNumber,
            string Subject,
            string Message,
            DateTime CreatedAt
        );
    }
}
