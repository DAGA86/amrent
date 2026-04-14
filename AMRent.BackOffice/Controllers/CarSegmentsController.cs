using System.Drawing;
using System.Security.Claims;
using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AMRent.Data.Contexts;
using AMRent.Data.Models.Database;

namespace AMRent.BackOffice.Controllers
{
    [Extensions.AuthorizePermission(Data.Enums.Permissions.Other)]
    public class CarSegmentsController : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;

        public CarSegmentsController(FullDatabaseContext context, ILogger<CarSegmentsController> logger, IWebHostEnvironment webHostEnvironment, IConfiguration configuration) : base(context, logger)
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
                IQueryable<CarSegment> recordsTotal = _context.CarSegments
                    .Include(x => x.Reservations);
                var recordsTotalCount = recordsTotal.Count();
                var recordsFiltered = recordsTotal;

                if (viewModel.order.Any())
                {
                    switch (viewModel.order.First().column)
                    {
                        case 0:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.Code)
                                : recordsFiltered.OrderByDescending(x => x.Code);
                            break;
                        case 1:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Name)
                                : recordsFiltered.OrderByDescending(x => x.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Name);
                            break;
                        case 2:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.Seats)
                                : recordsFiltered.OrderByDescending(x => x.Seats);
                            break;
                        case 3:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.CarFuel.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Name)
                                : recordsFiltered.OrderByDescending(x => x.CarFuel.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Name);
                            break;
                        case 4:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.CarGearbox.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Name)
                                : recordsFiltered.OrderByDescending(x => x.CarGearbox.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Name);
                            break;
                    }
                }
                if (!string.IsNullOrEmpty(viewModel.search.value))
                {
                    var predicate = PredicateBuilder.New<CarSegment>();
                    predicate = predicate.Or(x => x.Code.Contains(viewModel.search.value));
                    predicate = predicate.Or(x => x.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Name.Contains(viewModel.search.value));
                    predicate = predicate.Or(x => x.CarFuel.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Name.Contains(viewModel.search.value));
                    predicate = predicate.Or(x => x.CarGearbox.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Name.Contains(viewModel.search.value));

                    recordsFiltered = recordsFiltered.Where(predicate);
                }
                var recordsFilteredCount = recordsFiltered.Count();
                var recordsFilteredPage = recordsFiltered.Skip(viewModel.start).Take(viewModel.length).Select(x => new AMRent.Data.Models.View.CarSegmentIndex()
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Name,
                    Fuel = x.CarFuel.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Name,
                    Gearbox = x.CarGearbox.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Name,
                    Seats = x.Seats,
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

        private void BuildViewBag(CarSegment carSegment)
        {
            ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            if (carSegment.CarCategoryId > 0)
            {
                if (carSegment.CarCategory == null)
                {
                    carSegment.CarCategory = _context.CarCategories.FirstOrDefault(x => x.Id == carSegment.CarCategoryId);
                }

                if (carSegment.CarCategory.IsCommercial)
                {
                    ViewBag.InsuranceLevels = _context.InsuranceLevels
                        .Include(x => x.Translations.Where(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese))
                        .Where(x => x.Id == 1)
                        .OrderBy(x => x.Id)
                        .ToList();
                }
                else
                {
                    ViewBag.InsuranceLevels = _context.InsuranceLevels
                        .Include(x => x.Translations.Where(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese))
                        .OrderBy(x => x.Id)
                        .ToList();
                }
            }
            
            ViewBag.Categories = new SelectList(_context.CarCategories.Select(x => new { x.Id, x.Translations.FirstOrDefault(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese).Name }), "Id", "Name", carSegment?.CarCategoryId);
            ViewBag.Fuels = new SelectList(_context.CarFuels.Select(x => new { x.Id, x.Translations.FirstOrDefault(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese).Name }), "Id", "Name", carSegment?.CarFuelId);
            ViewBag.Gearboxes = new SelectList(_context.CarGearboxes.Select(x => new { x.Id, x.Translations.FirstOrDefault(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese).Name }), "Id", "Name", carSegment?.CarGearboxId);
        }

        private bool ProcessImageFile(CarSegment carSegment)
        {
            IFormFile? file = HttpContext.Request.Form.Files["ImageFile"];
            IFormFile? listFile = HttpContext.Request.Form.Files["ListImageFile"];
            IFormFile? gridFile = HttpContext.Request.Form.Files["GridImageFile"];
            string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "img\\segments");
            string publicUploadPath = _configuration["FileUploadSettings:SegmentImagesUploadPath"];
            string filePath = Path.Combine(uploadPath, $"{carSegment.Id}.jpg");
            string listFilePath = Path.Combine(uploadPath, $"{carSegment.Id}_list.jpg");
            string gridFilePath = Path.Combine(uploadPath, $"{carSegment.Id}_grid.jpg");
            var allowedExtensions = new[] { ".jpg", ".jpeg" };

            if (file != null && file.Length > 0)
            {
                string publicFilePath = Path.Combine(publicUploadPath, $"{carSegment.Id}.jpg");
                var fileExtension = Path.GetExtension(file.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("ImageFile", "Please upload a JPG file.");
                    return false;
                }
                using (var image = Image.FromStream(file.OpenReadStream()))
                {
                    if (image.Width != 817 || image.Height != 446)
                    {
                        ModelState.AddModelError("ImageFile", "Please upload an image with a resolution of 817px x 446px.");
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
                    ModelState.AddModelError("ImageFile", "Please select a valid image file.");
                    return false;
                }

            }
            if (listFile != null && listFile.Length > 0)
            {
                string listPublicFilePath = Path.Combine(publicUploadPath, $"{carSegment.Id}_list.jpg");
                var fileExtension = Path.GetExtension(listFile.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("ListImageFile", "Please upload a JPG file.");
                    return false;
                }
                using (var image = Image.FromStream(listFile.OpenReadStream()))
                {
                    if (image.Width != 275 || image.Height != 219)
                    {
                        ModelState.AddModelError("ListImageFile", "Please upload an image with a resolution of 275px x 219px.");
                        return false;
                    }
                }

                if (System.IO.File.Exists(listFilePath))
                {
                    System.IO.File.Delete(listFilePath);
                }

                if (System.IO.File.Exists(listPublicFilePath))
                {
                    System.IO.File.Delete(listPublicFilePath);
                }

                using (var stream = new FileStream(listFilePath, FileMode.Create))
                {
                    listFile.CopyTo(stream);
                }

                using (var stream = new FileStream(listPublicFilePath, FileMode.Create))
                {
                    listFile.CopyTo(stream);
                }
            }
            else
            {
                if (!System.IO.File.Exists(listFilePath))
                {
                    ModelState.AddModelError("ListImageFile", "Please select a valid image file.");
                    return false;
                }

            }
            if (gridFile != null && gridFile.Length > 0)
            {
                string gridPublicFilePath = Path.Combine(publicUploadPath, $"{carSegment.Id}_grid.jpg");
                var fileExtension = Path.GetExtension(gridFile.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("GridImageFile", "Please upload a JPG file.");
                    return false;
                }
                using (var image = Image.FromStream(gridFile.OpenReadStream()))
                {
                    if (image.Width != 550 || image.Height != 401)
                    {
                        ModelState.AddModelError("GridImageFile", "Please upload an image with a resolution of 550px x 401px.");
                        return false;
                    }
                }

                if (System.IO.File.Exists(gridFilePath))
                {
                    System.IO.File.Delete(gridFilePath);
                }

                if (System.IO.File.Exists(gridPublicFilePath))
                {
                    System.IO.File.Delete(gridPublicFilePath);
                }

                using (var stream = new FileStream(gridFilePath, FileMode.Create))
                {
                    gridFile.CopyTo(stream);
                }

                using (var stream = new FileStream(gridPublicFilePath, FileMode.Create))
                {
                    gridFile.CopyTo(stream);
                }
            }
            else
            {
                if (!System.IO.File.Exists(gridFilePath))
                {
                    ModelState.AddModelError("GridImageFile", "Please select a valid image file.");
                    return false;
                }

            }
            return true;
        }

        // GET: CarSegments
        public async Task<IActionResult> Index()
        {
            return View();
        }

        // GET: CarSegments/Create
        public IActionResult Create()
        {
            CarSegment carSegment = new();
            BuildViewBag(carSegment);
            return View(carSegment);
        }

        // POST: CarSegments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CarSegment carSegment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(carSegment);
                await _context.SaveChangesAsync();

                if (!ProcessImageFile(carSegment))
                {
                    BuildViewBag(carSegment);
                    return View(nameof(Edit), carSegment);
                }

                return RedirectToAction(nameof(Edit), new { id = carSegment.Id});
            }
            BuildViewBag(carSegment);
            return View(carSegment);
        }

        // GET: CarSegments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.CarSegments == null)
            {
                return NotFound();
            }

            var carSegment = await _context.CarSegments
                .Include(x => x.CarCategory)
                .Include(x => x.Translations)
                .Include(x => x.Insurances)
                    .ThenInclude(y => y.Changes)
                        .ThenInclude(z => z.User)
                .Include(x => x.Insurances)
                    .ThenInclude(y => y.Prices.OrderBy(z => z.Days))
                .Include(x => x.CarCategory)
                .Include(x => x.Reservations)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (carSegment == null)
            {
                return NotFound();
            }

            BuildViewBag(carSegment);
            string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "img\\segments");
            string filePath = Path.Combine(uploadPath, $"{carSegment.Id}.jpg");
            if (System.IO.File.Exists(filePath))
            {
                carSegment.ImagePath = $"\\img\\segments\\{carSegment.Id}.jpg";
            }
            string listFilePath = Path.Combine(uploadPath, $"{carSegment.Id}_list.jpg");
            if (System.IO.File.Exists(listFilePath))
            {
                carSegment.ListImagePath = $"\\img\\segments\\{carSegment.Id}_list.jpg";
            }
            string gridFilePath = Path.Combine(uploadPath, $"{carSegment.Id}_grid.jpg");
            if (System.IO.File.Exists(gridFilePath))
            {
                carSegment.GridImagePath = $"\\img\\segments\\{carSegment.Id}_grid.jpg";
            }
            return View(carSegment);
        }

        // POST: CarSegments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CarSegment carSegment)
        {
            if (id != carSegment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.UpdateWithTracking(carSegment, Guid.Parse(User.Claims.First(x => x.Type == ClaimTypes.UserData).Value));
                    await _context.SaveChangesAsync();

                    if (!ProcessImageFile(carSegment))
                    {
                        BuildViewBag(carSegment);
                        return View(carSegment);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarSegmentExists(carSegment.Id))
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
            BuildViewBag(carSegment);
            string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "img\\segments");
            string filePath = Path.Combine(uploadPath, $"{carSegment.Id}.jpg");
            if (System.IO.File.Exists(filePath))
            {
                carSegment.ImagePath = $"\\img\\segments\\{carSegment.Id}.jpg";
            }
            string listFilePath = Path.Combine(uploadPath, $"{carSegment.Id}_list.jpg");
            if (System.IO.File.Exists(listFilePath))
            {
                carSegment.ListImagePath = $"\\img\\segments\\{carSegment.Id}_list.jpg";
            }
            string gridFilePath = Path.Combine(uploadPath, $"{carSegment.Id}_grid.jpg");
            if (System.IO.File.Exists(gridFilePath))
            {
                carSegment.GridImagePath = $"\\img\\segments\\{carSegment.Id}_grid.jpg";
            }
            var databaseCarSegment = await _context.CarSegments
                .Include(x => x.CarCategory)
                .Include(x => x.Translations)
                .Include(x => x.Insurances)
                    .ThenInclude(y => y.Changes)
                        .ThenInclude(z => z.User)
                .Include(x => x.Insurances)
                    .ThenInclude(y => y.Prices.OrderBy(z => z.Days))
                .Include(x => x.CarCategory)
                .Include(x => x.Reservations)
                .FirstOrDefaultAsync(x => x.Id == id);

            carSegment.Insurances.ForEach(x => x.Changes = databaseCarSegment.Insurances.FirstOrDefault(y => y.InsuranceLevelId == x.InsuranceLevelId).Changes);

            return View(carSegment);
        }

        // GET: CarSegments/Deactivate/5
        public async Task<IActionResult> Deactivate(int? id)
        {
            if (id == null || _context.CarSegments == null)
            {
                return NotFound();
            }

            var carSegment = await _context.CarSegments
                .FirstOrDefaultAsync(x => x.Id == id);
            if (carSegment == null)
            {
                return NotFound();
            }

            carSegment.IsActive = false;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: CarSegments/Activate/5
        public async Task<IActionResult> Activate(int? id)
        {
            if (id == null || _context.CarSegments == null)
            {
                return NotFound();
            }

            var carSegment = await _context.CarSegments
                .FirstOrDefaultAsync(x => x.Id == id);
            if (carSegment == null)
            {
                return NotFound();
            }

            carSegment.IsActive = true;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool CarSegmentExists(int id)
        {
            return (_context.CarSegments?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
