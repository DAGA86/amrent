using System.Drawing;
using dCore.MultiLanguage.Models;
using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AMRent.Data.Contexts;
using AMRent.Data.Enums;
using AMRent.Data.Models.Database;

namespace AMRent.BackOffice.Controllers
{
    [Extensions.AuthorizePermission(Data.Enums.Permissions.Other)]
    public class HomeBannersController : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;

        public HomeBannersController(FullDatabaseContext context, ILogger<HomeBannersController> logger, IWebHostEnvironment webHostEnvironment, IConfiguration configuration) : base(context, logger)
        {
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
        }

        [HttpPost]
        public JsonResult Get(Models.DataTables.ViewModel viewModel)
        {
            JsonResult result = new JsonResult("");
            try
            {
                IQueryable<HomeBanner> recordsTotal = _context.HomeBanners;
                var recordsTotalCount = recordsTotal.Count();
                var recordsFiltered = recordsTotal;

                if (viewModel.order.Any())
                {
                    switch (viewModel.order.First().column)
                    {
                        case 0:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.Name)
                                : recordsFiltered.OrderByDescending(x => x.Name);
                            break;
                    }
                }
                if (!string.IsNullOrEmpty(viewModel.search.value))
                {
                    var predicate = PredicateBuilder.New<HomeBanner>();
                    predicate = predicate.Or(x => x.Name.Contains(viewModel.search.value));

                    recordsFiltered = recordsFiltered.Where(predicate);
                }
                var recordsFilteredCount = recordsFiltered.Count();
                var recordsFilteredPage = recordsFiltered.Skip(viewModel.start).Take(viewModel.length).Select(x => new AMRent.Data.Models.View.HomeBannerIndex()
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToArray();

                result = Json(new
                {
                    draw = viewModel.draw,
                    recordsTotal = recordsTotalCount,
                    recordsFiltered = recordsFilteredCount,
                    data = recordsFilteredPage,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            return result;
        }

        private bool ProcessImageFiles(HomeBanner homeBanner)
        {
            string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "img\\homebanners");
            string publicUploadPath = _configuration["FileUploadSettings:HomeBannerImagesUploadPath"];

            List<Language> languages = _context.Languages.ToList();
            foreach (HomeBannerTranslation homeBannerTranslation in homeBanner.Translations)
            {
                Language currentLanguage = languages.FirstOrDefault(x => x.Id == homeBannerTranslation.LanguageId);
                IFormFile? file = HttpContext.Request.Form.Files[$"ImageFile_{currentLanguage.Code}_{currentLanguage.CountryCode}"];
                string filePath = Path.Combine(uploadPath, $"{homeBanner.Id}_{currentLanguage.Code}_{currentLanguage.CountryCode}.jpg");
                string publicFilePath = Path.Combine(publicUploadPath, $"{homeBanner.Id}_{currentLanguage.Code}_{currentLanguage.CountryCode}.jpg");

                if (file != null && file.Length > 0)
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg" };
                    var fileExtension = Path.GetExtension(file.FileName).ToLower();

                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError($"ImageFile_{currentLanguage.Code}_{currentLanguage.CountryCode}", "Please upload a JPG file.");
                        return false;
                    }
                    using (var image = Image.FromStream(file.OpenReadStream()))
                    {
                        if (image.Width != 1922 || image.Height != 600)
                        {
                            ModelState.AddModelError($"ImageFile_{currentLanguage.Code}_{currentLanguage.CountryCode}", "Please upload an image with a resolution of 833px x 573px.");
                            return false;
                        }
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
                        file.CopyTo(stream);
                    }

                    using (var stream = new FileStream(publicFilePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }
                else
                {
                    if (!System.IO.File.Exists(filePath))
                    {
                        ModelState.AddModelError($"ImageFile_{currentLanguage.Code}_{currentLanguage.CountryCode}", "Please select a valid image file.");
                        return false;
                    }

                }
            }
            return true;
        }

        // GET: HomeBanners
        public async Task<IActionResult> Index()
        {
            return View();
        }

        // GET: HomeBanners/Create
        public IActionResult Create()
        {
            ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            return View();
        }

        // POST: HomeBanners/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HomeBanner homeBanner)
        {
            if (ModelState.IsValid)
            {
                _context.Add(homeBanner);
                await _context.SaveChangesAsync();

                if (!ProcessImageFiles(homeBanner))
                {
                    ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
                    return View(nameof(Edit), homeBanner);
                }

                return RedirectToAction(nameof(Index));
            }
            ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            return View(homeBanner);
        }

        // GET: HomeBanners/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.HomeBanners == null)
            {
                return NotFound();
            }

            var homeBanner = await _context.HomeBanners
                .Include(x => x.Translations)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (homeBanner == null)
            {
                return NotFound();
            }
            ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "img\\homebanners");
            foreach (HomeBannerTranslation homeBannerTranslation in homeBanner.Translations)
            {
                string fileName = $"{homeBanner.Id}_{homeBannerTranslation.Language.Code}_{homeBannerTranslation.Language.CountryCode}.jpg";
                string filePath = Path.Combine(uploadPath, fileName);
                if (System.IO.File.Exists(filePath))
                {
                    homeBanner.Translations.FirstOrDefault(x => x.LanguageId == homeBannerTranslation.LanguageId)
                        .ImagePath = $"\\img\\homebanners\\{fileName}";
                }
            }
            return View(homeBanner);
        }

        // POST: HomeBanners/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HomeBanner homeBanner)
        {
            if (id != homeBanner.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(homeBanner);
                    await _context.SaveChangesAsync();

                    if (!ProcessImageFiles(homeBanner))
                    {
                        ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
                        return View(nameof(Edit), homeBanner);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HomeBannerExists(homeBanner.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            List<Language> languages = _context.Languages.OrderBy(x => x.Id).ToList();
            string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "img\\homebanners");
            foreach (HomeBannerTranslation homeBannerTranslation in homeBanner.Translations)
            {
                Language currentLanguage = languages.FirstOrDefault(x => x.Id == homeBannerTranslation.LanguageId);
                string fileName = $"{homeBanner.Id}_{currentLanguage.Code}_{currentLanguage.CountryCode}.jpg";
                string filePath = Path.Combine(uploadPath, fileName);
                if (System.IO.File.Exists(filePath))
                {
                    homeBanner.Translations.FirstOrDefault(x => x.LanguageId == homeBannerTranslation.LanguageId)
                        .ImagePath = $"\\img\\homebanners\\{fileName}";
                }
            }
            ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            return View(homeBanner);
        }

        // GET: HomeBanners/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.HomeBanners == null)
            {
                return NotFound();
            }

            var homeBanner = await _context.HomeBanners
                .FirstOrDefaultAsync(m => m.Id == id);
            if (homeBanner == null)
            {
                return NotFound();
            }

            return View(homeBanner);
        }

        // POST: HomeBanners/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.HomeBanners == null)
            {
                return Problem("Entity set 'FullDatabaseContext.HomeBanners'  is null.");
            }
            var homeBanner = await _context.HomeBanners.FindAsync(id);
            if (homeBanner != null)
            {
                _context.HomeBanners.Remove(homeBanner);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HomeBannerExists(int id)
        {
            return (_context.HomeBanners?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
