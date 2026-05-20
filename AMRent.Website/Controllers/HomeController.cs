using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;
using System.Linq;

namespace AMRent.Website.Controllers
{
    //record PaisInfo
    //{
    //    public Dictionary<string, string> Translations { get; init; }
    //    public string Alpha2Code { get; init; }
    //    [System.Text.Json.Serialization.JsonPropertyName("callingCodes")]
    //    public List<string> CallingCodes { get; init; }

    //    public string NomePT => Translations != null && Translations.TryGetValue("pt", out var nomePt) ? nomePt : "";
    //    public string NomeFR => Translations != null && Translations.TryGetValue("fr", out var nomefr) ? nomefr : "";
    //    public string name { get; init; }

    //    public string CodigoTelefonico => CallingCodes != null && CallingCodes.Count > 0 ? CallingCodes[0] : "";
    //}

    public class HomeController : BaseController
    {
	    public HomeController(ILogger<HomeController> logger, AMRent.Data.Contexts.FullDatabaseContext context, dCore.MultiLanguage.Providers.TranslationProvider translationProvider) : base(logger, context, translationProvider)
        {
        }

        //public IActionResult Countries()
        //{
        //    List<PaisInfo> listaPaises = ObterListaPaises();
        //    List<Country> countryList = new List<Country>();
        //    List<Language> languages = _context.Languages.ToList();
        //    // Exibir informações dos países
        //    foreach (var pais in listaPaises)
        //    {
        //        if (pais.CallingCodes.Count == 0 || pais.CallingCodes.Count > 1)
        //        {

        //        }
        //        Country newCountry = new Country()
        //        {
        //            Alpha2Code = pais.Alpha2Code,
        //            TelephoneCode = pais.CodigoTelefonico,
        //        };
        //        string[] countries = new[] { "pt", "fr" };
        //        newCountry.Translations.Add(new CountryTranslation()
        //        {
        //            LanguageId = languages.FirstOrDefault(x => x.Code == "en").Id,
        //            Name = pais.name,
        //        });
        //        foreach (KeyValuePair<string, string> paisTranslation in pais.Translations.Where(x => countries.Contains(x.Key)))
        //        {
        //            newCountry.Translations.Add(new CountryTranslation()
        //            {
        //                LanguageId = languages.FirstOrDefault(x => x.Code == paisTranslation.Key).Id,
        //                Name = paisTranslation.Value,
        //            });
        //        }
        //        countryList.Add(newCountry);
        //    }
        //    _context.Countries.AddRange(countryList);
        //    _context.SaveChanges();

        //    return RedirectToAction("Index");
        //}

        //static List<PaisInfo> ObterListaPaises()
        //{
        //    string url = "https://restcountries.com/v2/all";

        //    using (HttpClient httpClient = new HttpClient())
        //    {
        //        try
        //        {
        //            var resposta = httpClient.GetByteArrayAsync(url);
        //            var utf8Json = Encoding.UTF8.GetString(resposta.Result);

        //            // Converter a resposta JSON para uma lista de objetos PaisInfo
        //            var listaPaises = JsonSerializer.Deserialize<List<PaisInfo>>(utf8Json, new JsonSerializerOptions
        //            {
        //                PropertyNameCaseInsensitive = true // Ignora a diferenciação de maiúsculas e minúsculas nas propriedades
        //            });
        //            return listaPaises;
        //        }
        //        catch (HttpRequestException e)
        //        {
        //            Console.WriteLine($"Erro ao obter dados da API: {e.Message}");
        //            return new List<PaisInfo>();
        //        }
        //    }
        //}

        private Models.HomeIndex EnsureViewModelHasDefaults(Models.HomeIndex inputModel)
        {
            if (inputModel.PickupLocationId == 0 | inputModel.ReturnLocationId == 0)
            {
                int? defaultPickupReturnLocationId = _context.PickupReturnLocations.FirstOrDefault(x => x.IsSelectedByDefault)?.Id;
                if (!defaultPickupReturnLocationId.HasValue)
                    throw new ApplicationException("Sem definição de localização de recolha / devolução por defeito.");

                if (inputModel.PickupLocationId == 0)
                    inputModel.PickupLocationId = defaultPickupReturnLocationId.Value;
                if (inputModel.ReturnLocationId == 0)
                    inputModel.ReturnLocationId = defaultPickupReturnLocationId.Value;

                Shared.Providers.PickupReturnLocation pickupReturnLocationProvider = new Shared.Providers.PickupReturnLocation(_context);
                inputModel.PickupDateTime = pickupReturnLocationProvider.GetNextCompliantWithAnticipationDateTime(
                    inputModel.PickupLocationId, dCore.Helpers.DateTime.RoundToNearestFutureQuarterHour(DateTime.Now));
                inputModel.ReturnDateTime = pickupReturnLocationProvider.GetNextAvailableDateTime(
                    inputModel.PickupLocationId, dCore.Helpers.DateTime.RoundToNearestFutureQuarterHour(inputModel.PickupDateTime.AddDays(1)));
            }

            return inputModel;
        }

