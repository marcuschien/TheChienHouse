using System.ComponentModel.DataAnnotations;

namespace TheChienHouse.Models
{
    public class CateringForm
    {
        [Required(ErrorMessage = "CateringType is required")]
        public CateringType CateringType { get; set; }
        [Required(ErrorMessage = "DietaryRestrictions is required")]
        public List<DietaryRestrictions> DietaryRestrictions { get; set; } = new List<DietaryRestrictions>();
        [Required(ErrorMessage = "Event Date is required")]
        public DateTime EventDate { get; set; }
        [Required(ErrorMessage = "Client name is required")]
        public required string ClientName { get; set; }
        [Required(ErrorMessage = "CateringForm Status is required")]
        public Status Status { get; set; } = Status.Pending;
        public Guid Id { get; set; }
        public Guid? ClientId { get; set; }
        public string? ClientEmail { get; set; }
        public string? ClientPhoneNumber { get; set; }
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; set; }

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
    public enum CateringType
    {
        Onsite,
        Offsite,
        Pickup,
        Delivery,
    }

    public enum Status
    {
        Pending,
        Confirmed,
        Completed,
        Cancelled
    }
}
