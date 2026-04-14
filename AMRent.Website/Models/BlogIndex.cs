namespace AMRent.Website.Models
{
    public class BlogIndex
	{
		public List<Data.Models.Database.BlogArticle> BlogArticles { get; set; } = new();
		public List<Data.Models.Database.BlogArticleCategory> BlogArticleCategories { get; set; } = new();
		public List<string> Tags { get; set; } = new();
		public List<Data.Models.Database.BlogArticle> MostViewedBlogArticles { get; set; } = new();
	}
}
