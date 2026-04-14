using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AMRent.Data.Contexts;
using AMRent.Data.Models.Database;

namespace AMRent.Website.Controllers
{
    public class TermsAndConditionsController : BaseController
    {
	    public TermsAndConditionsController(ILogger<HomeController> logger, FullDatabaseContext context, dCore.MultiLanguage.Providers.TranslationProvider translationProvider) : base(logger, context, translationProvider)
        {
        }

        public IActionResult Index()
        {
            int selectedLanguageId = GetSelectedLanguageId();

            TranslatableSetting? termsAndConditions = _context.TranslatableSettings
		        .Include(x => x.Translations.Where(x => x.LanguageId == selectedLanguageId))
		        .FirstOrDefault(x => x.Code == Data.Enums.TranslatableSettings.TermsAndConditions);
            
			BuildViewBag();
			return View(termsAndConditions);
        }
    }
}