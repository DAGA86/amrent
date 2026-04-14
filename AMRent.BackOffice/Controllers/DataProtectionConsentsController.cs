using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AMRent.Data.Contexts;
using AMRent.Data.Models.Database;

namespace AMRent.BackOffice.Controllers
{
    [Extensions.AuthorizePermission(Data.Enums.Permissions.Other)]
    public class DataProtectionConsentsController : BaseController
    {
        public DataProtectionConsentsController(FullDatabaseContext context, ILogger<DataProtectionConsentsController> logger) : base(context, logger)
        {
        }

        [HttpPost]
        public JsonResult Get(Models.DataTables.ViewModel viewModel)
        {
            JsonResult result = new JsonResult("");
            try
            {
                IQueryable<DataProtectionConsent> recordsTotal = _context.DataProtectionConsents
                    .Include(x => x.Translations.Where(y => y.LanguageId == (int)Data.Enums.Languages.Portuguese));
                var recordsTotalCount = recordsTotal.Count();
                var recordsFiltered = recordsTotal;

                if (viewModel.order.Any())
                {
                    switch (viewModel.order.First().column)
                    {
                        case 0:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.SortNumber)
                                : recordsFiltered.OrderByDescending(x => x.SortNumber);
                            break;
                        case 1:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.Translations.FirstOrDefault(y => y.LanguageId == (int)Data.Enums.Languages.Portuguese).Text)
                                : recordsFiltered.OrderByDescending(x => x.Translations.FirstOrDefault(y => y.LanguageId == (int)Data.Enums.Languages.Portuguese).Text);
                            break;
                    }
                }
                if (!string.IsNullOrEmpty(viewModel.search.value))
                {
                    var predicate = PredicateBuilder.New<DataProtectionConsent>();
                    predicate = predicate.Or(x => x.Translations.Any(x => x.Text.Contains(viewModel.search.value)));

                    recordsFiltered = recordsFiltered.Where(predicate);
                }
                var recordsFilteredCount = recordsFiltered.Count();
                var recordsFilteredPage = recordsFiltered.Skip(viewModel.start).Take(viewModel.length).Select(x => new AMRent.Data.Models.View.DataProtectionConsentIndex()
                {
                    Id = x.Id,
                    Text = x.Translations.FirstOrDefault(y => y.LanguageId == (int)Data.Enums.Languages.Portuguese).Text.Length > 100 ? 
                        $"{x.Translations.FirstOrDefault(y => y.LanguageId == (int)Data.Enums.Languages.Portuguese).Text.Substring(0, 100)}..."
                        : x.Translations.FirstOrDefault(y => y.LanguageId == (int)Data.Enums.Languages.Portuguese).Text,
                    IsActive = x.IsActive,
                    SortNumber = x.SortNumber
                }).ToArray();

                int highestSortNumberId = recordsTotal.OrderByDescending(x => x.SortNumber).FirstOrDefault()?.Id ?? 0;
                int lowestSortNumberId = recordsTotal.OrderBy(x => x.SortNumber).FirstOrDefault()?.Id ?? 0;

                if (recordsFilteredPage.Any(x => x.Id == highestSortNumberId))
                {
                    recordsFilteredPage.FirstOrDefault(x => x.Id == highestSortNumberId).IsHighestSortNumber = true;
                }
                if (recordsFilteredPage.Any(x => x.Id == lowestSortNumberId))
                {
                    recordsFilteredPage.FirstOrDefault(x => x.Id == lowestSortNumberId).IsLowestSortNumber = true;
                }

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

        // GET: DataProtectionConsents
        public async Task<IActionResult> Index()
        {
            return View();
        }

        // GET: DataProtectionConsents/Create
        public IActionResult Create()
        {
            var model = new DataProtectionConsent();
            ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            return View(model);
        }

