using System.ComponentModel.DataAnnotations;

namespace TheChienHouse.Models
{
    public class MenuItem
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Item name is required")]
        [StringLength(100, ErrorMessage = "Menu item name cannot exceed 100 characters")]
        public string? Name { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        public DishType DishType { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // The timestamps are useful for tracing and debugging purposes 
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // Some customers find them useful as well
    }

    public enum DishType
    {
        Soup,
        Salad,
        Appetizer,
        Main,
        Dessert,
        Drink,
        Side,
        Shareable
    }
}


