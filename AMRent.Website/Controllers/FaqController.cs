using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AMRent.Data.Contexts;
using AMRent.Data.Models.Database;

namespace AMRent.Website.Controllers
{
    public class FaqController : BaseController
    {
	    public FaqController(ILogger<HomeController> logger, FullDatabaseContext context, dCore.MultiLanguage.Providers.TranslationProvider translationProvider) : base(logger, context, translationProvider)
		{
        }

        public IActionResult Index()
        {
            List<QuestionAndAnswer> viewModel = _context.QuestionsAndAnswers
                .Include(x => x.Translations.Where(t => t.LanguageId == GetSelectedLanguageId())).ToList();

            BuildViewBag();
			return View(viewModel);
        }
    }
}