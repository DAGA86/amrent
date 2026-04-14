using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMRent.Data.Models.Database
{
    public class HomeBannerTranslation : dCore.MultiLanguage.Models.Translation
    {
        public int HomeBannerId { get; set; }
        [StringLength(512)]
        public string? URL { get; set; }
        [NotMapped]
        public string? ImagePath { get; set; }

        public HomeBanner? HomeBanner { get; set; }
    }
}
