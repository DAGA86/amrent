using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AMRent.Data.Contexts;
using AMRent.Data.Models.Database;
using Newtonsoft.Json.Linq;
using AMRent.Data.Migrations;

namespace AMRent.BackOffice.Controllers
{
    [Extensions.AuthorizePermission(Data.Enums.Permissions.Other)]
    public class ExtrasController : BaseController
    {
        public ExtrasController(FullDatabaseContext context, ILogger<ExtrasController> logger) : base(context, logger)
        {
        }

        [HttpPost]
        public JsonResult Get(Models.DataTables.ViewModel viewModel)
        {
            JsonResult result = new JsonResult("");
            try
            {
                IQueryable<Extra> recordsTotal = _context.Extras
                    .Include(x => x.Translations)
                    .Include(x => x.ExtraPricesByInsuranceLevel);
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
                                recordsFiltered.OrderBy(x => x.ExtraPricesByInsuranceLevel.FirstOrDefault(x => x.InsuranceLevelId == 1).Value)
                                : recordsFiltered.OrderByDescending(x => x.ExtraPricesByInsuranceLevel.FirstOrDefault(x => x.InsuranceLevelId == 1).Value);
                            break;
                        case 2:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.ExtraPricesByInsuranceLevel.FirstOrDefault(x => x.InsuranceLevelId == 2).Value)
                                : recordsFiltered.OrderByDescending(x => x.ExtraPricesByInsuranceLevel.FirstOrDefault(x => x.InsuranceLevelId == 1).Value);
                            break;
                        case 3:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.ExtraPricesByInsuranceLevel.FirstOrDefault(x => x.InsuranceLevelId == 3).Value)
                                : recordsFiltered.OrderByDescending(x => x.ExtraPricesByInsuranceLevel.FirstOrDefault(x => x.InsuranceLevelId == 1).Value);
                            break;
                    }
                }
                if (!string.IsNullOrEmpty(viewModel.search.value))
                {
                    var predicate = PredicateBuilder.New<Extra>();
                    predicate = predicate.Or(x => x.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Name.Contains(viewModel.search.value));

                    recordsFiltered = recordsFiltered.Where(predicate);
                }
                var recordsFilteredCount = recordsFiltered.Count();
                var recordsFilteredPage = recordsFiltered.Skip(viewModel.start).Take(viewModel.length).Select(x => new AMRent.Data.Models.View.ExtraIndex()
                {
                    Id = x.Id,
                    Name = x.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Name,
                    ValueInsuranceLevel1 = x.ExtraPricesByInsuranceLevel.FirstOrDefault(x => x.InsuranceLevelId == 1).Value,
                    ValueInsuranceLevel2 = x.ExtraPricesByInsuranceLevel.FirstOrDefault(x => x.InsuranceLevelId == 2).Value,
                    ValueInsuranceLevel3 = x.ExtraPricesByInsuranceLevel.FirstOrDefault(x => x.InsuranceLevelId == 3).Value,
                    IsActive = x.IsActive,
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

        // GET: Extras
        public async Task<IActionResult> Index()
        {
            ViewBag.InsuranceLevels = _context.InsuranceLevels
                .Include(x => x.Translations.Where(y => y.LanguageId == (int)Data.Enums.Languages.Portuguese))
                .OrderBy(x => x.Id)
                .Select(x => new
                {
                    Id = x.Id,
                    Name = x.Translations.FirstOrDefault().Name
                })
                .ToList();
            return View();
        }

        // GET: Extras/Create
        public IActionResult Create()
        {
            ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            ViewBag.InsuranceLevels = _context.InsuranceLevels
                .Include(x => x.Translations.Where(y => y.LanguageId == (int)Data.Enums.Languages.Portuguese))
                .OrderBy(x => x.Id)
                .Select(x => new
                {
                    Id = x.Id,
                    Name = x.Translations.FirstOrDefault().Name
                })
                .ToList();
            ViewBag.ExtraTypes = new SelectList(dCore.Helpers.Enum.GetWithDescription<Data.Enums.ExtraTypes>().Select(x => new SelectListItem
            {
                Text = x.Value,
                Value = x.Key
            }), "Value", "Text", Data.Enums.ExtraTypes.Other);
            return View();
        }

        // POST: Extras/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Extra extra)
        {
            if (ModelState.IsValid)
            {
                _context.Add(extra);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.ExtraTypes = new SelectList(dCore.Helpers.Enum.GetWithDescription<Data.Enums.ExtraTypes>().Select(x => new SelectListItem
            {
                Text = x.Value,
                Value = x.Key
            }), "Value", "Text", extra.ExtraType);
            ViewBag.InsuranceLevels = _context.InsuranceLevels
                .Include(x => x.Translations.Where(y => y.LanguageId == (int)Data.Enums.Languages.Portuguese))
                .OrderBy(x => x.Id)
                .Select(x => new
                {
                    Id = x.Id,
                    Name = x.Translations.FirstOrDefault().Name
                })
                .ToList();
            ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            return View(extra);
        }

        // GET: Extras/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Extras == null)
            {
                return NotFound();
            }

            var extra = await _context.Extras
                .Include(x => x.Translations)
                .Include(x => x.ExtraPricesByInsuranceLevel)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (extra == null)
            {
                return NotFound();
            }
            ViewBag.ExtraTypes = new SelectList(dCore.Helpers.Enum.GetWithDescription<Data.Enums.ExtraTypes>().Select(x => new SelectListItem
            {
                Text = x.Value,
                Value = x.Key
            }), "Value", "Text", extra.ExtraType);
            ViewBag.InsuranceLevels = _context.InsuranceLevels
                .Include(x => x.Translations.Where(y => y.LanguageId == (int)Data.Enums.Languages.Portuguese))
                .OrderBy(x => x.Id)
                .Select(x => new
                {
                    Id = x.Id,
                    Name = x.Translations.FirstOrDefault().Name
                })
                .ToList();
            ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            return View(extra);
        }

        // POST: Extras/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Extra extra)
        {
            if (id != extra.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(extra);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExtraExists(extra.Id))
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
            ViewBag.ExtraTypes = new SelectList(dCore.Helpers.Enum.GetWithDescription<Data.Enums.ExtraTypes>().Select(x => new SelectListItem
            {
                Text = x.Value,
                Value = x.Key
            }), "Value", "Text", extra.ExtraType);
            ViewBag.InsuranceLevels = _context.InsuranceLevels
                .Include(x => x.Translations.Where(y => y.LanguageId == (int)Data.Enums.Languages.Portuguese))
                .OrderBy(x => x.Id)
                .Select(x => new
                {
                    Id = x.Id,
                    Name = x.Translations.FirstOrDefault().Name
                })
                .ToList();
            ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            return View(extra);
        }

        // GET: Extras/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Extras == null)
            {
                return NotFound();
            }

            var extra = await _context.Extras
                .FirstOrDefaultAsync(m => m.Id == id);
            if (extra == null)
            {
                return NotFound();
            }

            return View(extra);
        }

        // POST: Extras/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Extras == null)
            {
                return Problem("Entity set 'FullDatabaseContext.Extras'  is null.");
            }
            var extra = await _context.Extras.FindAsync(id);
            if (extra != null)
            {
                _context.Extras.Remove(extra);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Extras/Deactivate/5
        public async Task<IActionResult> Deactivate(int? id)
        {
            if (id == null || _context.Extras == null)
            {
                return NotFound();
            }

            var extra = await _context.Extras
                .FirstOrDefaultAsync(x => x.Id == id);
            if (extra == null)
            {
                return NotFound();
            }

            extra.IsActive = false;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Extras/Activate/5
        public async Task<IActionResult> Activate(int? id)
        {
            if (id == null || _context.Extras == null)
            {
                return NotFound();
            }

            var extra = await _context.Extras
                .FirstOrDefaultAsync(x => x.Id == id);
            if (extra == null)
            {
                return NotFound();
            }

            extra.IsActive = true;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool ExtraExists(int id)
        {
            return (_context.Extras?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
