using System.ComponentModel.DataAnnotations.Schema;

namespace AMRent.Data.Models.Database
{
    public class ReservationService
    {
        public int ReservationId { get; set; }
        public int ServiceId { get; set; }

        [Column(TypeName = "decimal(7, 2)")]
        public decimal Value { get; set; }

        public Reservation? Reservation { get; set; }
        public Service? Service { get; set; }
    }
}