        public IActionResult Index()
        {
            Models.HomeIndex viewModel = EnsureViewModelHasDefaults(new Models.HomeIndex());

            viewModel.HomeBanners = _context.HomeBanners
                .Include(x => x.Translations.Where(t => t.LanguageId == GetSelectedLanguageId())).Where(x => (!x.ValidFromUtc.HasValue || x.ValidFromUtc <= DateTime.UtcNow) && (!x.ValidUntilUtc.HasValue || x.ValidUntilUtc >= DateTime.UtcNow)).ToList();
            viewModel.Campaigns = _context.Campaigns
                .Include(x => x.Translations.Where(t => t.LanguageId == GetSelectedLanguageId()))
                .Where(x => x.AppliesToBookingsMadeFromUtc <= DateTime.UtcNow && x.AppliesToBookingsMadeUntilUtc >= DateTime.UtcNow && x.IsActive)
                .OrderBy(x => x.ValidFromUtc).ToList();
            viewModel.Processes = _context.Processes
                .Include(x => x.Translations.Where(t => t.LanguageId == GetSelectedLanguageId())).OrderBy(x => x.Id)
                .ToList();
            viewModel.Facts = _context.Facts
                .Include(x => x.Translations.Where(t => t.LanguageId == GetSelectedLanguageId())).OrderBy(x => x.Id)
                .ToList();
            viewModel.ReasonsToChooseUs = _context.ReasonsToChooseUs
                .Include(x => x.Translations.Where(t => t.LanguageId == GetSelectedLanguageId())).OrderBy(x => x.Id)
                .ToList();
            viewModel.Testimonials = _context.Testimonials
                .Include(x => x.Translations.Where(t => t.LanguageId == GetSelectedLanguageId())).ToList();
            viewModel.MostRented = _context.CarSegments
                .Include(x => x.Prices
                    .Where(y => y.Season.StartDateUtc <= DateTime.Today && y.Season.EndDateUtc >= DateTime.Today &&
                                y.Days == 1))
                .Include(x => x.Translations.Where(t => t.LanguageId == GetSelectedLanguageId()))
                .Where(x => x.IsActive)
                .OrderByDescending(x => x.RentCountIncrement + x.Reservations.Count).Take(5).ToList();
            viewModel.SelectedLanguage = _context.Languages.FirstOrDefault(x => x.Id == GetSelectedLanguageId());

            List<Data.Models.Database.PickupReturnLocation> pickupReturnLocations = _context.PickupReturnLocations
                .Include(x => x.Translations.Where(t => t.LanguageId == GetSelectedLanguageId()))
                .Where(x => x.Id > 0)
                .OrderBy(x => x.Translations.FirstOrDefault(t => t.LanguageId == GetSelectedLanguageId()).Name)
                .ToList();
            pickupReturnLocations.Add(_context.PickupReturnLocations
                .Include(x => x.Translations.Where(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese))
                .FirstOrDefault(x => x.Id == -1));

            ViewBag.Locations = new SelectList(pickupReturnLocations
                .Select(x => new
                {
                    x.Id,
                    x.Translations.First().Name
                }), "Id", "Name", viewModel.PickupLocationId);

            BuildViewBag();

			return View(viewModel);
        }

        [HttpPost]
        public IActionResult ChangeLanguage(dCore.MultiLanguage.Models.Language language)
        {
            HttpContext.Session.SetInt32("SelectedLanguage", language.Id);
            _translationProvider.ClearCache();
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new Models.ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("sitemap.xml")]
        public IActionResult Sitemap()
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "sitemap.xml");

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            string sitemapXml = System.IO.File.ReadAllText(filePath);

            return Content(sitemapXml, "application/xml", Encoding.UTF8);
        }
    }
}