using System.ComponentModel.DataAnnotations;

namespace TheChienHouse.Models
{
    public class CateringFormDTO
    {
        public record CateringFormCreateRequest
        {
            public Guid Id { get; set; }
            public CateringType CateringType { get; set; }
            public List<DietaryRestrictions> MenuItems { get; set; } = new List<DietaryRestrictions>();
            public Guid? ClientId { get; set; }
            [Required(ErrorMessage = "Event date is required")]
            public DateTime EventDate { get; set; }
            [Required(ErrorMessage = "Name is required")]
            public required string ClientName { get; set; }
            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid email address")]
            public string? ClientEmail { get; set; }
            [Required(ErrorMessage = "Phone number is required")]
            [Phone(ErrorMessage = "Invalid phone number")]
            public string? ClientPhoneNumber { get; set; }
            public Status Status { get; set; } = Status.Pending;
            public DateTime CreatedAt { get; init; }
            public DateTime? UpdatedAt { get; init; }

        }
        public record CateringFormCreateResponse
        {
            public Guid Id { get; set; }
            public CateringType CateringType { get; set; }
            public List<DietaryRestrictions> MenuItems { get; set; } = new List<DietaryRestrictions>();
            public Guid? ClientId { get; set; }
            [Required(ErrorMessage = "Event date is required")]
            public DateTime EventDate { get; set; }
            [Required(ErrorMessage = "Name is required")]
            public required string ClientName { get; set; }
            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid email address")]
            public string? ClientEmail { get; set; }
            [Required(ErrorMessage = "Phone number is required")]
            [Phone(ErrorMessage = "Invalid phone number")]
            public string? ClientPhoneNumber { get; set; }
            public Status Status { get; set; } = Status.Pending;
            public DateTime CreatedAt { get; init; }
            public DateTime? UpdatedAt { get; init; }
        }
    }
}
