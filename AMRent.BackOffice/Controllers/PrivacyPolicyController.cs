using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AMRent.Data.Contexts;
using AMRent.Data.Models.Database;

namespace AMRent.BackOffice.Controllers
{
    [Extensions.AuthorizePermission(Data.Enums.Permissions.Other)]
    public class PrivacyPolicyController : BaseController
    {
        public PrivacyPolicyController(FullDatabaseContext context, ILogger<PrivacyPolicyController> logger) : base(context, logger)
        {
        }

        // GET: PrivacyPolicy/Edit
        public async Task<IActionResult> Edit()
        {
            if (_context.TranslatableSettings == null)
            {
                return NotFound();
            }

            var privacyPolicy = await _context.TranslatableSettings
                .Include(x => x.Translations)
                .FirstOrDefaultAsync(x => x.Code == Data.Enums.TranslatableSettings.PrivacyPolicy);
            if (privacyPolicy == null)
            {
                return NotFound();
            }
            ViewBag.Languages = _context.Languages.OrderBy(x => x.Id).ToList();
            return View(privacyPolicy);
        }

        // POST: PrivacyPolicy/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TranslatableSetting privacyPolicy)
        {
            if (id != privacyPolicy.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(privacyPolicy);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PrivacyPolicyExists(privacyPolicy.Id))
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
            return View(privacyPolicy);
        }

        private bool PrivacyPolicyExists(int id)
        {
            return (_context.TranslatableSettings?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
