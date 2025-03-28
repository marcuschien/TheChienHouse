namespace LightspeedTakeHome.Models
{
    public class Sale
    {
        public long Id { get; set; }

        //Making the following required because you can't have a sale without a LineItem. 
        public required List<LineItem> LineItems {get; set;}
        public long Total {get; set;}

    }
    
    public class LineItem
    {
        public long Id { get; set; }
        public required long ProductId { get; set; } // Using this to pull the Product from the DB.
        public required long Quantity {get; set;}
        public Product? ProductForSale { get; set; } // This is a reference to the product that is being sold, used for name and price data.
        public long TotalCost { get; set; }
    }
}
