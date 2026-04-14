using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace AMRent.BackOffice.Controllers
{
    public class AccountController : Controller
    {
        private readonly Data.Contexts.FullDatabaseContext _context;

        public AccountController(Data.Contexts.FullDatabaseContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync(dCore.Identity.Models.UserViewModel model, string returnUrl = null)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Data.Models.Database.User? user = _context.Users
                        .Include(x => x.Roles)
                            .ThenInclude(x => x.RolePermissions)
                        .FirstOrDefault(x =>
                            x.Username == model.Username &&
                            x.Password == dCore.Cryptography.Providers.Generic.Encrypt(model.Password) &&
                            x.Roles.Any(y => y.Id != (int)Data.Enums.UserRoles.Customer));

                    if (user != null)
                    {
                        //List<string> claims = new List<string>();
                        //user.Roles.ForEach(x => x.RolePermissions.ForEach(x => claims.Add(x.Permission)));

                        List<Claim> newClaims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, user.Username),
                            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                            new Claim(ClaimTypes.Email, user.EmailAddress),
                            new Claim(ClaimTypes.UserData, user.Id.ToString())
                        };
                        //for (int i = 0; i < claims.Count; i++)
                        //{
                        //    newClaims.Add(new Claim(claims[i], claims[i]));
                        //}

                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(
                            newClaims, CookieAuthenticationDefaults.AuthenticationScheme);

                        var authProperties = new AuthenticationProperties
                        {
                            AllowRefresh = true,
                            // Refreshing the authentication session should be allowed.
                            //ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8),
                            // The time at which the authentication ticket expires. A 
                            // value set here overrides the ExpireTimeSpan option of 
                            // CookieAuthenticationOptions set with AddCookie.
                            IsPersistent = true,
                            // Whether the authentication session is persisted across 
                            // multiple requests. Required when setting the 
                            // ExpireTimeSpan option of CookieAuthenticationOptions 
                            // set with AddCookie. Also required when setting 
                            // ExpiresUtc.
                            IssuedUtc = DateTimeOffset.UtcNow,
                            // The time at which the authentication ticket was issued.
                            //RedirectUri = "~/Account/Login"
                            // The full path or absolute URI to be used as an http 
                            // redirect response value.
                        };

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                        if (Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Dados de login inválidos!");
                    }
                }

            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> LogoutAsync()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectPermanent("~/Account/Login");
        }
    }
}
