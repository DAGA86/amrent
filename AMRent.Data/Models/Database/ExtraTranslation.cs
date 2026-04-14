using System.ComponentModel.DataAnnotations;

namespace AMRent.Data.Models.Database
{
    public class ExtraTranslation : dCore.MultiLanguage.Models.Translation
    {
        public int ExtraId { get; set; }
        [StringLength(64)]
        public string Name { get; set; }

        public Extra? Extra { get; set; }
    }
}
