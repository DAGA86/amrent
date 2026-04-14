using System.Drawing;
using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AMRent.Data.Contexts;
using AMRent.Data.Models.Database;

namespace AMRent.BackOffice.Controllers
{
    [Extensions.AuthorizePermission(Data.Enums.Permissions.Other)]
    public class BlogArticlesController : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;

        public BlogArticlesController(FullDatabaseContext context, ILogger<BlogArticlesController> logger, IWebHostEnvironment webHostEnvironment, IConfiguration configuration) : base(context, logger)
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
                IQueryable<BlogArticle> recordsTotal = _context.BlogArticles;
                var recordsTotalCount = recordsTotal.Count();
                var recordsFiltered = recordsTotal;

                if (viewModel.order.Any())
                {
                    switch (viewModel.order.First().column)
                    {
                        case 0:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Title)
                                : recordsFiltered.OrderByDescending(x => x.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Title);
                            break;
                    }
                }
                if (!string.IsNullOrEmpty(viewModel.search.value))
                {
                    var predicate = PredicateBuilder.New<BlogArticle>();
                    predicate = predicate.Or(x => x.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Title.Contains(viewModel.search.value));

                    recordsFiltered = recordsFiltered.Where(predicate);
                }
                var recordsFilteredCount = recordsFiltered.Count();
                var recordsFilteredPage = recordsFiltered.Skip(viewModel.start).Take(viewModel.length).Select(x => new AMRent.Data.Models.View.BlogArticleIndex()
                {
                    Id = x.Id,
                    Title = x.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Title
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

        private bool ProcessImageFiles(BlogArticle blogArticle)
        {
            IFormFile? file = HttpContext.Request.Form.Files["ImageFile"];
            IFormFile? topFile = HttpContext.Request.Form.Files["TopImageFile"];
            IFormFile? listFile = HttpContext.Request.Form.Files["ListImageFile"];
            string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "img\\blog");
            string publicUploadPath = _configuration["FileUploadSettings:BlogImagesUploadPath"];
            string filePath = Path.Combine(uploadPath, $"{blogArticle.Id}.jpg");
            string topFilePath = Path.Combine(uploadPath, $"{blogArticle.Id}_top.jpg");
            string listFilePath = Path.Combine(uploadPath, $"{blogArticle.Id}_list.jpg");
            string publicFilePath = Path.Combine(publicUploadPath, $"{blogArticle.Id}.jpg");
            string publicTopFilePath = Path.Combine(publicUploadPath, $"{blogArticle.Id}_top.jpg");
            string publicListFilePath = Path.Combine(publicUploadPath, $"{blogArticle.Id}_list.jpg");

            if (file != null && file.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg" };
                var fileExtension = Path.GetExtension(file.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("ImageFile", "Please upload a JPG file.");
                    return false;
                }
                using (var image = Image.FromStream(file.OpenReadStream()))
                {
                    if (image.Width != 640 || image.Height != 427)
                    {
                        ModelState.AddModelError("ImageFile", "Please upload an image with a resolution of 640px x 427px.");
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

            if (listFile != null && listFile.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg" };
                var fileExtension = Path.GetExtension(listFile.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("ListImageFile", "Please upload a JPG file.");
                    return false;
                }
                using (var image = Image.FromStream(listFile.OpenReadStream()))
                {
                    if (image.Width != 833 || image.Height != 573)
                    {
                        ModelState.AddModelError("ListImageFile", "Please upload an image with a resolution of 833px x 573px.");
                        return false;
                    }
                }

                if (System.IO.File.Exists(listFilePath))
                {
                    System.IO.File.Delete(listFilePath);
                }

                if (System.IO.File.Exists(publicListFilePath))
                {
                    System.IO.File.Delete(publicListFilePath);
                }

                using (var stream = new FileStream(listFilePath, FileMode.Create))
                {
                    listFile.CopyTo(stream);
                }

                using (var stream = new FileStream(publicListFilePath, FileMode.Create))
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
            return true;
        }

        // GET: BlogArticles
        public async Task<IActionResult> Index()
        {
            return View();
        }

        // GET: BlogArticles/Create
        public IActionResult Create()
        {
            ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            ViewBag.Categories = new SelectList(_context.BlogArticleCategories.Select(x => new { x.Id, x.Translations.FirstOrDefault(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese).Name }), "Id", "Name");
            return View();
        }

        // POST: BlogArticles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BlogArticle blogArticle)
        {
            if (ModelState.IsValid)
            {
                _context.Add(blogArticle);
                await _context.SaveChangesAsync();

                if (!ProcessImageFiles(blogArticle))
                {
                    ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
                    ViewBag.Categories = new SelectList(_context.BlogArticleCategories.Select(x => new { x.Id, x.Translations.FirstOrDefault(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese).Name }), "Id", "Name", blogArticle.BlogArticleCategoryId);
                    return View(nameof(Edit), blogArticle);
                }

                return RedirectToAction(nameof(Index));
            }
            ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            ViewBag.Categories = new SelectList(_context.BlogArticleCategories.Select(x => new { x.Id, x.Translations.FirstOrDefault(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese).Name }), "Id", "Name", blogArticle.BlogArticleCategoryId);
            return View(blogArticle);
        }

        // GET: BlogArticles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.BlogArticles == null)
            {
                return NotFound();
            }

            var blogArticle = await _context.BlogArticles
                .Include(x => x.Translations)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (blogArticle == null)
            {
                return NotFound();
            }
            ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            ViewBag.Categories = new SelectList(_context.BlogArticleCategories.Select(x => new { x.Id, x.Translations.FirstOrDefault(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese).Name }), "Id", "Name", blogArticle.BlogArticleCategoryId);
            string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "img\\blog");
            string filePath = Path.Combine(uploadPath, $"{blogArticle.Id}.jpg");
            if (System.IO.File.Exists(filePath))
            {
                blogArticle.ImagePath = $"\\img\\blog\\{blogArticle.Id}.jpg";
            }
            string topFilePath = Path.Combine(uploadPath, $"{blogArticle.Id}_top.jpg");
            if (System.IO.File.Exists(topFilePath))
            {
                blogArticle.TopImagePath = $"\\img\\blog\\{blogArticle.Id}_top.jpg";
            }
            string listFilePath = Path.Combine(uploadPath, $"{blogArticle.Id}_list.jpg");
            if (System.IO.File.Exists(listFilePath))
            {
                blogArticle.ListImagePath = $"\\img\\blog\\{blogArticle.Id}_list.jpg";
            }
            return View(blogArticle);
        }

        // POST: BlogArticles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BlogArticle blogArticle)
        {
            if (id != blogArticle.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(blogArticle);
                    await _context.SaveChangesAsync();

                    if (!ProcessImageFiles(blogArticle))
                    {
                        ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
                        ViewBag.Categories = new SelectList(_context.BlogArticleCategories.Select(x => new { x.Id, x.Translations.FirstOrDefault(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese).Name }), "Id", "Name", blogArticle.BlogArticleCategoryId);
                        return View(nameof(Edit), blogArticle);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogArticleExists(blogArticle.Id))
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
            ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            ViewBag.Categories = new SelectList(_context.BlogArticleCategories.Select(x => new { x.Id, x.Translations.FirstOrDefault(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese).Name }), "Id", "Name", blogArticle.BlogArticleCategoryId);
            string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "img\\blog");
            string filePath = Path.Combine(uploadPath, $"{blogArticle.Id}.jpg");
            if (System.IO.File.Exists(filePath))
            {
                blogArticle.ImagePath = $"\\img\\blog\\{blogArticle.Id}.jpg";
            }
            string topFilePath = Path.Combine(uploadPath, $"{blogArticle.Id}_top.jpg");
            if (System.IO.File.Exists(topFilePath))
            {
                blogArticle.TopImagePath = $"\\img\\blog\\{blogArticle.Id}_top.jpg";
            }
            string listFilePath = Path.Combine(uploadPath, $"{blogArticle.Id}_list.jpg");
            if (System.IO.File.Exists(listFilePath))
            {
                blogArticle.ListImagePath = $"\\img\\blog\\{blogArticle.Id}_list.jpg";
            }
            return View(blogArticle);
        }

        // GET: BlogArticles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.BlogArticles == null)
            {
                return NotFound();
            }

            var blogArticle = await _context.BlogArticles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blogArticle == null)
            {
                return NotFound();
            }

            _context.BlogArticles.Remove(blogArticle);
            await _context.SaveChangesAsync();

            return View(nameof(Index));
        }

        // GET: BlogArticles/DeleteImage/5
        public IActionResult DeleteImage(int id)
        {
            string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "img\\blog");
            string publicUploadPath = _configuration["FileUploadSettings:BlogImagesUploadPath"];
            string filePath = Path.Combine(uploadPath, $"{id}.jpg");
            string publicFilePath = Path.Combine(publicUploadPath, $"{id}.jpg");

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            if (System.IO.File.Exists(publicFilePath))
            {
                System.IO.File.Delete(publicFilePath);
            }

            return RedirectToAction(nameof(Edit), new { id = id });
        }

        private bool BlogArticleExists(int id)
        {
            return (_context.BlogArticles?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
