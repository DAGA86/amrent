using dCore.MultiLanguage.Models;
using Microsoft.AspNetCore.Mvc;
using AMRent.Data.Contexts;

namespace AMRent.Website.Controllers
{
    public class RentGuideController : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public RentGuideController(ILogger<HomeController> logger, FullDatabaseContext context, dCore.MultiLanguage.Providers.TranslationProvider translationProvider, IWebHostEnvironment webHostEnvironment) : base(logger, context, translationProvider)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            int selectedLanguageId = GetSelectedLanguageId();

            Language selectedLanguage = _context.Languages.FirstOrDefault(x => x.Id == selectedLanguageId);
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, $"docs\\rent_guide_{selectedLanguage.Code}_{selectedLanguage.CountryCode}.pdf");
            
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }
            string contentType = "application/pdf";
            string fileDownloadName = $"{_translationProvider.GetTranslation(selectedLanguageId, "Ficheiro.GuiaAluguer.Nome")}.pdf";
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, contentType, fileDownloadName);
        }
    }
}