using System.ComponentModel.DataAnnotations;

namespace TheChienHouse.Models
{
    public class CateringForm
    {
        public Guid Id { get; set; }

        public CateringType CateringType { get; set; }

        public List<DietaryRestrictions> MenuItems { get; set; } = new List<DietaryRestrictions>();

        public Guid? ClientId { get; set; }

        public DateTime EventDate { get; set; }

        [Required(ErrorMessage = "Contact name is required")]
        public required string ContactName { get; set; }

        public string? ContactEmail { get; set; }

        public string? ContactPhoneNumber { get; set; }

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
