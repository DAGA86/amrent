using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AMRent.Data.Contexts;
using AMRent.Data.Models.Database;

namespace AMRent.BackOffice.Controllers
{
    [Extensions.AuthorizePermission(Data.Enums.Permissions.Other)]
    public class OfficeLocationsController : BaseController
    {
        public OfficeLocationsController(FullDatabaseContext context, ILogger<OfficeLocationsController> logger) : base(context, logger)
        {
        }

        [HttpPost]
        public JsonResult Get(Models.DataTables.ViewModel viewModel)
        {
            JsonResult result = new JsonResult("");
            try
            {
                IQueryable<OfficeLocation> recordsTotal = _context.OfficeLocations;
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
                    var predicate = PredicateBuilder.New<OfficeLocation>();
                    predicate = predicate.Or(x => x.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Name.Contains(viewModel.search.value));

                    recordsFiltered = recordsFiltered.Where(predicate);
                }
                var recordsFilteredCount = recordsFiltered.Count();
                var recordsFilteredPage = recordsFiltered.Skip(viewModel.start).Take(viewModel.length).Select(x => new AMRent.Data.Models.View.OfficeLocationIndex()
                {
                    Id = x.Id,
                    Name = x.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Name
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

        // GET: OfficeLocations
        public async Task<IActionResult> Index()
        {
            return View();
        }

        // GET: OfficeLocations/Create
        public IActionResult Create()
        {
            ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            return View();
        }

        // POST: OfficeLocations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OfficeLocation officeLocation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(officeLocation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            return View(officeLocation);
        }

        // GET: OfficeLocations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.OfficeLocations == null)
            {
                return NotFound();
            }

            var officeLocation = await _context.OfficeLocations
                .Include(x => x.Translations)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (officeLocation == null)
            {
                return NotFound();
            }
            ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            return View(officeLocation);
        }

        // POST: OfficeLocations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OfficeLocation officeLocation)
        {
            if (id != officeLocation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(officeLocation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OfficeLocationExists(officeLocation.Id))
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
            return View(officeLocation);
        }

        // GET: OfficeLocations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.OfficeLocations == null)
            {
                return NotFound();
            }

            var officeLocation = await _context.OfficeLocations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (officeLocation == null)
            {
                return NotFound();
            }

            return View(officeLocation);
        }

        // POST: OfficeLocations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.OfficeLocations == null)
            {
                return Problem("Entity set 'FullDatabaseContext.OfficeLocations'  is null.");
            }
            var officeLocation = await _context.OfficeLocations.FindAsync(id);
            if (officeLocation != null)
            {
                _context.OfficeLocations.Remove(officeLocation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OfficeLocationExists(int id)
        {
            return (_context.OfficeLocations?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
