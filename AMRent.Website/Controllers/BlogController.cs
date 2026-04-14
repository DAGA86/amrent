using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AMRent.Data.Contexts;
using AMRent.Data.Models.Database;
using AMRent.Website.Models;

namespace AMRent.Website.Controllers
{
    public class BlogController : BaseController
	{
		private readonly IWebHostEnvironment _webHostEnvironment;

		public BlogController(ILogger<HomeController> logger, FullDatabaseContext context, dCore.MultiLanguage.Providers.TranslationProvider translationProvider, IWebHostEnvironment webHostEnvironment) : base(logger, context, translationProvider)
		{
			_webHostEnvironment = webHostEnvironment;
		}

        public IActionResult Index(string tag = "", int categoryId = 0)
        {
	        IQueryable<BlogArticle> blogArticles = _context.BlogArticles
		        .Include(x => x.Translations.Where(t => t.LanguageId == GetSelectedLanguageId()))
		        .Include(x => x.BlogArticleCategory)
					.ThenInclude(y => y.Translations.Where(t => t.LanguageId == GetSelectedLanguageId()))
				.OrderByDescending(x => x.CreateDate);

	        IQueryable<BlogArticle> mostViewedBlogArticles = _context.BlogArticles
		        .Include(x => x.Translations.Where(t => t.LanguageId == GetSelectedLanguageId()))
		        .OrderByDescending(x => x.ViewCount + x.ViewCountIncrement).Take(3);

	        if (categoryId > 0)
	        {
		        blogArticles = blogArticles.Where(x => x.BlogArticleCategoryId == categoryId);
		        mostViewedBlogArticles = mostViewedBlogArticles.Where(x => x.BlogArticleCategoryId == categoryId);

			}
	        else if (!string.IsNullOrEmpty(tag))
	        {
		        blogArticles = blogArticles.Where(x => x.Translations.Any(y => y.Tags.Contains(tag)));
		        mostViewedBlogArticles = mostViewedBlogArticles.Where(x => x.Translations.Any(y => y.Tags.Contains(tag)));
			}

			var viewModel = new BlogIndex()
            {
                BlogArticles = blogArticles.ToList(),
                BlogArticleCategories = _context.BlogArticleCategories.Where(x => x.BlogArticles.Any()).Include(x => x.Translations.Where(t => t.LanguageId == GetSelectedLanguageId())).ToList(),
				MostViewedBlogArticles = mostViewedBlogArticles.ToList()
			};

            foreach (Data.Models.Database.BlogArticle blogArticle in viewModel.BlogArticles)
            {
                if (blogArticle.Translations.Any())
                {
                    blogArticle.Translations.FirstOrDefault().Text =
                        blogArticle.Translations.FirstOrDefault().Text.Length > 200
                            ? $"{blogArticle.Translations.FirstOrDefault().Text.Substring(0, 200)}..."
                            : blogArticle.Translations.FirstOrDefault().Text;
                    if (!string.IsNullOrEmpty(blogArticle.Translations.FirstOrDefault().Tags))
                    {
	                    foreach (string blogArticleTag in blogArticle.Translations.FirstOrDefault().Tags.Split(','))
	                    {
		                    if (!viewModel.Tags.Contains(blogArticleTag))
		                    {
			                    viewModel.Tags.Add(blogArticleTag);
		                    }
	                    }
					}
                }
            }

            viewModel.Tags = viewModel.Tags.OrderBy(x => x).ToList();

            BuildViewBag();
			return View(viewModel);
		}

        public IActionResult Detail(int segmentId)
        {
	        Data.Models.Database.BlogArticle blogArticle = _context.BlogArticles
		        .Include(x => x.Translations.Where(t => t.LanguageId == GetSelectedLanguageId()))
		        .FirstOrDefault(x => x.Id == segmentId);

	        string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "img\\blog");
	        string filePath = Path.Combine(uploadPath, $"{blogArticle.Id}.jpg");
			if (System.IO.File.Exists(filePath))
			{
				blogArticle.ImagePath = $"~/img/blog/{blogArticle.Id}.jpg";
			}

			BuildViewBag();

            blogArticle.ViewCount++;
            _context.SaveChangesAsync();

            return View(blogArticle);
        }
	}
}