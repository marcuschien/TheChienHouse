using System.ComponentModel.DataAnnotations;

namespace TheChienHouse.Models
{
    public class EventForm
    {
        [Required(ErrorMessage = "EventType is required")]
        public EventType EventType { get; set; }
        [Required(ErrorMessage = "DietaryRestrictions is required")]
        public List<DietaryRestrictions> DietaryRestrictions { get; set; } = new List<DietaryRestrictions>();
        [Required(ErrorMessage = "Event Date is required")]
        public DateTime EventDate { get; set; }
        [Required(ErrorMessage = "First name is required")]
        public required string FirstName { get; set; }
        public string? LastName { get; set; }
        [Required(ErrorMessage = "EventForm Status is required")]
        public Status Status { get; set; } = Status.Pending;
        [Required(ErrorMessage = "Event Location is required")]
        public required string Location { get; set; }
        public Guid Id { get; set; }
        public Guid? ClientId { get; set; }
        [Required(ErrorMessage = "Client email is required")]
        public required string ClientEmail { get; set; }
        public string? ClientPhoneNumber { get; set; }
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; set; }
        public decimal BudgetPerPerson { get; set; }
        public int NumberOfGuests { get; set; }
        public string? ExtraNotes { get; set; }
    }

    public enum DietaryRestrictions
    {
        Peanuts,
        TreeNuts,
        Dairy,
        Fish,
        Vegetarian,
        Vegan,
        GlutenFree,
        Halal,
        Kosher,
        Shellfish,
        Soy,
        Egg,
        None
    }
    public enum EventType
    {
        PrivateDinner,
        Party
    }

    public enum Status
    {
        Pending,
        Confirmed,
        Completed,
        Cancelled
    }
}
