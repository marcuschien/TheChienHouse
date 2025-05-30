using System.ComponentModel.DataAnnotations;

namespace TheChienHouse.Models
{
    public class Sale
    {
        public long Id { get; set; } // Make me a UUID
        public required List<LineItem> LineItems {get; set;}
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Total {get; set;}
        [Range(0.01, double.MaxValue, ErrorMessage = "Discount must be greater than 0")]
        public decimal? SaleDiscount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // The timestamps are useful for tracing and debugging purposes 
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // Some customers find them useful as well
    }

    /* 
     * In reality I would probably give LineItem its' own model, since we'd probably want to have a table of these in the DB
     * But because there's no need for a table of LineItems in this assignment, I'm just going to include it in the Sale model.
     * The line items are calculated and included in the Sale object response. Outside of the scope of the assignment, but would save those to the DB after processing.
     * Would be nice to have (from user's perspective) the line items saved for future sale requests. They can just pick out the line items they want to buy again and create a new sale with them.
     */
    public class LineItem
    {
        public long Id { get; set; } // Make me a UUID
        public required long menuItemId { get; set; } // Using this to pull the Product from the DB.
        [Range(1, long.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public required long Quantity {get; set;}
        public MenuItem? MenuItemForSale { get; set; } // This is the product that is being sold (fetched with ProductID), used for name and price data.
        /*
         * [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")] 
         * Normally I'd add a tag like this ^ for sanity check
         * But in this assignment LineItems don't have their own table and so I initialize them with a price of 0 when the sale is created and calculate it at run time. 
         * */
        public decimal TotalCost { get; set; }
        public decimal LineItemDiscount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // The timestamps are useful for tracing and debugging purposes 
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // Some customers find them useful as well
    }
}
