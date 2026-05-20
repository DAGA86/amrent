using System.Drawing;
using dCore.MultiLanguage.Models;
using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AMRent.Data.Contexts;
using AMRent.Data.Models.Database;

namespace AMRent.BackOffice.Controllers
{
    [Extensions.AuthorizePermission(Data.Enums.Permissions.Other)]
    public class CampaignsController : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;

        public CampaignsController(FullDatabaseContext context, ILogger<CampaignsController> logger, IWebHostEnvironment webHostEnvironment, IConfiguration configuration) : base(context, logger)
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
                IQueryable<Campaign> recordsTotal = _context.Campaigns
                    .Include(x => x.Reservations);
                var recordsTotalCount = recordsTotal.Count();
                var recordsFiltered = recordsTotal;

                if (viewModel.order.Any())
                {
                    switch (viewModel.order.First().column)
                    {
                        case 0:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Name)
                                : recordsFiltered.OrderByDescending(x => x.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Name);
                            break;
                        case 1:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.DiscountType)
                                : recordsFiltered.OrderByDescending(x => x.DiscountType);
                            break;
                        case 2:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.Value)
                                : recordsFiltered.OrderByDescending(x => x.Value);
                            break;
                        case 4:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.ValidFromUtc)
                                : recordsFiltered.OrderByDescending(x => x.ValidFromUtc);
                            break;
                        case 5:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.ValidUntilUtc)
                                : recordsFiltered.OrderByDescending(x => x.ValidUntilUtc);
                            break;
                    }
                }
                if (!string.IsNullOrEmpty(viewModel.search.value))
                {
                    var predicate = PredicateBuilder.New<Campaign>();
                    predicate = predicate.Or(x => x.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Name.Contains(viewModel.search.value));

                    recordsFiltered = recordsFiltered.Where(predicate);
                }
                var recordsFilteredCount = recordsFiltered.Count();
                var recordsFilteredPage = recordsFiltered.Skip(viewModel.start).Take(viewModel.length).Select(x => new AMRent.Data.Models.View.CampaignIndex()
                {
                    Id = x.Id,
                    Name = x.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Name,
                    DiscountType = Data.Enums.Generic.GetDescription(x.DiscountType),
                    Value = x.Value,
                    Extras = string.Join(',', x.Extras.Select(e => e.Translations.FirstOrDefault(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese).Name)),
                    ValidFromUtc = x.ValidFromUtc,
                    ValidUntilUtc = x.ValidUntilUtc,
                    IsActive = x.IsActive,
                    HasReservations = x.Reservations.Any()
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

        private bool ProcessImageFile(Campaign campaign)
        {
            IFormFile? topFile = HttpContext.Request.Form.Files["TopImageFile"];
            string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "img\\campaigns");
            string publicUploadPath = _configuration["FileUploadSettings:CampaignImagesUploadPath"];

            string topFilePath = Path.Combine(uploadPath, $"{campaign.Id}_top.jpg");
            string publicTopFilePath = Path.Combine(publicUploadPath, $"{campaign.Id}_top.jpg");

            if (topFile != null && topFile.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg" };
                var fileExtension = Path.GetExtension(topFile.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("TopImageFile", "Please upload a JPG file.");
                    return false;
                }
                using (var image = Image.FromStream(topFile.OpenReadStream()))
                {
                    if (image.Width != 1920 || image.Height != 600)
                    {
                        ModelState.AddModelError("TopImageFile", "Please upload an image with a resolution of 1920px x 600px.");
                        return false;
                    }
                }

                if (System.IO.File.Exists(topFilePath))
                {
                    System.IO.File.Delete(topFilePath);
                }

                if (System.IO.File.Exists(publicTopFilePath))
                {
                    System.IO.File.Delete(publicTopFilePath);
                }

                using (var stream = new FileStream(topFilePath, FileMode.Create))
                {
                    topFile.CopyTo(stream);
                }

                using (var stream = new FileStream(publicTopFilePath, FileMode.Create))
                {
                    topFile.CopyTo(stream);
                }
            }
            else
            {
                if (!System.IO.File.Exists(topFilePath))
                {
                    ModelState.AddModelError("TopImageFile", "Please select a valid image file.");
                    return false;
                }

            }

            List<Language> languages = _context.Languages.ToList();
            foreach (CampaignTranslation campaignTranslation in campaign.Translations)
            {
                Language currentLanguage = languages.FirstOrDefault(x => x.Id == campaignTranslation.LanguageId);
                IFormFile? file = HttpContext.Request.Form.Files[$"ImageFile_{currentLanguage.Code}_{currentLanguage.CountryCode}"];
                string filePath = Path.Combine(uploadPath, $"{campaign.Id}_{currentLanguage.Code}_{currentLanguage.CountryCode}.jpg");
                string publicFilePath = Path.Combine(publicUploadPath, $"{campaign.Id}_{currentLanguage.Code}_{currentLanguage.CountryCode}.jpg");

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
                        if (image.Width != 833 || image.Height != 573)
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

                IFormFile? popupFile = HttpContext.Request.Form.Files[$"PopupImageFile_{currentLanguage.Code}_{currentLanguage.CountryCode}"];
                string popupFilePath = Path.Combine(uploadPath, $"{campaign.Id}_popup_{currentLanguage.Code}_{currentLanguage.CountryCode}.jpg");
                string popupPublicFilePath = Path.Combine(publicUploadPath, $"{campaign.Id}_popup_{currentLanguage.Code}_{currentLanguage.CountryCode}.jpg");

                if (popupFile != null && popupFile.Length > 0)
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg" };
                    var fileExtension = Path.GetExtension(popupFile.FileName).ToLower();

                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError($"PopupImageFile_{currentLanguage.Code}_{currentLanguage.CountryCode}", "Please upload a JPG file.");
                        return false;
                    }
                    using (var image = Image.FromStream(popupFile.OpenReadStream()))
                    {
                        if (image.Width != 1080 || image.Height != 1080)
                        {
                            ModelState.AddModelError($"PopupImageFile_{currentLanguage.Code}_{currentLanguage.CountryCode}", "Please upload an image with a resolution of 1080px x 1080px.");
                            return false;
                        }
                    }

                    if (System.IO.File.Exists(popupFilePath))
                    {
                        System.IO.File.Delete(popupFilePath);
                    }

                    if (System.IO.File.Exists(popupPublicFilePath))
                    {
                        System.IO.File.Delete(popupPublicFilePath);
                    }

                    using (var stream = new FileStream(popupFilePath, FileMode.Create))
                    {
                        popupFile.CopyTo(stream);
                    }

                    using (var stream = new FileStream(popupPublicFilePath, FileMode.Create))
                    {
                        popupFile.CopyTo(stream);
                    }
                }
                else
                {
                    if (!System.IO.File.Exists(popupFilePath))
                    {
                        ModelState.AddModelError($"PopupImageFile_{currentLanguage.Code}_{currentLanguage.CountryCode}", "Please select a valid image file.");
                        return false;
                    }

                }
            }

            return true;
        }

        // GET: Campaigns
        public async Task<IActionResult> Index()
        {
            return View();
        }

        // GET: Campaigns/Create
        public IActionResult Create()
        {
            ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            ViewBag.Segments = new MultiSelectList(_context.CarSegments.Select(x => new { x.Id, x.Code }), "Id", "Code");
            ViewBag.Extras = new MultiSelectList(_context.Extras.Select(x => new { x.Id, x.Translations.FirstOrDefault(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese).Name }), "Id", "Name");
            return View();
        }

        // POST: Campaigns/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Campaign campaign)
        {
            if (ModelState.IsValid)
            {
                foreach (int segmentId in campaign.SegmentIds)
                {
                    campaign.CarSegments.Add(_context.CarSegments.FirstOrDefault(x => x.Id == segmentId));
                }
                foreach (int extraId in campaign.ExtraIds)
                {
                    campaign.Extras.Add(_context.Extras.FirstOrDefault(x => x.Id == extraId));
                }
                _context.Add(campaign);
                await _context.SaveChangesAsync();

                if (!ProcessImageFile(campaign))
                {
                    ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
                    ViewBag.Segments = new MultiSelectList(_context.CarSegments.Select(x => new { x.Id, x.Code }), "Id", "Code", campaign.CarSegments.Select(x => x.Id));
                    ViewBag.Extras = new MultiSelectList(_context.Extras.Select(x => new { x.Id, x.Translations.FirstOrDefault(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese).Name }), "Id", "Name", campaign.Extras.Select(x => x.Id));
                    return View(nameof(Edit), campaign);
                }

                return RedirectToAction(nameof(Index));
            }
            ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            ViewBag.Segments = new MultiSelectList(_context.CarSegments.Select(x => new { x.Id, x.Code }), "Id", "Code", campaign.CarSegments.Select(x => x.Id));
            ViewBag.Extras = new MultiSelectList(_context.Extras.Select(x => new { x.Id, x.Translations.FirstOrDefault(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese).Name }), "Id", "Name", campaign.Extras.Select(x => x.Id));
            return View(campaign);
        }

        // GET: Campaigns/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Campaigns == null)
            {
                return NotFound();
            }

            var campaign = await _context.Campaigns
                .Include(x => x.Translations)
                .Include(x => x.CarSegments)
                .Include(x => x.Extras)
                .Include(x => x.Reservations)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (campaign == null || campaign.Reservations.Any())
            {
                return NotFound();
            }
            List<Language> languages = _context.Languages.OrderBy(x => x.Id).ToList();
            string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "img\\campaigns");
            string topFilePath = Path.Combine(uploadPath, $"{campaign.Id}_top.jpg");
            if (System.IO.File.Exists(topFilePath))
            {
                campaign.TopImagePath = $"\\img\\campaigns\\{campaign.Id}_top.jpg";
            }
            foreach (CampaignTranslation campaignTranslation in campaign.Translations)
            {
                string fileName = $"{campaign.Id}_{campaignTranslation.Language.Code}_{campaignTranslation.Language.CountryCode}.jpg";
                string filePath = Path.Combine(uploadPath, fileName);
                if (System.IO.File.Exists(filePath))
                {
                    campaign.Translations.FirstOrDefault(x => x.LanguageId == campaignTranslation.LanguageId)
                        .ImagePath = $"\\img\\campaigns\\{fileName}";
                }
                string popupFileName = $"{campaign.Id}_popup_{campaignTranslation.Language.Code}_{campaignTranslation.Language.CountryCode}.jpg";
                string popupFilePath = Path.Combine(uploadPath, popupFileName);
                if (System.IO.File.Exists(popupFilePath))
                {
                    campaign.Translations.FirstOrDefault(x => x.LanguageId == campaignTranslation.LanguageId)
                        .PopupImagePath = $"\\img\\campaigns\\{popupFileName}";
                }
            }
            ViewBag.Languages = languages;
            ViewBag.Segments = new MultiSelectList(_context.CarSegments.Select(x => new { x.Id, x.Code }), "Id", "Code");
            campaign.SegmentIds = campaign.CarSegments.Select(x => x.Id).ToList();
            ViewBag.Extras = new MultiSelectList(_context.Extras.Select(x => new { x.Id, x.Translations.FirstOrDefault(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese).Name }), "Id", "Name");
            campaign.ExtraIds = campaign.Extras.Select(x => x.Id).ToList();
            return View(campaign);
        }

        // POST: Campaigns/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Campaign campaign)
        {
            if (id != campaign.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.CampaignCarSegments.RemoveRange(_context.CampaignCarSegments.Where(x => x.CampaignId == campaign.Id));
                    foreach (int segmentId in campaign.SegmentIds)
                    {
                        _context.CampaignCarSegments.Add(new CampaignCarSegment() { CampaignId = campaign.Id, CarSegmentId = segmentId });
                    }
                    _context.CampaignExtras.RemoveRange(_context.CampaignExtras.Where(x => x.CampaignId == campaign.Id));
                    foreach (int extraId in campaign.ExtraIds)
                    {
                        _context.CampaignExtras.Add(new CampaignExtra() { CampaignId = campaign.Id, ExtraId = extraId });
                    }
                    _context.Update(campaign);
                    await _context.SaveChangesAsync();
                    foreach (int segmentId in campaign.SegmentIds)
                    {
                        campaign.CarSegments.Add(_context.CarSegments.FirstOrDefault(x => x.Id == segmentId));
                    }
                    foreach (int extraId in campaign.ExtraIds)
                    {
                        campaign.Extras.Add(_context.Extras.FirstOrDefault(x => x.Id == extraId));
                    }
                    await _context.SaveChangesAsync();

                    if (!ProcessImageFile(campaign))
                    {
                        campaign = _context.Campaigns.FirstOrDefault(x => x.Id == campaign.Id);
                        ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
                        ViewBag.Segments = new MultiSelectList(_context.CarSegments.Select(x => new { x.Id, x.Code }), "Id", "Code", campaign.CarSegments.Select(x => x.Id));
                        ViewBag.Extras = new MultiSelectList(_context.Extras.Select(x => new { x.Id, x.Translations.FirstOrDefault(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese).Name }), "Id", "Name", campaign.Extras.Select(x => x.Id));
                        return View(campaign);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CampaignExists(campaign.Id))
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
            string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "img\\campaigns");
            string topFilePath = Path.Combine(uploadPath, $"{campaign.Id}_top.jpg");
            if (System.IO.File.Exists(topFilePath))
            {
                campaign.TopImagePath = $"\\img\\campaigns\\{campaign.Id}_top.jpg";
            }
            foreach (CampaignTranslation campaignTranslation in campaign.Translations)
            {
                Language currentLanguage = languages.FirstOrDefault(x => x.Id == campaignTranslation.LanguageId);
                string fileName = $"{campaign.Id}_{currentLanguage.Code}_{currentLanguage.CountryCode}.jpg";
                string filePath = Path.Combine(uploadPath, fileName);
                if (System.IO.File.Exists(filePath))
                {
                    campaign.Translations.FirstOrDefault(x => x.LanguageId == campaignTranslation.LanguageId)
                        .ImagePath = $"\\img\\campaigns\\{fileName}";
                }
                string popupFileName = $"{campaign.Id}_popup_{campaignTranslation.Language.Code}_{campaignTranslation.Language.CountryCode}.jpg";
                string popupFilePath = Path.Combine(uploadPath, popupFileName);
                if (System.IO.File.Exists(popupFilePath))
                {
                    campaign.Translations.FirstOrDefault(x => x.LanguageId == campaignTranslation.LanguageId)
                        .PopupImagePath = $"\\img\\campaigns\\{popupFileName}";
                }
            }
            ViewBag.Languages = languages;
            ViewBag.Segments = new MultiSelectList(_context.CarSegments.Select(x => new { x.Id, x.Code }), "Id", "Code", campaign.CarSegments.Select(x => x.Id));
            ViewBag.Extras = new MultiSelectList(_context.Extras.Select(x => new { x.Id, x.Translations.FirstOrDefault(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese).Name }), "Id", "Name", campaign.Extras.Select(x => x.Id));
            return View(campaign);
        }

        // GET: Campaigns/Deactivate/5
        public async Task<IActionResult> Deactivate(int? id)
        {
            if (id == null || _context.Campaigns == null)
            {
                return NotFound();
            }

            var campaign = await _context.Campaigns
                .FirstOrDefaultAsync(x => x.Id == id);
            if (campaign == null)
            {
                return NotFound();
            }

            campaign.IsActive = false;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Campaigns/Activate/5
        public async Task<IActionResult> Activate(int? id)
        {
            if (id == null || _context.Campaigns == null)
            {
                return NotFound();
            }

            var campaign = await _context.Campaigns
                .FirstOrDefaultAsync(x => x.Id == id);
            if (campaign == null)
            {
                return NotFound();
            }

            campaign.IsActive = true;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Campaigns/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Campaigns == null)
            {
                return NotFound();
            }

            var campaign = await _context.Campaigns
                .FirstOrDefaultAsync(m => m.Id == id);
            if (campaign == null)
            {
                return NotFound();
            }

            _context.Campaigns.Remove(campaign);
            await _context.SaveChangesAsync();

            return View(nameof(Index));
        }

        private bool CampaignExists(int id)
        {
            return (_context.Campaigns?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
