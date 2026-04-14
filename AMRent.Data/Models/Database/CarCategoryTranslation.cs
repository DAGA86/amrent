using System.ComponentModel.DataAnnotations;

namespace AMRent.Data.Models.Database
{
    public class CarCategoryTranslation : dCore.MultiLanguage.Models.Translation
    {
        public int CarCategoryId { get; set; }
        [StringLength(128)]
        public string Name { get; set; }
        public string? Included { get; set; }

        public CarCategory? CarCategory { get; set; }
    }
}
