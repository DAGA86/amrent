using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AMRent.Data.Contexts;
using AMRent.Data.Models.Database;

namespace AMRent.BackOffice.Controllers
{
    [Extensions.AuthorizePermission(Data.Enums.Permissions.Other)]
    public class BlogArticleCategoriesController : BaseController
    {
        public BlogArticleCategoriesController(FullDatabaseContext context, ILogger<BlogArticleCategoriesController> logger) : base(context, logger)
        {
        }

        [HttpPost]
        public JsonResult Get(Models.DataTables.ViewModel viewModel)
        {
            JsonResult result = new JsonResult("");
            try
            {
                IQueryable<BlogArticleCategory> recordsTotal = _context.BlogArticleCategories;
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
                    }
                }
                if (!string.IsNullOrEmpty(viewModel.search.value))
                {
                    var predicate = PredicateBuilder.New<BlogArticleCategory>();
                    predicate = predicate.Or(x => x.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Name.Contains(viewModel.search.value));

                    recordsFiltered = recordsFiltered.Where(predicate);
                }
                var recordsFilteredCount = recordsFiltered.Count();
                var recordsFilteredPage = recordsFiltered.Skip(viewModel.start).Take(viewModel.length).Select(x => new AMRent.Data.Models.View.BlogArticleCategoryIndex()
                {
                    Id = x.Id,
                    Name = x.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Name,
                    IsActive = x.IsActive
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

        // GET: BlogArticleCategories
        public async Task<IActionResult> Index()
        {
            return View();
        }

        // GET: BlogArticleCategories/Create
        public IActionResult Create()
        {
            ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            return View();
        }

        // POST: BlogArticleCategories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BlogArticleCategory blogArticleCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(blogArticleCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            return View(blogArticleCategory);
        }

        // GET: BlogArticleCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.BlogArticleCategories == null)
            {
                return NotFound();
            }

            var blogArticleCategory = await _context.BlogArticleCategories
                .Include(x => x.Translations)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (blogArticleCategory == null)
            {
                return NotFound();
            }
            ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            return View(blogArticleCategory);
        }

        // POST: BlogArticleCategories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BlogArticleCategory blogArticleCategory)
        {
            if (id != blogArticleCategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(blogArticleCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogArticleCategoryExists(blogArticleCategory.Id))
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
            return View(blogArticleCategory);
        }

        // GET: BlogArticleCategories/Deactivate/5
        public async Task<IActionResult> Deactivate(int? id)
        {
            if (id == null || _context.BlogArticleCategories == null)
            {
                return NotFound();
            }

            var blogArticleCategory = await _context.BlogArticleCategories
                .FirstOrDefaultAsync(x => x.Id == id);
            if (blogArticleCategory == null)
            {
                return NotFound();
            }

            blogArticleCategory.IsActive = false;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: BlogArticleCategories/Activate/5
        public async Task<IActionResult> Activate(int? id)
        {
            if (id == null || _context.BlogArticleCategories == null)
            {
                return NotFound();
            }

            var blogArticleCategory = await _context.BlogArticleCategories
                .FirstOrDefaultAsync(x => x.Id == id);
            if (blogArticleCategory == null)
            {
                return NotFound();
            }

            blogArticleCategory.IsActive = true;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool BlogArticleCategoryExists(int id)
        {
            return (_context.BlogArticleCategories?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
