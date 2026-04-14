using AMRent.Data.Contexts;
using AMRent.Website.Extensions;
using dCore.MultiLanguage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AMRent.Website.Controllers
{
    public class BaseController : Controller
    {
	    internal readonly ILogger<BaseController> _logger;
	    internal readonly FullDatabaseContext _context;
		internal readonly dCore.MultiLanguage.Providers.TranslationProvider _translationProvider;

		public BaseController(ILogger<BaseController> logger, FullDatabaseContext context, dCore.MultiLanguage.Providers.TranslationProvider translationProvider)
        {
            _logger = logger;
            _context = context;
            _translationProvider = translationProvider;
			
		}

		internal int GetSelectedLanguageId()
		{
			return HttpContext?.Session?.GetInt32("SelectedLanguage") ?? (int)Data.Enums.Languages.Portuguese;
		}

        internal void BuildViewBag()
        {
			List<Language> languages = HttpContext?.Session?.GetObject<List<Language>>("Languages");
	        if (languages == null)
	        {
		        languages = _context.Languages.ToList();
		        HttpContext.Session.SetObject("Languages", languages);
	        }
			int selectedLanguageId = GetSelectedLanguageId();
			ViewBag.Languages = new SelectList(languages, "Id", "FlagUrl", selectedLanguageId);
			ViewBag.SelectedLanguageObject = _context.Languages.FirstOrDefault(x => x.Id == GetSelectedLanguageId());

            bool hasCampaigns = _context.Campaigns.Any(x => x.AppliesToBookingsMadeFromUtc <= DateTime.UtcNow && x.AppliesToBookingsMadeUntilUtc >= DateTime.UtcNow && x.IsActive);
			ViewBag.HasCampaigns = hasCampaigns;
        }
    }
}
