namespace AMRent.Data.Models.Database
{
    public class BlogArticleCategory
    {
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<BlogArticle>? BlogArticles { get; set; }
        public ICollection<BlogArticleCategoryTranslation>? Translations { get; set; } = new List<BlogArticleCategoryTranslation>();
    }
}
