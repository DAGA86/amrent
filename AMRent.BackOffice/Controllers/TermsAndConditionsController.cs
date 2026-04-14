using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AMRent.Data.Contexts;
using AMRent.Data.Models.Database;

namespace AMRent.BackOffice.Controllers
{
    [Extensions.AuthorizePermission(Data.Enums.Permissions.Other)]
    public class TermsAndConditionsController : BaseController
    {
        public TermsAndConditionsController(FullDatabaseContext context, ILogger<TermsAndConditionsController> logger) : base(context, logger)
        {
        }

        // GET: TermsAndConditions/Edit
        public async Task<IActionResult> Edit()
        {
            if (_context.TranslatableSettings == null)
            {
                return NotFound();
            }

            var termsAndConditions = await _context.TranslatableSettings
                .Include(x => x.Translations)
                .FirstOrDefaultAsync(x => x.Code == Data.Enums.TranslatableSettings.TermsAndConditions);
            if (termsAndConditions == null)
            {
                return NotFound();
            }
            ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            return View(termsAndConditions);
        }

        // POST: TermsAndConditions/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TranslatableSetting termsAndConditions)
        {
            if (id != termsAndConditions.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(termsAndConditions);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TermsAndConditionsExists(termsAndConditions.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            return View(termsAndConditions);
        }

        private bool TermsAndConditionsExists(int id)
        {
            return (_context.TranslatableSettings?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
