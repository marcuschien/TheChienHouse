using System.ComponentModel.DataAnnotations;

namespace TheChienHouse.Models
{
    public class CateringFormDTO
    {
        public record CateringFormCreateRequest
        (
            CateringType CateringType,
            List<DietaryRestrictions> DietaryRestrictions,
            Guid? ClientId,
            DateTime EventDate,
            string ClientName,
            string? ClientEmail,
            string? ClientPhoneNumber,
            Status Status
        );
        public record CateringFormCreateResponse( // These will be used as part of the success confirmation pop up after submission.
            Guid Id,
            CateringType CateringType,
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
        public record CateringFormUpdateRequest
        (
            Guid Id,
            CateringType CateringType,
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
