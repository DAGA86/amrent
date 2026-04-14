using dCore.Helpers.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMRent.Data.Models.Database
{
    public class Voucher
    {
        public int Id { get; set; }
        public string Code { get; set; }
        [RequiredWhen(nameof(DiscountType), Enums.DiscountTypes.Percentage, Enums.DiscountTypes.Euro)]
        public int? Value { get; set; }
        public Enums.DiscountTypes DiscountType { get; set; } = Enums.DiscountTypes.Euro;
        public bool IsUsableForBackoffice { get; set; }
        [NotMapped]
        [RequiredWhen(nameof(DiscountType), Enums.DiscountTypes.Extra)]
        public List<int> ExtraIds { get; set; } = new();
        public DateTime? ValidFromUtc { get; set; }
        public DateTime? ValidUntilUtc { get; set; }
        public DateTime? AppliesToBookingsMadeFromUtc { get; set; }
        public DateTime? AppliesToBookingsMadeUntilUtc { get; set; }

        public ICollection<Extra>? Extras { get; set; } = new List<Extra>();
        public ICollection<Reservation>? Reservations { get; set; } = new List<Reservation>();
    }
}
