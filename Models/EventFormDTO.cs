using System.ComponentModel.DataAnnotations;

namespace TheChienHouse.Models
{
    public class EventFormDTO
    {
        public record EventFormCreateRequest
        (
            EventType EventType,
            List<DietaryRestrictions> DietaryRestrictions,
            Guid? ClientId,
            DateTime EventDate,
            string ClientName,
            string? ClientEmail,
            string? ClientPhoneNumber,
            Status Status
        );
        public record EventFormCreateResponse( // These will be used as part of the success confirmation pop up after submission.
            Guid Id,
            EventType EventType,
            List<DietaryRestrictions> DietaryRestrictions,
            Guid? ClientId,
            DateTime EventDate,
            string ClientName,
            string? ClientEmail,
            string? ClientPhoneNumber,
            Status Status,
            DateTime CreatedAt,
            DateTime? UpdatedAt
        );
        public record EventFormUpdateRequest
        (
            Guid Id,
            EventType EventType,
            List<DietaryRestrictions> DietaryRestrictions,
            Guid? ClientId,
            DateTime EventDate,
            string ClientName,
            string? ClientEmail,
            string? ClientPhoneNumber,
            Status Status
        );
    }
}
