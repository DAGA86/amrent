using System.ComponentModel.DataAnnotations;

namespace AMRent.Data.Models.Database
{
    public class BlogArticleTranslation : dCore.MultiLanguage.Models.Translation
    {
        public int BlogArticleId { get; set; }
        [StringLength(128)]
        public string Title { get; set; }
        public string Text { get; set; }
        [StringLength(512)]
        public string? Tags { get; set; }
        [StringLength(512)]
        public string? YoutubeURL { get; set; }

        public BlogArticle? BlogArticle { get; set; }
    }
}
