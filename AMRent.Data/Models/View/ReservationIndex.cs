namespace AMRent.Data.Models.View
{
    public class ReservationIndex
    {
        public int Id { get; set; }

        public string? Number { get; set; }
        public string? CollaboratorName { get; set; }
        public string? Status { get; set; }
        public string? PaymentStatus { get; set; }
        public string? DriverName { get; set; }
        public string? PickupLocation { get; set; }
        public DateTime? PickupDateTime { get; set; }
        public DateTime? CreateDateTime { get; set; }
    }
}
