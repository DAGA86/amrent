using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AMRent.Data.Contexts;
using AMRent.Data.Models.Database;

namespace AMRent.BackOffice.Controllers
{
    [Extensions.AuthorizePermission(Data.Enums.Permissions.Other)]
    public class CarFuelsController : BaseController
    {
        public CarFuelsController(FullDatabaseContext context, ILogger<CarFuelsController> logger) : base(context, logger)
        {
        }

        [HttpPost]
        public JsonResult Get(Models.DataTables.ViewModel viewModel)
        {
            JsonResult result = new JsonResult("");
            try
            {
                IQueryable<CarFuel> recordsTotal = _context.CarFuels
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
                    var predicate = PredicateBuilder.New<CarFuel>();
                    predicate = predicate.Or(x => x.Translations.FirstOrDefault(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese).Name.Contains(viewModel.search.value));

                    recordsFiltered = recordsFiltered.Where(predicate);
                }
                var recordsFilteredCount = recordsFiltered.Count();
                var recordsFilteredPage = recordsFiltered.Skip(viewModel.start).Take(viewModel.length).Select(x => new AMRent.Data.Models.View.CarFuelIndex()
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

        // GET: CarFuels
        public async Task<IActionResult> Index()
        {
            return View();
        }

        // GET: CarFuels/Create
        public IActionResult Create()
        {
            ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            return View();
        }

        // POST: CarFuels/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CarFuel carFuel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(carFuel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            return View(carFuel);
        }

        // GET: CarFuels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.CarFuels == null)
            {
                return NotFound();
            }

            var carFuel = await _context.CarFuels
                .Include(x => x.Translations)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (carFuel == null)
            {
                return NotFound();
            }
            ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            return View(carFuel);
        }

        // POST: CarFuels/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CarFuel carFuel)
        {
            if (id != carFuel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(carFuel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarFuelExists(carFuel.Id))
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
            return View(carFuel);
        }

        // GET: CarFuels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.CarFuels == null)
            {
                return NotFound();
            }

            var carFuel = await _context.CarFuels.Include(x => x.CarSegments)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (carFuel == null || carFuel.CarSegments.Any())
            {
                return NotFound();
            }

            _context.CarFuels.Remove(carFuel);
            await _context.SaveChangesAsync();

            return View(nameof(Index));
        }

        private bool CarFuelExists(int id)
        {
            return (_context.CarFuels?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
