using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AMRent.Data.Contexts;
using AMRent.Data.Models.Database;

namespace AMRent.BackOffice.Controllers
{
    [Extensions.AuthorizePermission(Data.Enums.Permissions.Other)]
    public class CarGearboxesController : BaseController
    {
        public CarGearboxesController(FullDatabaseContext context, ILogger<CarGearboxesController> logger) : base(context, logger)
        {
        }

        [HttpPost]
        public JsonResult Get(Models.DataTables.ViewModel viewModel)
        {
            JsonResult result = new JsonResult("");
            try
            {
                IQueryable<CarGearbox> recordsTotal = _context.CarGearboxes
                    .Include(x => x.CarSegments);
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
                    var predicate = PredicateBuilder.New<CarGearbox>();
                    predicate = predicate.Or(x => x.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Name.Contains(viewModel.search.value));

                    recordsFiltered = recordsFiltered.Where(predicate);
                }
                var recordsFilteredCount = recordsFiltered.Count();
                var recordsFilteredPage = recordsFiltered.Skip(viewModel.start).Take(viewModel.length).Select(x => new AMRent.Data.Models.View.CarGearboxIndex()
                {
                    Id = x.Id,
                    Name = x.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Name,
                    HasSegments = x.CarSegments.Any()
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

        // GET: CarGearboxes
        public async Task<IActionResult> Index()
        {
            return View();
        }

        // GET: CarGearboxes/Create
        public IActionResult Create()
        {
            ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            return View();
        }

        // POST: CarGearboxes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CarGearbox carGearbox)
        {
            if (ModelState.IsValid)
            {
                _context.Add(carGearbox);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            return View(carGearbox);
        }

        // GET: CarGearboxes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.CarGearboxes == null)
            {
                return NotFound();
            }

            var carGearbox = await _context.CarGearboxes
                .Include(x => x.Translations)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (carGearbox == null)
            {
                return NotFound();
            }
            ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            return View(carGearbox);
        }

        // POST: CarGearboxes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CarGearbox carGearbox)
        {
            if (id != carGearbox.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(carGearbox);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarGearboxExists(carGearbox.Id))
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
            return View(carGearbox);
        }

        // GET: CarGearboxes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.CarGearboxes == null)
            {
                return NotFound();
            }

            var carGearbox = await _context.CarGearboxes.Include(x => x.CarSegments)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (carGearbox == null || carGearbox.CarSegments.Any())
            {
                return NotFound();
            }

            _context.CarGearboxes.Remove(carGearbox);
            await _context.SaveChangesAsync();

            return View(nameof(Index));
        }

        private bool CarGearboxExists(int id)
        {
            return (_context.CarGearboxes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
