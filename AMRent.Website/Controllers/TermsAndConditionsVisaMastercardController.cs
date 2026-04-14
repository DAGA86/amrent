using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AMRent.Data.Contexts;
using AMRent.Data.Models.Database;

namespace AMRent.Website.Controllers
{
    public class TermsAndConditionsVisaMastercardController : BaseController
    {
	    public TermsAndConditionsVisaMastercardController(ILogger<HomeController> logger, FullDatabaseContext context, dCore.MultiLanguage.Providers.TranslationProvider translationProvider) : base(logger, context, translationProvider)
        {
        }

        public IActionResult Index()
        {
            int selectedLanguageId = GetSelectedLanguageId();

            TranslatableSetting? termsAndConditionsVisaMastercard = _context.TranslatableSettings
		        .Include(x => x.Translations.Where(x => x.LanguageId == selectedLanguageId))
		        .FirstOrDefault(x => x.Code == Data.Enums.TranslatableSettings.TermsAndConditionsVisaMastercard);
            
			BuildViewBag();
			return View(termsAndConditionsVisaMastercard);
        }
    }
}