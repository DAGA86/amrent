using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AMRent.Website.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IConfiguration _configuration;

        public AccountController(
            ILogger<HomeController> logger,
            AMRent.Data.Contexts.FullDatabaseContext context,
            dCore.MultiLanguage.Providers.TranslationProvider translationProvider,
            IConfiguration configuration) : base(logger, context, translationProvider)
        {
            _configuration = configuration;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [Filters.PassAlongQueryParamertersFilter()]
        public async Task<IActionResult> LoginAsync(dCore.Identity.Models.UserViewModel model, string returnUrl = null)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Data.Models.Database.User? user = _context.Users
                        .Include(x => x.Roles)
                        .FirstOrDefault(x =>
                            x.EmailAddress == model.Username &&
                            x.Password == dCore.Cryptography.Providers.Generic.Encrypt(model.Password) &&
                            x.Roles.Any(y => y.Id == (int)Data.Enums.UserRoles.Customer));

                    if (user != null)
                    {
                        if (!user.IsActive)
                        {
                            ModelState.AddModelError(string.Empty, "Utilizador inactivo. Verifique o seu email!");
                        }
                        else
                        {
                            List<string> claims = new List<string>();

                            List<Claim> newClaims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Email, user.EmailAddress),
                            new Claim(ClaimTypes.UserData, user.Id.ToString())
                        };
                            //for (int i = 0; i < claims.Count; i++)
                            //{
                            //    newClaims.Add(new Claim(claims[i], claims[i]));
                            //}

                            ClaimsIdentity claimsIdentity = new ClaimsIdentity(newClaims, CookieAuthenticationDefaults.AuthenticationScheme);

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

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [Filters.PassAlongQueryParamertersFilter()]
        public async Task<IActionResult> Register(dCore.Identity.Models.UserViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (_context.Users.Any(x => x.EmailAddress == model.Username))
                    {
                        ModelState.AddModelError(string.Empty, "Já existe um conta registada com o email introduzido!");
                    }
                    else
                    {
                        Data.Models.Database.User newUser = new Data.Models.Database.User
                        {
                            EmailAddress = model.Username,
                            Username = model.Username,
                            Password = dCore.Cryptography.Providers.Generic.Encrypt(model.Password),
                            IsLocked = false
                        };

                        newUser.Roles.Add(_context.Roles.FirstOrDefault(x => x.Id == (int)Data.Enums.UserRoles.Customer));
                        newUser.Reservations.AddRange(_context.Reservations.Where(x => (x.BillEmailAddress == newUser.EmailAddress || x.DriverEmailAddress == newUser.EmailAddress) && x.CustomerId == null));

                        newUser.Name = newUser.Reservations.FirstOrDefault(x => !string.IsNullOrEmpty(x.DriverName))?.DriverName;
                        if (string.IsNullOrEmpty(newUser.Name))
                        {
                            newUser.Name = newUser.Reservations.FirstOrDefault(x => !string.IsNullOrEmpty(x.BillName))?.BillName;
                        }
                        newUser.BirthDate = newUser.Reservations.FirstOrDefault(x => x.DriverBirthDate != null)?.DriverBirthDate;
                        newUser.TelephonePrefixCountryId = newUser.Reservations.FirstOrDefault(x => x.DriverTelephonePrefixCountryId != null)?.DriverTelephonePrefixCountryId;
                        if (!newUser.TelephonePrefixCountryId.HasValue)
                        {
                            newUser.TelephonePrefixCountryId = newUser.Reservations.FirstOrDefault(x => x.BillTelephonePrefixCountryId != null)?.BillTelephonePrefixCountryId;
                        }
                        newUser.Telephone = newUser.Reservations.FirstOrDefault(x => !string.IsNullOrEmpty(x.DriverTelephone))?.DriverTelephone;
                        if (string.IsNullOrEmpty(newUser.Telephone))
                        {
                            newUser.Telephone = newUser.Reservations.FirstOrDefault(x => !string.IsNullOrEmpty(x.BillTelephone))?.BillTelephone;
                        }
                        newUser.IdentityCountryId = newUser.Reservations.FirstOrDefault(x => x.DriverIdentityCountryId.HasValue)?.DriverIdentityCountryId;
                        newUser.IdentityNumber = newUser.Reservations.FirstOrDefault(x => !string.IsNullOrEmpty(x.DriverIdentityNumber))?.DriverIdentityNumber;
                        newUser.LicenseCountryId = newUser.Reservations.FirstOrDefault(x => x.DriverLicenseCountryId.HasValue)?.DriverLicenseCountryId;
                        newUser.LicenseNumber = newUser.Reservations.FirstOrDefault(x => !string.IsNullOrEmpty(x.DriverLicenseNumber))?.DriverLicenseNumber;
                        newUser.LicenseDate = newUser.Reservations.FirstOrDefault(x => x.DriverLicenseDate.HasValue)?.DriverLicenseDate;
                        newUser.LicenseExpireDate = newUser.Reservations.FirstOrDefault(x => x.DriverLicenseExpireDate.HasValue)?.DriverLicenseExpireDate;
                        newUser.Address = newUser.Reservations.FirstOrDefault(x => !string.IsNullOrEmpty(x.BillAddress))?.BillAddress;
                        newUser.PostalCode = newUser.Reservations.FirstOrDefault(x => !string.IsNullOrEmpty(x.BillPostalCode))?.BillPostalCode;
                        newUser.PostalLocation = newUser.Reservations.FirstOrDefault(x => !string.IsNullOrEmpty(x.BillPostalLocation))?.BillPostalLocation;
                        newUser.CountryId = newUser.Reservations.FirstOrDefault(x => x.BillCountryId.HasValue)?.BillCountryId;
                        newUser.VatNumber = newUser.Reservations.FirstOrDefault(x => !string.IsNullOrEmpty(x.BillVatNumber))?.BillVatNumber;

                        _context.Users.Add(newUser);
                        _context.SaveChanges();

                        dCore.Communication.Models.SmtpConfiguration smtpConfiguration = _configuration.GetSection("SmtpConfiguration").Get<dCore.Communication.Models.SmtpConfiguration>();
                        Shared.Providers.EmailSender emailSender = new Shared.Providers.EmailSender(_context, _translationProvider, smtpConfiguration, GetSelectedLanguageId());
                        emailSender.Send(Data.Enums.EmailContentTypes.AccountRegistered, _configuration["Environment"] == "Test", userId: newUser.Id);

                        ModelState.AddModelError(string.Empty, "Registo efectuado. Consulte o seu email!");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Dados inválidos!");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View(model);
        }

        public IActionResult Activate(string id)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == Guid.Parse(id));
            if (user != null)
            {
                user.IsActive = true;
                _context.SaveChanges();
                ModelState.AddModelError(string.Empty, "Conta activada. Faça login!");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Utilizador não encontrado!");
            }
            return View("Login");
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [Filters.PassAlongQueryParamertersFilter()]
        public async Task<IActionResult> ForgotPassword(Models.ForgotPassword model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!_context.Users.Any(x => x.EmailAddress == model.EmailAddress))
                    {
                        ModelState.AddModelError(string.Empty, "Não existe uma conta associada a este email!");
                    }
                    else
                    {
                        Guid userId = _context.Users.FirstOrDefault(x => x.EmailAddress == model.EmailAddress).Id;
                        dCore.Communication.Models.SmtpConfiguration smtpConfiguration = _configuration.GetSection("SmtpConfiguration").Get<dCore.Communication.Models.SmtpConfiguration>();
                        Shared.Providers.EmailSender emailSender = new Shared.Providers.EmailSender(_context, _translationProvider, smtpConfiguration, GetSelectedLanguageId());
                        emailSender.Send(Data.Enums.EmailContentTypes.PasswordReset, _configuration["Environment"] == "Test", userId: userId);

                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Dados inválidos!");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View(model);
        }

        public IActionResult ResetPassword(string id)
        {
            Models.ResetPassword model = new Models.ResetPassword
            {
                UserId = Guid.Parse(id)
            };
            return View(model);
        }

        [HttpPost]
        [Filters.PassAlongQueryParamertersFilter()]
        public async Task<IActionResult> ResetPassword(Models.ResetPassword model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.NewPassword != model.NewPasswordConfirm)
                    {
                        ModelState.AddModelError("", "As passwords são diferentes!");
                    }
                    else
                    {
                        Data.Models.Database.User user = _context.Users.FirstOrDefault(x => x.Id == model.UserId);
                        user.Password = dCore.Cryptography.Providers.Generic.Encrypt(model.NewPassword);
                        await _context.SaveChangesAsync();

                        return RedirectToAction("Login");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Dados inválidos!");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View(model);
        }

        [Authorize]
        public IActionResult Profile()
        {
            IQueryable<Data.Models.Database.Country> countries = _context.Countries;
            ViewBag.Countries = new SelectList(countries.Select(x => new { x.Id, x.Translations.FirstOrDefault(t => t.LanguageId == GetSelectedLanguageId()).Name }), "Id", "Name");
            BuildViewBag();
            Guid userId = Guid.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.UserData).Value);
            Data.Models.Database.User user = _context.Users.FirstOrDefault(x => x.Id == userId);
            return View(user);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Profile(Data.Models.Database.User user)
        {
            Guid userId = Guid.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.UserData).Value);
            Data.Models.Database.User databaseUser = _context.Users.FirstOrDefault(x => x.Id == userId);
            if (databaseUser != null)
            {
                databaseUser.Name = user.Name;
                databaseUser.BirthDate = user.BirthDate;
                databaseUser.EmailAddress = user.EmailAddress;
                databaseUser.TelephonePrefixCountryId = user.TelephonePrefixCountryId;
                databaseUser.Telephone = user.Telephone;
                databaseUser.IdentityCountryId = user.IdentityCountryId;
                databaseUser.IdentityNumber = user.IdentityNumber;
                databaseUser.LicenseCountryId = user.LicenseCountryId;
                databaseUser.LicenseNumber = user.LicenseNumber;
                databaseUser.LicenseDate = user.LicenseDate;
                databaseUser.LicenseExpireDate = user.LicenseExpireDate;
                databaseUser.Address = user.Address;
                databaseUser.PostalCode = user.PostalCode;
                databaseUser.PostalLocation = user.PostalLocation;
                databaseUser.CountryId = user.CountryId;
                databaseUser.VatNumber = user.VatNumber;

                _context.SaveChanges();
            }

            return RedirectToAction("Profile");
        }

        [Authorize]
        public IActionResult Reservations()
        {
            Guid userId = Guid.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.UserData).Value);
            List<Data.Models.Database.Reservation> reservations = _context.Reservations
                                                                        .Include(x => x.CarSegment)
                                                                        .ThenInclude(x => x.Translations.Where(y => y.LanguageId == GetSelectedLanguageId()))
                                                                        .Include(x => x.CarSegment)
                                                                        .ThenInclude(x => x.CarGearbox)
                                                                        .ThenInclude(x => x.Translations.Where(y => y.LanguageId == GetSelectedLanguageId()))
                                                                        .Include(x => x.CarSegment)
                                                                        .ThenInclude(x => x.CarFuel)
                                                                        .ThenInclude(x => x.Translations.Where(y => y.LanguageId == GetSelectedLanguageId()))
                                                                        .Where(x => x.CustomerId == userId)
                                                                        .ToList();

            foreach (var reservation in reservations)
            {
                Shared.Providers.CostCalculator costCalculator = new Shared.Providers.CostCalculator(_context, reservation.PickupDateTime, reservation.ReturnDateTime);
                reservation.TotalDays = costCalculator.GetTotalDays();
            }
            BuildViewBag();
            return View(reservations);
        }
    }
}