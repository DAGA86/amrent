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
    public class UsersController : BaseController
    {
        public UsersController(FullDatabaseContext context, ILogger<UsersController> logger) : base(context, logger)
        {
        }

        [HttpPost]
        public JsonResult GetCustomers(Models.DataTables.ViewModel viewModel)
        {
            JsonResult result = new JsonResult("");
            try
            {
                IQueryable<User> recordsTotal = _context.Users.Where(x => x.Roles.Any(y => y.Id == (int)Data.Enums.UserRoles.Customer));
                var recordsTotalCount = recordsTotal.Count();
                var recordsFiltered = recordsTotal;

                if (viewModel.order.Any())
                {
                    switch (viewModel.order.First().column)
                    {
                        case 0:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.Name)
                                : recordsFiltered.OrderByDescending(x => x.Name);
                            break;
                        case 1:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.Quotations.Count)
                                : recordsFiltered.OrderByDescending(x => x.Quotations.Count);
                            break;
                        case 2:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.Reservations.Count)
                                : recordsFiltered.OrderByDescending(x => x.Reservations.Count);
                            break;
                    }
                }
                if (!string.IsNullOrEmpty(viewModel.search.value))
                {
                    var predicate = PredicateBuilder.New<User>();
                    predicate = predicate.Or(x => x.Name.Contains(viewModel.search.value));
                    predicate = predicate.Or(x => x.Username.Contains(viewModel.search.value));
                    predicate = predicate.Or(x => x.EmailAddress.Contains(viewModel.search.value));

                    recordsFiltered = recordsFiltered.Where(predicate);
                }
                var recordsFilteredCount = recordsFiltered.Count();
                var recordsFilteredPage = recordsFiltered.Skip(viewModel.start).Take(viewModel.length).Select(x => new AMRent.Data.Models.View.CustomerIndex()
                {
                    Id = x.Id,
                    Name = x.Name,
                    QuotationCount = x.Quotations.Count,
                    ReservationCount = x.Reservations.Count,
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

        // GET: Users/Customers
        public async Task<IActionResult> Customers()
        {
            return View();
        }

        // GET: Users/CustomerDetail/5
        public async Task<IActionResult> CustomerDetail(Guid? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var customer = await _context.Users
                .Include(x => x.Reservations)
                    .ThenInclude(y => y.CarSegment)
                .Include(x => x.Quotations)
                .Include(x => x.DataProtectionConsents)
                    .ThenInclude(x => x.DataProtectionConsent)
                        .ThenInclude(x => x.Translations.Where(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese))
                .FirstOrDefaultAsync(x => x.Roles.Any(x => x.Id == (int)Data.Enums.UserRoles.Customer) && x.Id == id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // GET: Seasons/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Seasons == null)
            {
                return NotFound();
            }

            var season = await _context.Seasons
                .FirstOrDefaultAsync(m => m.Id == id);
            if (season == null)
            {
                return NotFound();
            }

            return View(season);
        }

        // POST: Seasons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Seasons == null)
            {
                return Problem("Entity set 'FullDatabaseContext.Seasons'  is null.");
            }
            var season = await _context.Seasons.FindAsync(id);
            if (season != null)
            {
                _context.Seasons.Remove(season);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SeasonExists(int id)
        {
            return (_context.Seasons?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
