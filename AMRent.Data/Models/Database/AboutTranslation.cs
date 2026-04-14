using System.ComponentModel.DataAnnotations;

namespace AMRent.Data.Models.Database
{
    public class AboutTranslation : dCore.MultiLanguage.Models.Translation
    {
        public int AboutId { get; set; }
        [StringLength(128)]
        public string Title { get; set; }
        public string Text { get; set; }
        [StringLength(32)]
        public string? ImageSideText { get; set; }

        public About? About { get; set; }
    }
}
