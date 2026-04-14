namespace AMRent.Data.Models.Database
{
    public class TranslatableSetting
    {
        public int Id { get; set; }
        public Enums.TranslatableSettings Code { get; set; }

        public ICollection<TranslatableSettingTranslation>? Translations { get; set; } = new List<TranslatableSettingTranslation>();
    }
}
