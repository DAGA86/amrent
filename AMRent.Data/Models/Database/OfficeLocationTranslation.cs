using System.ComponentModel.DataAnnotations;

namespace AMRent.Data.Models.Database
{
    public class OfficeLocationTranslation : dCore.MultiLanguage.Models.Translation
    {
        public int OfficeLocationId { get; set; }
        [StringLength(128)]
        public string Name { get; set; }

        public OfficeLocation? OfficeLocation { get; set; }
    }
}
