using AMRent.Data.Contexts;
using AMRent.Data.Models.Database;
using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AMRent.BackOffice.Controllers
{
    [Extensions.AuthorizePermission(Data.Enums.Permissions.Other)]
    public class VouchersController : BaseController
    {
        public VouchersController(FullDatabaseContext context, ILogger<VouchersController> logger) : base(context, logger)
        {
        }

        [HttpPost]
        public JsonResult Get(Models.DataTables.ViewModel viewModel)
        {
            JsonResult result = new JsonResult("");
            try
            {
                IQueryable<Voucher> recordsTotal = _context.Vouchers;
                var recordsTotalCount = recordsTotal.Count();
                var recordsFiltered = recordsTotal;

                if (viewModel.order.Any())
                {
                    switch (viewModel.order.First().column)
                    {
                        case 0:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.Code)
                                : recordsFiltered.OrderByDescending(x => x.Code);
                            break;
                        case 1:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.DiscountType)
                                : recordsFiltered.OrderByDescending(x => x.DiscountType);
                            break;
                        case 2:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.Value)
                                : recordsFiltered.OrderByDescending(x => x.Value);
                            break;
                        case 4:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.ValidFromUtc)
                                : recordsFiltered.OrderByDescending(x => x.ValidFromUtc);
                            break;
                        case 5:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.ValidUntilUtc)
                                : recordsFiltered.OrderByDescending(x => x.ValidUntilUtc);
                            break;
                    }
                }
                if (!string.IsNullOrEmpty(viewModel.search.value))
                {
                    var predicate = PredicateBuilder.New<Voucher>();
                    predicate = predicate.Or(x => x.Code.Contains(viewModel.search.value));

                    recordsFiltered = recordsFiltered.Where(predicate);
                }
                var recordsFilteredCount = recordsFiltered.Count();
                var recordsFilteredPage = recordsFiltered.Skip(viewModel.start).Take(viewModel.length).Select(x => new AMRent.Data.Models.View.VoucherIndex()
                {
                    Id = x.Id,
                    Code = x.Code,
                    DiscountType = Data.Enums.Generic.GetDescription(x.DiscountType),
                    Value = x.Value,
                    Extras = string.Join(',', x.Extras.Select(e => e.Translations.FirstOrDefault(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese).Name)),
                    ValidFromUtc = x.ValidFromUtc,
                    ValidUntilUtc = x.ValidUntilUtc
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

        // GET: Vouchers
        public async Task<IActionResult> Index()
        {
            return View();
        }

        // GET: Vouchers/Create
        public IActionResult Create()
        {
            ViewBag.Extras = new MultiSelectList(_context.Extras.Select(x => new { x.Id, x.Translations.FirstOrDefault(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese).Name }), "Id", "Name");
            return View();
        }

        // POST: Vouchers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Voucher voucher)
        {
            if (ModelState.IsValid)
            {
                foreach (int extraId in voucher.ExtraIds)
                {
                    voucher.Extras.Add(_context.Extras.FirstOrDefault(x => x.Id == extraId));
                }
                _context.Add(voucher);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Extras = new MultiSelectList(_context.Extras.Select(x => new { x.Id, x.Translations.FirstOrDefault(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese).Name }), "Id", "Name", voucher.Extras.Select(x => x.Id));
            return View(voucher);
        }

        // GET: Vouchers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Vouchers == null)
            {
                return NotFound();
            }

            var voucher = await _context.Vouchers
                .Include(x => x.Extras)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (voucher == null)
            {
                return NotFound();
            }
            ViewBag.Extras = new MultiSelectList(_context.Extras.Select(x => new { x.Id, x.Translations.FirstOrDefault(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese).Name }), "Id", "Name", voucher.Extras.Select(x => x.Id));
            return View(voucher);
        }

        // POST: Vouchers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Voucher voucher)
        {
            if (id != voucher.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.VoucherExtras.RemoveRange(_context.VoucherExtras.Where(x => x.VoucherId == voucher.Id));
                    foreach (int extraId in voucher.ExtraIds)
                    {
                        _context.VoucherExtras.Add(new VoucherExtra() { VoucherId = voucher.Id, ExtraId = extraId });
                    }
                    _context.Update(voucher);
                    await _context.SaveChangesAsync();
                    foreach (int extraId in voucher.ExtraIds)
                    {
                        voucher.Extras.Add(_context.Extras.FirstOrDefault(x => x.Id == extraId));
                    }
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VoucherExists(voucher.Id))
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
            ViewBag.Extras = new MultiSelectList(_context.Extras.Select(x => new { x.Id, x.Translations.FirstOrDefault(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese).Name }), "Id", "Name", voucher.Extras.Select(x => x.Id));
            return View(voucher);
        }

        // GET: Vouchers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Vouchers == null)
            {
                return NotFound();
            }

            var voucher = await _context.Vouchers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (voucher == null)
            {
                return NotFound();
            }

            return View(voucher);
        }

        // POST: Vouchers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Vouchers == null)
            {
                return Problem("Entity set 'FullDatabaseContext.Vouchers'  is null.");
            }
            var voucher = await _context.Vouchers.FindAsync(id);
            if (voucher != null)
            {
                _context.Vouchers.Remove(voucher);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VoucherExists(int id)
        {
            return (_context.Vouchers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
