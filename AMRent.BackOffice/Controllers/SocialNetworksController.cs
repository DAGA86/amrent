using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AMRent.Data.Contexts;
using LinqKit;
using AMRent.Data.Models.Database;

namespace AMRent.BackOffice.Controllers
{
    [Extensions.AuthorizePermission(Data.Enums.Permissions.Other)]
    public class SocialNetworksController : BaseController
    {
        public SocialNetworksController(FullDatabaseContext context, ILogger<SocialNetworksController> logger) : base(context, logger)
        {
        }

        [HttpPost]
        public JsonResult Get(Models.DataTables.ViewModel viewModel)
        {
            JsonResult result = new JsonResult("");
            try
            {
                IQueryable<Data.Models.Database.SocialNetwork> recordsTotal = _context.SocialNetworks;
                var recordsTotalCount = recordsTotal.Count();
                var recordsFiltered = recordsTotal;

                if (viewModel.order.Any())
                {
                    switch (viewModel.order.First().column)
                    {
                        case 0:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ? recordsFiltered.OrderBy(x => x.Name) : recordsFiltered.OrderByDescending(x => x.Name);
                            break;
                    }
                }
                if (!string.IsNullOrEmpty(viewModel.search.value))
                {
                    var predicate = PredicateBuilder.New<Data.Models.Database.SocialNetwork>();
                    predicate = predicate.Or(x => x.Name.Contains(viewModel.search.value));

                    recordsFiltered = recordsFiltered.Where(predicate);
                }
                var recordsFilteredCount = recordsFiltered.Count();
                var recordsFilteredPage = recordsFiltered.Skip(viewModel.start).Take(viewModel.length).Select(x => new AMRent.Data.Models.View.SocialNetworkIndex()
                {
                    Id = x.Id,
                    Name = x.Name
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

        // GET: SocialNetworks
        public async Task<IActionResult> Index()
        {
              return View();
        }

        // GET: SocialNetworks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SocialNetworks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SocialNetwork socialNetwork)
        {
            if (ModelState.IsValid)
            {
                _context.Add(socialNetwork);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(socialNetwork);
        }

        // GET: SocialNetworks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.SocialNetworks == null)
            {
                return NotFound();
            }

            var socialNetwork = await _context.SocialNetworks.FindAsync(id);
            if (socialNetwork == null)
            {
                return NotFound();
            }
            return View(socialNetwork);
        }

        // POST: SocialNetworks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SocialNetwork socialNetwork)
        {
            if (id != socialNetwork.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(socialNetwork);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SocialNetworkExists(socialNetwork.Id))
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
            return View(socialNetwork);
        }

        // GET: SocialNetworks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.SocialNetworks == null)
            {
                return NotFound();
            }

            var socialNetwork = await _context.SocialNetworks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (socialNetwork == null)
            {
                return NotFound();
            }

            return View(socialNetwork);
        }

        // POST: SocialNetworks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.SocialNetworks == null)
            {
                return Problem("Entity set 'FullDatabaseContext.SocialNetworks'  is null.");
            }
            var socialNetwork = await _context.SocialNetworks.FindAsync(id);
            if (socialNetwork != null)
            {
                _context.SocialNetworks.Remove(socialNetwork);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SocialNetworkExists(int id)
        {
          return (_context.SocialNetworks?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
