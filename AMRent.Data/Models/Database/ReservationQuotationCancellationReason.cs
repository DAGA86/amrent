namespace AMRent.Data.Models.Database
{
    public class ReservationQuotationCancellationReason
    {
        public int Id { get; set; }
        public string Description { get; set; }

        public List<Reservation> Reservations { get; set; } = new();
        public List<Quotation> Quotations { get; set; } = new();
    }
}
