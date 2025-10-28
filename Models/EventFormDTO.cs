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
            string FirstName,
            string? LastName,
            string ClientEmail,
            string? ClientPhoneNumber,
            Status Status,
            string Location,
            decimal BudgetPerPerson,
            int NumberOfGuests,
            string? ExtraNotes
        );
        public record EventFormUpdateRequest
        (
            Guid Id,
            EventType EventType,
            List<DietaryRestrictions> DietaryRestrictions,
            Guid? ClientId,
            DateTime EventDate,
            string FirstName,
            string? LastName,
            string ClientEmail,
            string? ClientPhoneNumber,
            Status Status,
            string Location,
            decimal BudgetPerPerson,
            int NumberOfGuests,
            string? ExtraNotes
        );
        public record EventFormCreateResponse( // These will be used as part of the success confirmation pop up after submission.
            Guid Id,
            EventType EventType,
            List<DietaryRestrictions> DietaryRestrictions,
            Guid? ClientId,
            DateTime EventDate,
            string FirstName,
            string? LastName,
            string ClientEmail,
            string? ClientPhoneNumber,
            Status Status,
            DateTime CreatedAt,
            DateTime? UpdatedAt,
            string Location,
            decimal BudgetPerPerson,
            int NumberOfGuests,
            string? ExtraNotes
        );
        
        public record EventFormRequest(
            Guid? Id = null,
            Guid? ClientId = null,
            Status? Status = null,
            DateTime? StartDate = null,
            DateTime? EndDate = null
        );

        public record EventFormResponse(
            Guid Id,
            EventType EventType,
            List<DietaryRestrictions> DietaryRestrictions,
            Guid? ClientId,
            DateTime EventDate,
            string FirstName,
            string? LastName,
            string ClientEmail,
            string? ClientPhoneNumber,
            Status Status,
            string Location,
            decimal BudgetPerPerson,
            int NumberOfGuests,
            string? ExtraNotes,
            DateTime CreatedAt,
            DateTime? UpdatedAt
        );

        public record EventFormsResponse(
            IEnumerable<EventForm> EventForms
        );
    }
}
