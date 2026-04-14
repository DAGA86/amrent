namespace AMRent.Data.Models.View
{
    public class PriceIndex
    {
        public int Id { get; set; }
        public string Season { get; set; }
        public string Segment { get; set; }
        public int Days { get; set; }
        public decimal Value { get; set; }
    }
}
