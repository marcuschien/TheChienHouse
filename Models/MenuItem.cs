using System.ComponentModel.DataAnnotations;

namespace TheChienHouse.Models
{
    public class MenuItem
    {
        public string Id { get; set; } // TO DO: Make me a Guid. 

        [Required(ErrorMessage = "Item name is required")]
        public required string Name { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        public required string DishType { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // The timestamps are useful for tracing and debugging purposes 
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow; // Some customers find them useful as well
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

    /*
    public enum DishName
    {
        SpotPrawnCarpaccio,
        SpotPrawnBisque,
        SpotPrawnPaella,
        BeefTartare,
        KaleSalad,
        ChocolateCake,
        StickyToffeePudding
        // Does it make sense to have a DishName enum?
        // Would be safer for matching and avoiding typos, but would require updating the enum every time a new dish is added. 
        // If we have a lot of dishes or the menu changes frequently, this could get unwieldy.
    }
    */
}


