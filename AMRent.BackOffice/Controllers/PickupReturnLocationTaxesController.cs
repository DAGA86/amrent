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
    public class PickupReturnLocationTaxesController : BaseController
    {
        public PickupReturnLocationTaxesController(FullDatabaseContext context, ILogger<PickupReturnLocationTaxesController> logger) : base(context, logger)
        {
        }

        [HttpPost]
        public JsonResult Get(Models.DataTables.ViewModel viewModel)
        {
            JsonResult result = new JsonResult("");
            try
            {
                IQueryable<PickupReturnLocationTax> recordsTotal = _context.PickupReturnLocationTaxes;
                var recordsTotalCount = recordsTotal.Count();
                var recordsFiltered = recordsTotal;

                if (viewModel.order.Any())
                {
                    switch (viewModel.order.First().column)
                    {
                        case 0:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.PickupReturnLocation.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Name)
                                : recordsFiltered.OrderByDescending(x => x.PickupReturnLocation.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Name);
                            break;
                        case 1:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.Days)
                                : recordsFiltered.OrderByDescending(x => x.Days);
                            break;
                        case 2:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.Value)
                                : recordsFiltered.OrderByDescending(x => x.Value);
                            break;
                    }
                }
                if (!string.IsNullOrEmpty(viewModel.search.value))
                {
                    var predicate = PredicateBuilder.New<PickupReturnLocationTax>();
                    predicate = predicate.Or(x => x.PickupReturnLocation.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Name.Contains(viewModel.search.value));

                    recordsFiltered = recordsFiltered.Where(predicate);
                }
                var recordsFilteredCount = recordsFiltered.Count();
                var recordsFilteredPage = recordsFiltered.Skip(viewModel.start).Take(viewModel.length).Select(x => new AMRent.Data.Models.View.PickupReturnLocationTaxIndex()
                {
                    Id = x.Id,
                    PickupReturnLocationName = x.PickupReturnLocation.Translations.FirstOrDefault(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese).Name,
                    Days = x.Days,
                    Value = x.Value
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

        // GET: PickupReturnLocationTaxes
        public async Task<IActionResult> Index()
        {
            return View();
        }

        // GET: PickupReturnLocationTaxes/Create
        public IActionResult Create()
        {
            ViewBag.Locations = new SelectList(_context.PickupReturnLocations.Select(x => new { x.Id, Text = $"{x.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Name}" }), "Id", "Text");
            return View();
        }

        // POST: PickupReturnLocationTaxes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PickupReturnLocationTax pickupReturnLocationTax)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pickupReturnLocationTax);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Locations = new SelectList(_context.PickupReturnLocations.Select(x => new { x.Id, Text = $"{x.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Name}" }), "Id", "Text", pickupReturnLocationTax.PickupReturnLocationId);
            return View(pickupReturnLocationTax);
        }

        // GET: PickupReturnLocationTaxes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.PickupReturnLocationTaxes == null)
            {
                return NotFound();
            }

            var pickupReturnLocationTax = await _context.PickupReturnLocationTaxes
                .Include(x => x.Changes)
                .ThenInclude(y => y.User)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (pickupReturnLocationTax == null)
            {
                return NotFound();
            }
            ViewBag.Locations = new SelectList(_context.PickupReturnLocations.Select(x => new { x.Id, Text = $"{x.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Name}" }), "Id", "Text", pickupReturnLocationTax.PickupReturnLocationId);
            return View(pickupReturnLocationTax);
        }

        // POST: PickupReturnLocationTaxes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PickupReturnLocationTax pickupReturnLocationTax)
        {
            if (id != pickupReturnLocationTax.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.UpdateWithTracking(pickupReturnLocationTax, Guid.Parse(User.Claims.First(x => x.Type == ClaimTypes.UserData).Value));
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PickupReturnLocationTaxExists(pickupReturnLocationTax.Id))
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
            ViewBag.Locations = new SelectList(_context.PickupReturnLocations.Select(x => new { x.Id, Text = $"{x.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Name}" }), "Id", "Text", pickupReturnLocationTax.PickupReturnLocationId);
            return View(pickupReturnLocationTax);
        }

        // GET: PickupReturnLocationTaxes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.PickupReturnLocationTaxes == null)
            {
                return NotFound();
            }

            var pickupReturnLocationTax = await _context.PickupReturnLocationTaxes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pickupReturnLocationTax == null)
            {
                return NotFound();
            }
            _context.PickupReturnLocationTaxes.Remove(pickupReturnLocationTax);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: PickupReturnLocationTaxes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.PickupReturnLocationTaxes == null)
            {
                return Problem("Entity set 'FullDatabaseContext.PickupReturnLocationTaxes'  is null.");
            }
            var pickupReturnLocationTax = await _context.PickupReturnLocationTaxes.FindAsync(id);
            if (pickupReturnLocationTax != null)
            {
                _context.PickupReturnLocationTaxes.Remove(pickupReturnLocationTax);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PickupReturnLocationTaxExists(int id)
        {
            return (_context.Prices?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
