using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AMRent.Data.Contexts;
using AMRent.Data.Models.Database;

namespace AMRent.BackOffice.Controllers
{
    [Extensions.AuthorizePermission(Data.Enums.Permissions.Other)]
    public class SeasonCategoriesController : BaseController
    {
        public SeasonCategoriesController(FullDatabaseContext context, ILogger<SeasonCategoriesController> logger) : base(context, logger)
        {
        }

        [HttpPost]
        public JsonResult Get(Models.DataTables.ViewModel viewModel)
        {
            JsonResult result = new JsonResult("");
            try
            {
                IQueryable<SeasonCategory> recordsTotal = _context.SeasonCategories;
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
                    var predicate = PredicateBuilder.New<SeasonCategory>();
                    predicate = predicate.Or(x => x.Name.Contains(viewModel.search.value));

                    recordsFiltered = recordsFiltered.Where(predicate);
                }
                var recordsFilteredCount = recordsFiltered.Count();
                var recordsFilteredPage = recordsFiltered.Skip(viewModel.start).Take(viewModel.length).Select(x => new AMRent.Data.Models.View.SeasonCategoryIndex()
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

        // GET: SeasonCategories
        public async Task<IActionResult> Index()
        {
            return View();
        }

        // GET: SeasonCategories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SeasonCategories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SeasonCategory seasonCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(seasonCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(seasonCategory);
        }

        // GET: SeasonCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.SeasonCategories == null)
            {
                return NotFound();
            }

            var seasonCategory = await _context.SeasonCategories
                .FirstOrDefaultAsync(x => x.Id == id);
            if (seasonCategory == null)
            {
                return NotFound();
            }
            return View(seasonCategory);
        }

        // POST: SeasonCategories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SeasonCategory seasonCategory)
        {
            if (id != seasonCategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(seasonCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SeasonCategoryExists(seasonCategory.Id))
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
            return View(seasonCategory);
        }

        // GET: SeasonCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.SeasonCategories == null)
            {
                return NotFound();
            }

            var seasonCategory = await _context.SeasonCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (seasonCategory == null)
            {
                return NotFound();
            }

            return View(seasonCategory);
        }

        // POST: SeasonCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.SeasonCategories == null)
            {
                return Problem("Entity set 'FullDatabaseContext.SeasonCategories'  is null.");
            }
            var seasonCategory = await _context.SeasonCategories.FindAsync(id);
            if (seasonCategory != null)
            {
                _context.SeasonCategories.Remove(seasonCategory);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SeasonCategoryExists(int id)
        {
            return (_context.SeasonCategories?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
