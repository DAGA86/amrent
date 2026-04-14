using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AMRent.Data.Contexts;
using AMRent.Website.Models;

namespace AMRent.Website.Controllers
{
    public class AboutController : BaseController
    {
	    public AboutController(ILogger<HomeController> logger, FullDatabaseContext context, dCore.MultiLanguage.Providers.TranslationProvider translationProvider) : base(logger, context, translationProvider)
        {
        }

        public IActionResult Index()
        {
	        Data.Models.Database.About about = _context.About.Include(x => x.Translations.Where(t => t.LanguageId == GetSelectedLanguageId())).FirstOrDefault();

            About model = new()
			{
				Title = about?.Translations?.FirstOrDefault()?.Title,
				Text = about?.Translations?.FirstOrDefault()?.Text,
				ImageSideText = about?.Translations?.FirstOrDefault()?.ImageSideText
			};
			BuildViewBag();
			return View(model);
        }
    }
}