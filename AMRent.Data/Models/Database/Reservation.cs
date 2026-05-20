using dCore.Helpers.DataAnnotations;
using dCore.MultiLanguage.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMRent.Data.Models.Database
{
    public class Reservation
    {
        public int Id { get; set; }
        [Required(AllowEmptyStrings = true)]
        public string Number { get; set; } = string.Empty;
        public DateTime CreateDate { get; set; }
        [EnumDataType(typeof(Enums.ReservationQuotationSources))]
        public Enums.ReservationQuotationSources Source { get; set; }
        public Enums.ReservationStatus Status { get; set; } = Enums.ReservationStatus.Registered;
        public Guid? AssignedUserId { get; set; }
        public Guid? CustomerId { get; set; }
        public DateTime? QuotationExpireDateTime { get; set; }

		// Associations
		public int CarSegmentId { get; set; }
        public int? CampaignId { get; set; }
        public int? VoucherId { get; set; }
        public int InsuranceLevelId { get; set; }

        // Pickup / Return
        public int PickupLocationId { get; set; }
        public string? CustomPickupLocationName { get; set; }
        public DateTime PickupDateTime { get; set; }
        public int ReturnLocationId { get; set; }
        public string? CustomReturnLocationName { get; set; }
        public DateTime ReturnDateTime { get; set; }
        [NotMapped]
        public int TotalDays { get; set; }

        // Payment
        [EnumDataType(typeof(Enums.PaymentTypes))]
        public Enums.PaymentTypes? PaymentType { get; set; }
        [EnumDataType(typeof(Enums.PaymentStatus))]
        public Enums.PaymentStatus PaymentStatus { get; set; } = Enums.PaymentStatus.Pending;
        public string? ExternalPaymentReference { get; set; }
        public string? MultibancoEntity { get; set; }
        public string? MultibancoReference { get; set; }
        public string? CreditCardAuthorizationKey { get; set; }
        public bool HasAdvancePartialPayment { get; set; } = false;
        public Enums.PaymentTypes? AdvancePartialPaymentPaymentType { get; set; }
        [Column(TypeName = "decimal(7, 2)")]
        public decimal? AdvancePartialPaymentValue { get; set; }

        // Costs
        [Column(TypeName = "decimal(7, 2)")]
        public decimal CarSegmentCost { get; set; }
        [Column(TypeName = "decimal(7, 2)")]
        public decimal PickupCost { get; set; }
        [Column(TypeName = "decimal(7, 2)")]
        public decimal ReturnCost { get; set; }
        [Column(TypeName = "decimal(7, 2)")]
        public decimal InsuranceCost { get; set; }
        [Column(TypeName = "decimal(7, 2)")]
        public decimal InsuranceExcess { get; set; }
        [Column(TypeName = "decimal(7, 2)")]
        public decimal TotalCost { get; set; }
        [Column(TypeName = "decimal(7, 2)")]
        public decimal? TotalCostOverride { get; set; }

        // Driver
        public string DriverName { get; set; }
        public DateTime DriverBirthDate { get; set; }
        public string DriverEmailAddress { get; set; }
        public int? DriverTelephonePrefixCountryId { get; set; }
        public string DriverTelephone { get; set; }
        public Enums.IdentityType? DriverIdentityType { get; set; }
        public int? DriverIdentityCountryId { get; set; }
        public string? DriverIdentityNumber { get; set; }
        public string? DriverVatNumber { get; set; }
        public int? DriverLicenseCountryId { get; set; }
        public string? DriverLicenseNumber { get; set; }
        public DateTime? DriverLicenseDate { get; set; }
        public DateTime? DriverLicenseExpireDate { get; set; }

        // Bill
        public string? BillName { get; set; }
        public string? BillEmailAddress { get; set; }
        public int? BillTelephonePrefixCountryId { get; set; }
        public string? BillTelephone { get; set; }
        public string? BillAddress { get; set; }
        public string? BillPostalCode { get; set; }
        public string? BillPostalLocation { get; set; }
        public int? BillCountryId { get; set; }
        public string? BillVatNumber { get; set; }

        // Other
        public string? FlightNumber { get; set; }
        public string? Comments { get; set; }
        public string? RejectionReason { get; set; }
        public int? SourceQuotationId { get; set; }
        public int LanguageId { get; set; } = (int)Enums.Languages.Portuguese;
        [RequiredWhen(nameof(Status), Enums.ReservationStatus.Cancelled)]
        public int? CancellationReasonId { get; set; }
        public string? CancellationReasonDescription { get; set; }

        // Navigation Properties
        public User? AssignedUser { get; set; }
        public User? Customer { get; set; }
        public Language? Language { get; set; }
        public CarSegment? CarSegment { get; set; }
        public PickupReturnLocation? PickupLocation { get; set; }
        public PickupReturnLocation? ReturnLocation { get; set; }
        public InsuranceLevel? InsuranceLevel { get; set; }
        public Campaign? Campaign { get; set; }
        public Voucher? Voucher { get; set; }
        public Country? DriverTelephonePrefixCountry { get; set; }
        public Country? DriverIdentityCountry { get; set; }
        public Country? DriverLicenseCountry { get; set; }
        public Country? BillTelephonePrefixCountry { get; set; }
        public Country? BillCountry { get; set; }
        public Quotation? SourceQuotation { get; set; }
        public ReservationQuotationCancellationReason? CancellationReason { get; set; }

        public List<ReservationExtraDriver>? ExtraDrivers { get; set; } = new();
        public List<ReservationExtra>? Extras { get; set; } = new();
        public List<ReservationPickupReturnTemporaryTax>? PickupReturnTemporaryTaxes { get; set; } = new();
        public List<ReservationService>? Services { get; set; } = new();
        public List<ReservationChange>? Changes { get; set; } = new();
        public List<DataProtectionConsentReservation>? DataProtectionConsents { get; set; } = new();
    }
}
