using AMRent.Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMRent.Website.Models
{
    public class BookingPayment
    {
        public int ReservationId { get; set; }

        public string ReservationNumber { get; set; }

        [Column(TypeName = "decimal(7, 2)")]
        public decimal TotalCost { get; set; }
        public Data.Enums.PaymentTypes? PaymentType { get; set; } = PaymentTypes.BankTransfer;
        public string? MbWayNumber { get; set; }
    }
}
