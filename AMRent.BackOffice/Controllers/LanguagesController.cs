using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AMRent.Data.Contexts;
using LinqKit;

namespace AMRent.BackOffice.Controllers
{
    [Extensions.AuthorizePermission(Data.Enums.Permissions.Other)]
    public class LanguagesController : BaseController
    {
        public LanguagesController(FullDatabaseContext context, ILogger<LanguagesController> logger) : base(context, logger)
        {
        }

        [HttpPost]
        public JsonResult Get(Models.DataTables.ViewModel viewModel)
        {
            JsonResult result = new JsonResult("");
            try
            {
                IQueryable<dCore.MultiLanguage.Models.Language> recordsTotal = _context.Languages;
                var recordsTotalCount = recordsTotal.Count();
                var recordsFiltered = recordsTotal;

                if (viewModel.order.Any())
                {
                    switch (viewModel.order.First().column)
                    {
                        case 0:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ? recordsFiltered.OrderBy(x => x.Name) : recordsFiltered.OrderByDescending(x => x.Name);
                            break;
                        case 1:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ? recordsFiltered.OrderBy(x => x.Code) : recordsFiltered.OrderByDescending(x => x.Code);
                            break;
                        case 2:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ? recordsFiltered.OrderBy(x => x.CountryCode) : recordsFiltered.OrderByDescending(x => x.CountryCode);
                            break;
                    }
                }
                if (!string.IsNullOrEmpty(viewModel.search.value))
                {
                    var predicate = PredicateBuilder.New<dCore.MultiLanguage.Models.Language>();
                    predicate = predicate.Or(x => x.Name.Contains(viewModel.search.value));
                    predicate = predicate.Or(x => x.Code.Contains(viewModel.search.value));
                    predicate = predicate.Or(x => x.CountryCode.Contains(viewModel.search.value));

                    recordsFiltered = recordsFiltered.Where(predicate);
                }
                var recordsFilteredCount = recordsFiltered.Count();
                var recordsFilteredPage = recordsFiltered.Skip(viewModel.start).Take(viewModel.length).Select(x => new AMRent.Data.Models.View.LanguageIndex
                {
                    Id = x.Id,
                    Name = x.Name,
                    Code = x.Code,
                    CountryCode = x.CountryCode
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

        // GET: Languages
        public async Task<IActionResult> Index()
        {
              return View();
        }

        // GET: Languages/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Languages/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(dCore.MultiLanguage.Models.Language language)
        {
            if (ModelState.IsValid)
            {
                _context.Add(language);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(language);
        }

        // GET: Languages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Languages == null)
            {
                return NotFound();
            }

            var language = await _context.Languages.FindAsync(id);
            if (language == null)
            {
                return NotFound();
            }
            return View(language);
        }

        // POST: Languages/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, dCore.MultiLanguage.Models.Language language)
        {
            if (id != language.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(language);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LanguageExists(language.Id))
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
            return View(language);
        }

        // GET: Languages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Languages == null)
            {
                return NotFound();
            }

            var language = await _context.Languages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (language == null)
            {
                return NotFound();
            }

            return View(language);
        }

        // POST: Languages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Languages == null)
            {
                return Problem("Entity set 'FullDatabaseContext.Languages'  is null.");
            }
            var language = await _context.Languages.FindAsync(id);
            if (language != null)
            {
                _context.Languages.Remove(language);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LanguageExists(int id)
        {
          return (_context.Languages?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
