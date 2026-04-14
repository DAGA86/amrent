using System.ComponentModel.DataAnnotations;

namespace AMRent.Data.Models.Database
{
    public class CountryTranslation : dCore.MultiLanguage.Models.Translation
    {
        public int CountryId { get; set; }
        [StringLength(128)]
        public string Name { get; set; }

        public Country? Country { get; set; }
    }
}
