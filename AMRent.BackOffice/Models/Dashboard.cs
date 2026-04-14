namespace AMRent.BackOffice.Models
{
    public class Dashboard
    {
        public List<Reservations> FinishedReservationsPerMonthByCreateDate { get; set; } = [];
        public List<Reservations> CancelledReservationsPerMonthByCreateDate { get; set; } = [];
        public List<Reservations> ReservationsPerMonthByPickupDate { get; set; } = [];
    }

    public class Reservations
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Count { get; set; } = 0;
    }
}
