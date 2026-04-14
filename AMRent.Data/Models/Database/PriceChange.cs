namespace AMRent.Data.Models.Database
{
    public class PriceChange : Base.EntityChange
    {
        public int PriceId { get; set; }
        public Price Price { get; set; }
    }
}
