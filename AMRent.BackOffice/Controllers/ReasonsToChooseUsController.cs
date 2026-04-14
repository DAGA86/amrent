using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AMRent.Data.Contexts;
using AMRent.Data.Models.Database;

namespace AMRent.BackOffice.Controllers
{
    [Extensions.AuthorizePermission(Data.Enums.Permissions.Other)]
    public class ReasonsToChooseUsController : BaseController
    {
        public ReasonsToChooseUsController(FullDatabaseContext context, ILogger<ReasonsToChooseUsController> logger) : base(context, logger)
        {
        }

        [HttpPost]
        public JsonResult Get(Models.DataTables.ViewModel viewModel)
        {
            JsonResult result = new JsonResult("");
            try
            {
                IQueryable<ReasonToChooseUs> recordsTotal = _context.ReasonsToChooseUs;
                var recordsTotalCount = recordsTotal.Count();
                var recordsFiltered = recordsTotal;

                if (viewModel.order.Any())
                {
                    switch (viewModel.order.First().column)
                    {
                        case 0:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Title)
                                : recordsFiltered.OrderByDescending(x => x.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Title);
                            break;
                    }
                }
                if (!string.IsNullOrEmpty(viewModel.search.value))
                {
                    var predicate = PredicateBuilder.New<ReasonToChooseUs>();
                    predicate = predicate.Or(x => x.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Title.Contains(viewModel.search.value));

                    recordsFiltered = recordsFiltered.Where(predicate);
                }
                var recordsFilteredCount = recordsFiltered.Count();
                var recordsFilteredPage = recordsFiltered.Skip(viewModel.start).Take(viewModel.length).Select(x => new AMRent.Data.Models.View.ReasonToChooseUsIndex()
                {
                    Id = x.Id,
                    Title = x.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Title
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

        // GET: ReasonsToChooseUs
        public async Task<IActionResult> Index()
        {
            return View();
        }

        // GET: ReasonsToChooseUs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ReasonsToChooseUs == null)
            {
                return NotFound();
            }

            var reasonToChooseUs = await _context.ReasonsToChooseUs
                .Include(x => x.Translations)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (reasonToChooseUs == null)
            {
                return NotFound();
            }
            ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            return View(reasonToChooseUs);
        }

        // POST: ReasonsToChooseUs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ReasonToChooseUs reasonToChooseUs)
        {
            if (id != reasonToChooseUs.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reasonToChooseUs);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReasonToChooseUsExists(reasonToChooseUs.Id))
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
            return View(reasonToChooseUs);
        }

        private bool ReasonToChooseUsExists(int id)
        {
            return (_context.ReasonsToChooseUs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
