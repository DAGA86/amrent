namespace AMRent.Data.Models.View
{
    public class CustomerIndex
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int QuotationCount { get; set; }
        public int ReservationCount { get; set; }
    }
}
