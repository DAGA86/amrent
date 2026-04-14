using System.ComponentModel.DataAnnotations.Schema;

namespace AMRent.Data.Models.Database
{
    public class HomeBanner
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<HomeBannerTranslation>? Translations { get; set; } = new List<HomeBannerTranslation>();
    }
}
