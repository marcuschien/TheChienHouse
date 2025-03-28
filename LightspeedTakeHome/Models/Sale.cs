namespace LightspeedTakeHome.Models
{
    public class Sale
    {
        public long Id { get; set; }
        public LineItem[] LineItems {get; set;}
        public long[] LineItemTotals {get; set;}
        public long Total {get; set;}

    }
    
    public class LineItem
    {
        public long Id {get; set;}
        public long Quantity {get; set;}
    }
}
