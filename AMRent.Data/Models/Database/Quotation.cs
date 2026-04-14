using dCore.MultiLanguage.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMRent.Data.Models.Database
{
    public class Quotation
    {
        public int Id { get; set; }
        [Required(AllowEmptyStrings = true)]
        public string Number { get; set; } = string.Empty;
        public DateTime CreateDate { get; set; }
        [EnumDataType(typeof(Enums.ReservationQuotationSources))]
        public Enums.ReservationQuotationSources Source { get; set; }
        public Enums.QuotationStatus Status { get; set; } = Enums.QuotationStatus.Registered;
        public Guid? UserId { get; set; }
        public DateTime ExpireDateTime { get; set; } = DateTime.UtcNow.AddDays(15);

        // Pickup / Return
        public int PickupLocationId { get; set; }
        public string? CustomPickupLocationName { get; set; }
        public DateTime PickupDateTime { get; set; }
        public int ReturnLocationId { get; set; }
        public string? CustomReturnLocationName { get; set; }
        public DateTime ReturnDateTime { get; set; }
        [NotMapped]
        public int? TotalDays { get; set; }

        // Customer
        public string CustomerName { get; set; }
        public string CustomerEmailAddress { get; set; }
        public int? CustomerTelephonePrefixCountryId { get; set; }
        public string CustomerTelephone { get; set; }

        // Other
        public string? Comments { get; set; }
        public string? RejectionReason { get; set; }
        public int LanguageId { get; set; } = (int)Enums.Languages.Portuguese;

        // Navigation Properties
        public User? User { get; set; }
        public Language? Language { get; set; }
        public PickupReturnLocation? PickupLocation { get; set; }
        public PickupReturnLocation? ReturnLocation { get; set; }
        public Country? CustomerTelephonePrefixCountry { get; set; }
        public IList<QuotationItem> QuotationItems { get; set; } = new List<QuotationItem>();
        public List<QuotationChange>? Changes { get; set; } = new();
        public List<DataProtectionConsentQuotation>? DataProtectionConsents { get; set; } = new();
    }
}
