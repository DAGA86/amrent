namespace AMRent.Data.Models.Database
{
    public class ReservationChange : Base.EntityChange
    {
        public int ReservationId { get; set; }
        public Reservation Reservation { get; set; }
    }
}
