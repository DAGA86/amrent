using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AMRent.Data.Contexts;
using AMRent.Data.Models.Database;
using AMRent.Website.Models;

namespace AMRent.Website.Controllers
{
    public class OffersController : BaseController
	{
		private readonly IWebHostEnvironment _webHostEnvironment;

		public OffersController(ILogger<HomeController> logger, FullDatabaseContext context, dCore.MultiLanguage.Providers.TranslationProvider translationProvider, IWebHostEnvironment webHostEnvironment) : base(logger, context, translationProvider)
		{
			_webHostEnvironment = webHostEnvironment;
		}

        public IActionResult Index()
        {
	        List<Campaign> campaigns = _context.Campaigns
		        .Where(x => x.AppliesToBookingsMadeFromUtc <= DateTime.UtcNow && x.AppliesToBookingsMadeUntilUtc >= DateTime.UtcNow && x.IsActive)
		        .Include(x => x.Translations.Where(t => t.LanguageId == GetSelectedLanguageId()))
		        .ToList();

	        var viewModel = new OffersIndex()
            {
				Campaigns = campaigns,
				//SelectedLanguage = _context.Languages.FirstOrDefault(x => x.Id == GetSelectedLanguageId())
			};
            
            foreach (Data.Models.Database.Campaign campaign in viewModel.Campaigns)
            {
                if (campaign.Translations.Any())
                {
                    campaign.Translations.FirstOrDefault().Description =
                        campaign.Translations.FirstOrDefault().Description.Length > 200
                            ? $"{campaign.Translations.FirstOrDefault().Description.Substring(0, 200)}..."
                            : campaign.Translations.FirstOrDefault().Description;
                }
            }

            BuildViewBag();
			return View(viewModel);
		}

        public IActionResult Detail(int segmentId)
        {
	        Data.Models.Database.Campaign campaign = _context.Campaigns
		        .Include(x => x.Translations.Where(t => t.LanguageId == GetSelectedLanguageId()))
		        .FirstOrDefault(x => x.Id == segmentId);

			BuildViewBag();
            _context.SaveChangesAsync();

            return View(campaign);
        }
	}
}