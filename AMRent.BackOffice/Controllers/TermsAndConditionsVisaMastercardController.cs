using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AMRent.Data.Contexts;
using AMRent.Data.Models.Database;

namespace AMRent.BackOffice.Controllers
{
    [Extensions.AuthorizePermission(Data.Enums.Permissions.Other)]
    public class TermsAndConditionsVisaMastercardController : BaseController
    {
        public TermsAndConditionsVisaMastercardController(FullDatabaseContext context, ILogger<TermsAndConditionsVisaMastercardController> logger) : base(context, logger)
        {
        }

        // GET: TermsAndConditionsVisaMastercard/Edit
        public async Task<IActionResult> Edit()
        {
            if (_context.TranslatableSettings == null)
            {
                return NotFound();
            }

            var termsAndConditionsVisaMastercard = await _context.TranslatableSettings
                .Include(x => x.Translations)
                .FirstOrDefaultAsync(x => x.Code == Data.Enums.TranslatableSettings.TermsAndConditionsVisaMastercard);
            if (termsAndConditionsVisaMastercard == null)
            {
                return NotFound();
            }
            ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            return View(termsAndConditionsVisaMastercard);
        }

        // POST: TermsAndConditionsVisaMastercard/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TranslatableSetting termsAndConditionsVisaMastercard)
        {
            if (id != termsAndConditionsVisaMastercard.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(termsAndConditionsVisaMastercard);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TermsAndConditionsVisaMastercardExists(termsAndConditionsVisaMastercard.Id))
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
            return View(termsAndConditionsVisaMastercard);
        }

        private bool TermsAndConditionsVisaMastercardExists(int id)
        {
            return (_context.TranslatableSettings?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
