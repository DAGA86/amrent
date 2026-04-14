namespace AMRent.Data.Models.Database
{
    public class DataProtectionConsentTranslation : dCore.MultiLanguage.Models.Translation
    {
        public int DataProtectionConsentId { get; set; }
        public string Text { get; set; }

        public DataProtectionConsent? DataProtectionConsent { get; set; }
    }
}
