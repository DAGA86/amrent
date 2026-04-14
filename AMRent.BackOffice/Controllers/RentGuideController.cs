using dCore.MultiLanguage.Models;
using Microsoft.AspNetCore.Mvc;
using AMRent.BackOffice.Models;
using AMRent.Data.Contexts;

namespace AMRent.BackOffice.Controllers
{
    [Extensions.AuthorizePermission(Data.Enums.Permissions.Other)]
    public class RentGuideController : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;

        public RentGuideController(FullDatabaseContext context, ILogger<RentGuideController> logger, IWebHostEnvironment webHostEnvironment, IConfiguration configuration) : base(context, logger)
        {
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
        }

        // GET: RentGuide/Edit
        public async Task<IActionResult> Edit()
        {
            ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            return View();
        }

        // POST: RentGuide/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RentGuideViewModel model)
        {
            List<Language> languages = _context.Languages.ToList();

            string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "docs");
            string publicUploadPath = _configuration["FileUploadSettings:DocumentsUploadPath"];
            foreach (RentGuideEdit rentGuide in model.RentGuides)
            {
                Language currentLanguage = languages.FirstOrDefault(x => x.Id == rentGuide.LanguageId);
                string filePath = Path.Combine(uploadPath, $"rent_guide_{currentLanguage.Code}_{currentLanguage.CountryCode}.pdf");
                if (rentGuide.File != null && rentGuide.File.Length > 0)
                {
                    string publicFilePath = Path.Combine(publicUploadPath, $"rent_guide_{currentLanguage.Code}_{currentLanguage.CountryCode}.pdf");

                    var allowedExtensions = new[] { ".pdf" };
                    var fileExtension = Path.GetExtension(rentGuide.File.FileName).ToLower();

                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("", "Please upload only PDF files.");
                        ViewBag.Languages = languages.OrderBy(x => x.Id).ToList();
                        return View();
                    }

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }

                    if (System.IO.File.Exists(publicFilePath))
                    {
                        System.IO.File.Delete(publicFilePath);
                    }

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        rentGuide.File.CopyTo(stream);
                    }

                    using (var stream = new FileStream(publicFilePath, FileMode.Create))
                    {
                        rentGuide.File.CopyTo(stream);
                    }
                }
                else
                {
                    if (!System.IO.File.Exists(filePath))
                    {
                        ModelState.AddModelError("", "Please select a valid pdf file.");
                        ViewBag.Languages = languages.OrderBy(x => x.Id).ToList();
                        return View();
                    }

                }
            }
            ViewBag.Languages = languages.OrderBy(x => x.Id).ToList();
            return View();
        }
    }
}
