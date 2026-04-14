namespace AMRent.Website.Models
{
    public class BookingAskQuotation
    {
        // Filters
        public int PickupLocationId { get; set; }
        public string? CustomPickupLocationName { get; set; }
        public string PickupLocationName { get; set; }
        public DateTime PickupDateTime { get; set; }
        public int ReturnLocationId { get; set; }
        public string? CustomReturnLocationName { get; set; }
        public string ReturnLocationName { get; set; }
        public DateTime ReturnDateTime { get; set; }

        // Data
        public int SegmentId { get; set; }
        public Guid SegmentImageId { get; set; }
        public string? SegmentName { get; set; }
        public decimal RentValue { get; set; }
        public decimal? TotalValue { get; set; }
        public decimal PickupValue { get; set; }
        public decimal ReturnValue { get; set; }
        public int Days { get; set; }
        public int SelectedInsuranceLevelId { get; set; }

        // Driver
        public string DriverName { get; set; }
        public DateTime DriverBirthDate { get; set; }
        public string DriverEmail { get; set; }
        public int DriverTelephoneCountryId { get; set; }
        public string DriverTelephone { get; set; }

        // Other
        public string Comments { get; set; }
        public string? VoucherCode { get; set; }
        public int? CampaignId { get; set; }
        public int? VoucherId { get; set; }
        public string? CampaignVoucherName { get; set; }

        public List<Data.Models.Database.Insurance>? Insurances { get; set; } = new();
        public List<AMRent.Website.Models.SelectedExtra>? Extras { get; set; } = new();
        public List<AMRent.Website.Models.PickupReturnTemporaryTax>? PickupReturnTemporaryTaxes { get; set; } = new();
        public List<AMRent.Website.Models.DataProtectionConsent>? DataProtectionConsents { get; set; } = new();
    }
}
