namespace AMRent.Data.Models.Database
{
    public class DataProtectionConsentQuotation
    {
        public int Id { get; set; }
        public bool HasConsented { get; set; } = false;

        public int DataProtectionConsentId { get; set; }
        public DataProtectionConsent? DataProtectionConsent { get; set; }

        public int QuotationId { get; set; }
        public Quotation? Quotation { get; set; }

        public List<DataProtectionConsentQuotationChange>? DataProtectionConsentQuotationChanges { get; set; } = new();
    }
}