        // POST: DataProtectionConsents/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DataProtectionConsent dataProtectionConsent)
        {
            if (ModelState.IsValid)
            {
                dataProtectionConsent.SortNumber = (short)(await _context.DataProtectionConsents.MaxAsync(x => x.SortNumber) + 1);
                _context.Add(dataProtectionConsent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            return View(dataProtectionConsent);
        }

        // GET: DataProtectionConsents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.DataProtectionConsents == null)
            {
                return NotFound();
            }

            var dataProtectionConsent = await _context.DataProtectionConsents
                .Include(x => x.Translations)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (dataProtectionConsent == null)
            {
                return NotFound();
            }
            ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            return View(dataProtectionConsent);
        }

        // POST: DataProtectionConsents/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DataProtectionConsent dataProtectionConsent)
        {
            if (id != dataProtectionConsent.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dataProtectionConsent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DataProtectionConsentExists(dataProtectionConsent.Id))
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
            return View(dataProtectionConsent);
        }

        // GET: DataProtectionConsents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.DataProtectionConsents == null)
            {
                return NotFound();
            }

            var dataProtectionConsent = await _context.DataProtectionConsents
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dataProtectionConsent == null)
            {
                return NotFound();
            }

            return View(dataProtectionConsent);
        }

        // POST: DataProtectionConsents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.DataProtectionConsents == null)
            {
                return Problem("Entity set 'FullDatabaseContext.DataProtectionConsents'  is null.");
            }
            var dataProtectionConsent = await _context.DataProtectionConsents.FindAsync(id);
            if (dataProtectionConsent != null)
            {
                _context.DataProtectionConsents.Remove(dataProtectionConsent);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: DataProtectionConsents/Deactivate/5
        public async Task<IActionResult> Deactivate(int? id)
        {
            if (id == null || _context.DataProtectionConsents == null)
            {
                return NotFound();
            }

            var dataProtectionConsent = await _context.DataProtectionConsents
                .FirstOrDefaultAsync(x => x.Id == id);
            if (dataProtectionConsent == null)
            {
                return NotFound();
            }

            dataProtectionConsent.IsActive = false;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: DataProtectionConsents/Activate/5
        public async Task<IActionResult> Activate(int? id)
        {
            if (id == null || _context.DataProtectionConsents == null)
            {
                return NotFound();
            }

            var dataProtectionConsent = await _context.DataProtectionConsents
                .FirstOrDefaultAsync(x => x.Id == id);
            if (dataProtectionConsent == null)
            {
                return NotFound();
            }

            dataProtectionConsent.IsActive = true;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: DataProtectionConsents/SortUp/5
        public async Task<IActionResult> SortUp(int? id)
        {
            if (id == null || _context.DataProtectionConsents == null)
            {
                return NotFound();
            }

            var dataProtectionConsent = await _context.DataProtectionConsents
                .FirstOrDefaultAsync(x => x.Id == id);
            if (dataProtectionConsent == null)
            {
                return NotFound();
            }

            if (dataProtectionConsent.SortNumber > 1)
            {
                var lowerSortNumberTeamMember = await _context.DataProtectionConsents.FirstOrDefaultAsync(x => x.SortNumber == dataProtectionConsent.SortNumber - 1);
                if (lowerSortNumberTeamMember != null)
                {
                    lowerSortNumberTeamMember.SortNumber++;
                }

                dataProtectionConsent.SortNumber--;

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: DataProtectionConsents/SortDown/5
        public async Task<IActionResult> SortDown(int? id)
        {
            if (id == null || _context.DataProtectionConsents == null)
            {
                return NotFound();
            }

            var dataProtectionConsent = await _context.DataProtectionConsents
                .FirstOrDefaultAsync(x => x.Id == id);
            if (dataProtectionConsent == null)
            {
                return NotFound();
            }

            var higherSortNumberTeamMember = await _context.DataProtectionConsents.FirstOrDefaultAsync(x => x.SortNumber == dataProtectionConsent.SortNumber + 1);
            if (higherSortNumberTeamMember != null)
            {
                higherSortNumberTeamMember.SortNumber--;
            }

            dataProtectionConsent.SortNumber++;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool DataProtectionConsentExists(int id)
        {
            return (_context.DataProtectionConsents?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
