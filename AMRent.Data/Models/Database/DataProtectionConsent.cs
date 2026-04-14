namespace AMRent.Data.Models.Database
{
    public class DataProtectionConsent
    {
        public int Id { get; set; }
        public bool IsMandatory { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public short SortNumber { get; set; }

        public List<DataProtectionConsentTranslation>? Translations { get; set; } = new List<DataProtectionConsentTranslation>();
    }
}
