using System.ComponentModel.DataAnnotations.Schema;

namespace AMRent.Data.Models.Database
{
    public class BlogArticle
    {
        public int Id { get; set; }
        public int BlogArticleCategoryId { get; set; }
        public int ViewCount { get; set; } = 0;
        public int ViewCountIncrement { get; set; } = 0;
        public DateTime CreateDate { get; set; } = DateTime.Now;
		[NotMapped]
        public string? ImagePath { get; set; }
        [NotMapped]
        public string? TopImagePath { get; set; }
        [NotMapped]
        public string? ListImagePath { get; set; }

		public BlogArticleCategory? BlogArticleCategory { get; set; }
        public ICollection<BlogArticleTranslation>? Translations { get; set; } = new List<BlogArticleTranslation>();
    }
}
