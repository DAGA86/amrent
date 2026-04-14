using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AMRent.Data.Contexts;
using AMRent.Data.Models.Database;

namespace AMRent.BackOffice.Controllers
{
    [Extensions.AuthorizePermission(Data.Enums.Permissions.Other)]
    public class PickupReturnLocationsController : BaseController
    {
        public PickupReturnLocationsController(FullDatabaseContext context, ILogger<PickupReturnLocationsController> logger) : base(context, logger)
        {
        }

        [HttpPost]
        public JsonResult Get(Models.DataTables.ViewModel viewModel)
        {
            JsonResult result = new JsonResult("");
            try
            {
                IQueryable<PickupReturnLocation> recordsTotal = _context.PickupReturnLocations;
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
                    var predicate = PredicateBuilder.New<PickupReturnLocation>();
                    predicate = predicate.Or(x => x.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Name.Contains(viewModel.search.value));

                    recordsFiltered = recordsFiltered.Where(predicate);
                }
                var recordsFilteredCount = recordsFiltered.Count();
                var recordsFilteredPage = recordsFiltered.Skip(viewModel.start).Take(viewModel.length).Select(x => new AMRent.Data.Models.View.PickupReturnLocationIndex()
                {
                    Id = x.Id,
                    Name = x.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Name,
                    IsSelectedByDefault = x.IsSelectedByDefault,
                    IsWorkingOffice = x.IsWorkingOffice
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

        private PickupReturnLocation PopulateDayOfWeekSchedules(PickupReturnLocation pickupReturnLocation)
        {
            if (!pickupReturnLocation.DayOfWeekSchedules.Any())
            {
                for (int i = 1; i < 8; i++)
                {
                    Data.Enums.DaysOfWeek dayOfWeek = (Data.Enums.DaysOfWeek)i;
                    pickupReturnLocation.DayOfWeekSchedules.Add(new PickupReturnLocationDayOfWeekSchedule()
                    {
                        DayOfWeek = dayOfWeek
                    });
                }
            }

            return pickupReturnLocation;
        }

        // GET: PickupReturnLocations
        public async Task<IActionResult> Index()
        {
            return View();
        }

        // GET: PickupReturnLocations/Create
        public IActionResult Create()
        {
            ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            PickupReturnLocation model = PopulateDayOfWeekSchedules(new PickupReturnLocation());
            return View(model);
        }

        // POST: PickupReturnLocations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PickupReturnLocation pickupReturnLocation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pickupReturnLocation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PopulateDayOfWeekSchedules(pickupReturnLocation);
            ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            pickupReturnLocation = PopulateDayOfWeekSchedules(pickupReturnLocation);
            return View(pickupReturnLocation);
        }

        // GET: PickupReturnLocations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.PickupReturnLocations == null)
            {
                return NotFound();
            }

            var pickupReturnLocation = await _context.PickupReturnLocations
                .Include(x => x.Translations)
                .Include(x => x.DayOfWeekSchedules)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (pickupReturnLocation == null)
            {
                return NotFound();
            }
            ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            pickupReturnLocation = PopulateDayOfWeekSchedules(pickupReturnLocation);
            return View(pickupReturnLocation);
        }

        // POST: PickupReturnLocations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PickupReturnLocation pickupReturnLocation)
        {
            if (id != pickupReturnLocation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pickupReturnLocation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PickupReturnLocationExists(pickupReturnLocation.Id))
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
            pickupReturnLocation = PopulateDayOfWeekSchedules(pickupReturnLocation);
            return View(pickupReturnLocation);
        }

        // GET: PickupReturnLocations/SetSelectedByDefault/5
        public async Task<IActionResult> SetSelectedByDefault(int? id)
        {
            if (id == null || _context.PickupReturnLocations == null)
            {
                return NotFound();
            }

            var pickupReturnLocation = await _context.PickupReturnLocations
                .FirstOrDefaultAsync(x => x.Id == id);
            if (pickupReturnLocation == null)
            {
                return NotFound();
            }

            _context.PickupReturnLocations.Where(x => x.Id != id).ForEach(x => x.IsSelectedByDefault = false);
            pickupReturnLocation.IsSelectedByDefault = true;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: PickupReturnLocations/SetWorkingOffice/5
        public async Task<IActionResult> SetWorkingOffice(int? id)
        {
            if (id == null || _context.PickupReturnLocations == null)
            {
                return NotFound();
            }

            var pickupReturnLocation = await _context.PickupReturnLocations
                .FirstOrDefaultAsync(x => x.Id == id);
            if (pickupReturnLocation == null)
            {
                return NotFound();
            }

            _context.PickupReturnLocations.Where(x => x.Id != id).ForEach(x => x.IsWorkingOffice = false);
            pickupReturnLocation.IsWorkingOffice = true;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: PickupReturnLocations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.PickupReturnLocations == null)
            {
                return NotFound();
            }

            var pickupReturnLocation = await _context.PickupReturnLocations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pickupReturnLocation == null)
            {
                return NotFound();
            }

            return View(pickupReturnLocation);
        }

        // POST: PickupReturnLocations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.PickupReturnLocations == null)
            {
                return Problem("Entity set 'FullDatabaseContext.PickupReturnLocations'  is null.");
            }
            var pickupReturnLocation = await _context.PickupReturnLocations.FindAsync(id);
            if (pickupReturnLocation != null)
            {
                _context.PickupReturnLocations.Remove(pickupReturnLocation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PickupReturnLocationExists(int id)
        {
            return (_context.PickupReturnLocations?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
