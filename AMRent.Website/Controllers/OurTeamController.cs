using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AMRent.Data.Contexts;
using AMRent.Data.Models.Database;

namespace AMRent.Website.Controllers
{
    public class OurTeamController : BaseController
    {
	    public OurTeamController(ILogger<HomeController> logger, FullDatabaseContext context, dCore.MultiLanguage.Providers.TranslationProvider translationProvider) : base(logger, context, translationProvider)
		{
        }

        public IActionResult Index()
        {
            List<TeamMember> viewModel = _context.TeamMembers
                .Include(x => x.Translations.Where(t => t.LanguageId == GetSelectedLanguageId()))
                .OrderBy(x => x.SortNumber)
                .ToList();

            BuildViewBag();
			return View(viewModel);
        }
    }
}