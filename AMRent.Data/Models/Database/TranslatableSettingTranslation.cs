namespace AMRent.Data.Models.Database
{
    public class TranslatableSettingTranslation : dCore.MultiLanguage.Models.Translation
    {
        public int TranslatableSettingId { get; set; }
        public string Text { get; set; }

        public TranslatableSetting? TranslatableSetting { get; set; }
    }
}
