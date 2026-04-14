using AMRent.Data.Enums;
using dCore.Helpers.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMRent.Data.Models.Database
{
    public class Campaign
    {
        public int Id { get; set; }
        [RequiredWhen(nameof(DiscountType), Enums.DiscountTypes.Percentage, Enums.DiscountTypes.Euro)]
        public int? Value { get; set; }
        public Enums.DiscountTypes DiscountType { get; set; } = DiscountTypes.Euro;
        public DateTime ValidFromUtc { get; set; }
        public DateTime ValidUntilUtc { get; set; }
        public DateTime AppliesToBookingsMadeFromUtc { get; set; }
        public DateTime AppliesToBookingsMadeUntilUtc { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsUsableForBackoffice { get; set; }
        [NotMapped]
        public List<int> SegmentIds { get; set; } = new();
        [NotMapped]
        [RequiredWhen(nameof(DiscountType), Enums.DiscountTypes.Extra)]
        public List<int> ExtraIds { get; set; } = new();
        [NotMapped]
        public string? TopImagePath { get; set; }
        [NotMapped]
        public string? PopupImagePath { get; set; }

        public ICollection<CampaignTranslation>? Translations { get; set; } = new List<CampaignTranslation>();
        public ICollection<CarSegment>? CarSegments { get; set; } = new List<CarSegment>();
        public ICollection<Extra>? Extras { get; set; } = new List<Extra>();
        public ICollection<Reservation>? Reservations { get; set; } = new List<Reservation>();
        //internal ICollection<CampaignCarSegment>? CampaignSegments { get; set; } = new List<CampaignCarSegment>();
    }
}
