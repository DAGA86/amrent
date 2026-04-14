namespace AMRent.Data.Models.Database
{
    public class DataProtectionConsentQuotationChange : Base.EntityChange
    {
        public int DataProtectionConsentQuotationId { get; set; }
        public DataProtectionConsentQuotation DataProtectionConsentQuotation { get; set; }
    }
}
