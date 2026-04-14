using System.ComponentModel.DataAnnotations;

namespace AMRent.Data.Models.Database
{
    public class BlogArticleCategoryTranslation : dCore.MultiLanguage.Models.Translation
    {
        public int BlogArticleCategoryId { get; set; }
        [StringLength(128)]
        public string Name { get; set; }

        public BlogArticleCategory? BlogArticleCategory { get; set; }
    }
}
