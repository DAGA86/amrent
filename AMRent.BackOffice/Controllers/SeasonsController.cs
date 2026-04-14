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
    public class SeasonsController : BaseController
    {
        public SeasonsController(FullDatabaseContext context, ILogger<SeasonsController> logger) : base(context, logger)
        {
        }

        [HttpPost]
        public JsonResult Get(Models.DataTables.ViewModel viewModel)
        {
            JsonResult result = new JsonResult("");
            try
            {
                IQueryable<Season> recordsTotal = _context.Seasons;
                var recordsTotalCount = recordsTotal.Count();
                var recordsFiltered = recordsTotal;

                if (viewModel.order.Any())
                {
                    switch (viewModel.order.First().column)
                    {
                        case 0:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.SeasonCategory.Name)
                                : recordsFiltered.OrderByDescending(x => x.SeasonCategory.Name);
                            break;
                        case 1:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.StartDateUtc)
                                : recordsFiltered.OrderByDescending(x => x.StartDateUtc);
                            break;
                        case 2:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.EndDateUtc)
                                : recordsFiltered.OrderByDescending(x => x.EndDateUtc);
                            break;
                    }
                }
                if (!string.IsNullOrEmpty(viewModel.search.value))
                {
                    var predicate = PredicateBuilder.New<Season>();
                    predicate = predicate.Or(x => x.SeasonCategory.Name.Contains(viewModel.search.value));

                    recordsFiltered = recordsFiltered.Where(predicate);
                }
                var recordsFilteredCount = recordsFiltered.Count();
                var recordsFilteredPage = recordsFiltered.Skip(viewModel.start).Take(viewModel.length).Select(x => new AMRent.Data.Models.View.SeasonIndex()
                {
                    Id = x.Id,
                    SeasonCategory = x.SeasonCategory.Name,
                    StartDateUtc = x.StartDateUtc.ToString("yyyy-MM-dd"),
                    EndDateUtc = x.EndDateUtc.ToString("yyyy-MM-dd")
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

        // GET: Seasons
        public async Task<IActionResult> Index()
        {
            return View();
        }

        // GET: Seasons/Create
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.SeasonCategories.Select(x => new { x.Id, x.Name }), "Id", "Name");
            return View();
        }

        // POST: Seasons/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Season season)
        {
            if (ModelState.IsValid)
            {
                _context.Add(season);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = new SelectList(_context.SeasonCategories.Select(x => new { x.Id, x.Name }), "Id", "Name", season.SeasonCategoryId);
            return View(season);
        }

        // GET: Seasons/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Seasons == null)
            {
                return NotFound();
            }

            var season = await _context.Seasons
                .Include(x => x.Prices)
                .ThenInclude(y => y.CarSegment)
                .Include(x => x.Changes)
                .ThenInclude(y => y.User)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (season == null)
            {
                return NotFound();
            }
            ViewBag.Categories = new SelectList(_context.SeasonCategories.Select(x => new { x.Id, x.Name }), "Id", "Name", season.SeasonCategoryId);
            return View(season);
        }

        // POST: Seasons/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Season season)
        {
            if (id != season.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.UpdateWithTracking(season, Guid.Parse(User.Claims.First(x => x.Type == ClaimTypes.UserData).Value));
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SeasonExists(season.Id))
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
            ViewBag.Categories = new SelectList(_context.SeasonCategories.Select(x => new { x.Id, x.Name }), "Id", "Name", season.SeasonCategoryId);
            return View(season);
        }

        // GET: Seasons/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Seasons == null)
            {
                return NotFound();
            }

            var season = await _context.Seasons
                .FirstOrDefaultAsync(m => m.Id == id);
            if (season == null)
            {
                return NotFound();
            }

            return View(season);
        }

        // POST: Seasons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Seasons == null)
            {
                return Problem("Entity set 'FullDatabaseContext.Seasons'  is null.");
            }
            var season = await _context.Seasons.FindAsync(id);
            if (season != null)
            {
                _context.Seasons.Remove(season);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SeasonExists(int id)
        {
            return (_context.Seasons?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
