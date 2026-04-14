using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AMRent.Data.Contexts;
using AMRent.Data.Models.Database;

namespace AMRent.BackOffice.Controllers
{
    [Extensions.AuthorizePermission(Data.Enums.Permissions.Other)]
    public class PickupReturnTemporaryTaxes : BaseController
    {
        public PickupReturnTemporaryTaxes(FullDatabaseContext context, ILogger<PickupReturnTemporaryTaxes> logger) : base(context, logger)
        {
        }

        [HttpPost]
        public JsonResult Get(Models.DataTables.ViewModel viewModel)
        {
            JsonResult result = new JsonResult("");
            try
            {
                IQueryable<PickupReturnTemporaryTax> recordsTotal = _context.PickupReturnTemporaryTaxes
                    .Include(x => x.Translations);
                var recordsTotalCount = recordsTotal.Count();
                var recordsFiltered = recordsTotal;

                if (viewModel.order.Any())
                {
                    switch (viewModel.order.First().column)
                    {
                        case 0:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.StartDate)
                                : recordsFiltered.OrderByDescending(x => x.StartDate);
                            break;
                        case 1:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.EndDate)
                                : recordsFiltered.OrderByDescending(x => x.EndDate);
                            break;
                        case 2:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.StartTime)
                                : recordsFiltered.OrderByDescending(x => x.StartTime);
                            break;
                        case 3:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.EndTime)
                                : recordsFiltered.OrderByDescending(x => x.EndTime);
                            break;
                        case 4:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.Value)
                                : recordsFiltered.OrderByDescending(x => x.Value);
                            break;
                        case 5:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Name)
                                : recordsFiltered.OrderByDescending(x => x.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Name);
                            break;
                    }
                }
                if (!string.IsNullOrEmpty(viewModel.search.value))
                {
                    var predicate = PredicateBuilder.New<PickupReturnTemporaryTax>();
                    predicate = predicate.Or(x => x.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Name.Contains(viewModel.search.value));

                    recordsFiltered = recordsFiltered.Where(predicate);
                }
                var recordsFilteredCount = recordsFiltered.Count();
                var recordsFilteredPage = recordsFiltered.Skip(viewModel.start).Take(viewModel.length).Select(x => new AMRent.Data.Models.View.PickupReturnTemporaryTaxIndex()
                {
                    Id = x.Id,
                    Name = x.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Name,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    Value = x.Value,
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

        // GET: PickupReturnTemporaryTaxes
        public async Task<IActionResult> Index()
        {
            return View();
        }

        // GET: PickupReturnTemporaryTaxes/Create
        public IActionResult Create()
        {
            ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            return View();
        }

        // POST: PickupReturnTemporaryTaxes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PickupReturnTemporaryTax pickupReturnTemporaryTax)
        {
            if (!pickupReturnTemporaryTax.StartDate.HasValue
                && !pickupReturnTemporaryTax.EndDate.HasValue
                && !pickupReturnTemporaryTax.StartTime.HasValue
                && !pickupReturnTemporaryTax.EndTime.HasValue)
            {
                ModelState.AddModelError("", "Tem de preencher as datas e/ou as horas.");
            }
            else
            {
                if (
                    (!pickupReturnTemporaryTax.StartDate.HasValue && pickupReturnTemporaryTax.EndDate.HasValue)
                    || (pickupReturnTemporaryTax.StartDate.HasValue && !pickupReturnTemporaryTax.EndDate.HasValue)
                    )
                {
                    ModelState.AddModelError("", "Se preencheu uma data, tem de preencher as duas.");
                }
                if (
                    (!pickupReturnTemporaryTax.StartTime.HasValue && pickupReturnTemporaryTax.EndTime.HasValue)
                    || (pickupReturnTemporaryTax.StartTime.HasValue && !pickupReturnTemporaryTax.EndTime.HasValue)
                    )
                {
                    ModelState.AddModelError("", "Se preencheu uma hora, tem de preencher as duas.");
                }
            }
            if (ModelState.IsValid)
            {
                _context.Add(pickupReturnTemporaryTax);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            return View(pickupReturnTemporaryTax);
        }

        // GET: PickupReturnTemporaryTaxes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.PickupReturnTemporaryTaxes == null)
            {
                return NotFound();
            }

            var pickupReturnTemporaryTax = await _context.PickupReturnTemporaryTaxes
                .Include(x => x.Translations)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (pickupReturnTemporaryTax == null)
            {
                return NotFound();
            }
            ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            return View(pickupReturnTemporaryTax);
        }

        // POST: PickupReturnTemporaryTaxes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PickupReturnTemporaryTax pickupReturnTemporaryTax)
        {
            if (id != pickupReturnTemporaryTax.Id)
            {
                return NotFound();
            }

            if (!pickupReturnTemporaryTax.StartDate.HasValue
                && !pickupReturnTemporaryTax.EndDate.HasValue
                && !pickupReturnTemporaryTax.StartTime.HasValue
                && !pickupReturnTemporaryTax.EndTime.HasValue)
            {
                ModelState.AddModelError("", "Tem de preencher as datas e/ou as horas.");
            }
            else
            {
                if (
                    (!pickupReturnTemporaryTax.StartDate.HasValue && pickupReturnTemporaryTax.EndDate.HasValue)
                    || (pickupReturnTemporaryTax.StartDate.HasValue && !pickupReturnTemporaryTax.EndDate.HasValue)
                    )
                {
                    ModelState.AddModelError("", "Se preencheu uma data, tem de preencher as duas.");
                }
                if (
                    (!pickupReturnTemporaryTax.StartTime.HasValue && pickupReturnTemporaryTax.EndTime.HasValue)
                    || (pickupReturnTemporaryTax.StartTime.HasValue && !pickupReturnTemporaryTax.EndTime.HasValue)
                    )
                {
                    ModelState.AddModelError("", "Se preencheu uma hora, tem de preencher as duas.");
                }
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pickupReturnTemporaryTax);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PickupReturnTemporaryTaxExists(pickupReturnTemporaryTax.Id))
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
            return View(pickupReturnTemporaryTax);
        }

        // GET: PickupReturnTemporaryTaxes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.PickupReturnTemporaryTaxes == null)
            {
                return NotFound();
            }

            var pickupReturnTemporaryTax = await _context.PickupReturnTemporaryTaxes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pickupReturnTemporaryTax == null)
            {
                return NotFound();
            }

            return View(pickupReturnTemporaryTax);
        }

        // POST: PickupReturnTemporaryTaxes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.PickupReturnTemporaryTaxes == null)
            {
                return Problem("Entity set 'FullDatabaseContext.PickupReturnTemporaryTaxes'  is null.");
            }
            var pickupReturnTemporaryTax = await _context.PickupReturnTemporaryTaxes.FindAsync(id);
            if (pickupReturnTemporaryTax != null)
            {
                _context.PickupReturnTemporaryTaxes.Remove(pickupReturnTemporaryTax);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PickupReturnTemporaryTaxExists(int id)
        {
            return (_context.PickupReturnTemporaryTaxes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
