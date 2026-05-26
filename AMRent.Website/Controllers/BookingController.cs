using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AMRent.Data.Contexts;
using AMRent.Data.Models.Database;
using AMRent.Shared.Providers;
using System.Security.Claims;
using System.Text.Json;

namespace AMRent.Website.Controllers
{
    public class BookingController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly Shared.Providers.PayPal _payPalService;

        public BookingController(ILogger<HomeController> logger, FullDatabaseContext context, Shared.Providers.PayPal payPalService, dCore.MultiLanguage.Providers.TranslationProvider translationProvider, IConfiguration configuration) : base(logger, context, translationProvider)
        {
            _configuration = configuration;
            _payPalService = payPalService;
        }

        [HttpPost]
        [Filters.PassAlongQueryParamertersFilter()]
        public IActionResult Index(Models.BookingIndex viewModel)
        {
            TempData["ViewModel"] = JsonSerializer.Serialize(viewModel);
            return RedirectToAction(nameof(Index));
        }

        [Filters.PassAlongQueryParamertersFilter()]
        public IActionResult Index()
        {
            int selectedLanguageId = GetSelectedLanguageId();

            Models.BookingIndex viewModel = new();
            if (TempData.ContainsKey("ViewModel"))
            {
                viewModel = JsonSerializer.Deserialize<Models.BookingIndex>(TempData["ViewModel"] as string);
                TempData.Remove("ViewModel");
            }

            Data.Models.Database.CarSegment segment = _context.CarSegments
                .Include(x => x.Translations.Where(t => t.LanguageId == selectedLanguageId))
                .Include(x => x.Insurances.Where(y => y.InsuranceLevelId == viewModel.SelectedInsuranceLevelId))
                    .ThenInclude(x => x.InsuranceLevel)
                        .ThenInclude(x => x.Translations.Where(t => t.LanguageId == selectedLanguageId))
                .FirstOrDefault(x => x.Id == viewModel.SegmentId && x.IsActive);

            if (segment == null)
            {
                return RedirectToAction(nameof(Index), "Search");
            }

            Shared.Providers.PickupReturnLocation pickupReturnLocationProvider = new Shared.Providers.PickupReturnLocation(_context);
            DateTime validatedPickupDateTime =
                pickupReturnLocationProvider.GetNextCompliantWithAnticipationDateTime(
                    viewModel.PickupLocationId,
                    viewModel.PickupDateTime);
            DateTime dateTimeToRequest = viewModel.ReturnDateTime;
            if (dateTimeToRequest <= validatedPickupDateTime.AddMinutes(15))
            {
                dateTimeToRequest = validatedPickupDateTime.AddMinutes(15);
            }
            DateTime validatedReturnDateTime =
                pickupReturnLocationProvider.GetNextAvailableDateTime(
                    viewModel.ReturnLocationId, dateTimeToRequest);
            if (dCore.Helpers.DateTime.GetWithMinutePrecision(viewModel.PickupDateTime) != dCore.Helpers.DateTime.GetWithMinutePrecision(validatedPickupDateTime)
                || dCore.Helpers.DateTime.GetWithMinutePrecision(viewModel.ReturnDateTime) != dCore.Helpers.DateTime.GetWithMinutePrecision(validatedReturnDateTime))
            {
                return RedirectToAction("Detail", "Segment", new Models.SegmentDetail()
                {
                    SegmentId = viewModel.SegmentId,
                    PickupLocationId = viewModel.PickupLocationId,
                    ReturnLocationId = viewModel.ReturnLocationId,
                    PickupDateTime = viewModel.PickupDateTime,
                    ReturnDateTime = viewModel.ReturnDateTime,
                    //SelectedExtraIds = viewModel.SelectedExtraIds,
                    SelectedInsuranceLevelId = viewModel.SelectedInsuranceLevelId
                });
            }

            // Data
            viewModel.PickupLocationName = _context.PickupReturnLocations
                .Include(x => x.Translations.Where(x => x.LanguageId == selectedLanguageId))
                .FirstOrDefault(x => x.Id == viewModel.PickupLocationId).Translations.FirstOrDefault().Name;
            viewModel.ReturnLocationName = _context.PickupReturnLocations
                .Include(x => x.Translations.Where(x => x.LanguageId == selectedLanguageId))
                .FirstOrDefault(x => x.Id == viewModel.ReturnLocationId).Translations.FirstOrDefault().Name;

            Shared.Providers.CostCalculator costCalculatorHelper = new Shared.Providers.CostCalculator(_context, viewModel.PickupDateTime, viewModel.ReturnDateTime);
            costCalculatorHelper.SetSegment(viewModel.SegmentId);

            viewModel.Days = costCalculatorHelper.GetTotalDays();

            if (!string.IsNullOrEmpty(viewModel.VoucherCode))
            {
                viewModel.VoucherId = costCalculatorHelper.GetVoucherId(viewModel.VoucherCode, viewModel.Extras.Where(x => x.Quantity > 0).Select(x => x.Id).ToArray());

                if (User != null && User.Claims.Any())
                {
                    Guid userId = Guid.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.UserData).Value);
                    if (_context.Users.FirstOrDefault(x => x.Id == userId).Reservations.Any(x => x.Voucher.Code == viewModel.VoucherCode))
                    {
                        viewModel.VoucherId = null;
                    }
                }

                if (viewModel.VoucherId.HasValue)
                {
                    viewModel.CampaignVoucherName = viewModel.VoucherCode;
                }
            }

            if (!viewModel.VoucherId.HasValue)
            {
                viewModel.CampaignId = costCalculatorHelper.GetCampaignId(extraIds: viewModel.Extras.Where(x => x.Quantity > 0).Select(x => x.Id).ToArray());
                if (viewModel.CampaignId.HasValue)
                {
                    viewModel.CampaignVoucherName = _context.Campaigns.Include(x => x.Translations).FirstOrDefault(x => x.Id == viewModel.CampaignId).Translations.FirstOrDefault(x => x.LanguageId == selectedLanguageId).Name;
                }
            }

            viewModel.RentValue = costCalculatorHelper.GetCarCost().Value;
            viewModel.PickupValue = costCalculatorHelper.GetPickupReturnCost(viewModel.PickupLocationId);
            viewModel.ReturnValue = costCalculatorHelper.GetPickupReturnCost(viewModel.ReturnLocationId);
            foreach (Data.Models.Database.Insurance insurance in _context.CarSegments.AsNoTracking().Include(x => x.Insurances).ThenInclude(x => x.Prices).Include(x => x.Insurances).ThenInclude(x => x.InsuranceLevel).ThenInclude(x => x.Translations.Where(y => y.LanguageId == selectedLanguageId)).FirstOrDefault(x => x.Id == segment.Id).Insurances.Where(x => x.InsuranceLevelId == viewModel.SelectedInsuranceLevelId).ToList())
            {
                insurance.Prices.Clear();
                insurance.Prices.Add(new() { Value = costCalculatorHelper.GetInsuranceCost(insurance.InsuranceLevelId) });
                viewModel.Insurances.Add(insurance);
            }

            List<AMRent.Website.Models.SelectedExtra> extras = new();
            viewModel.Extras = viewModel.Extras.Where(x => x.Quantity > 0).ToList();
            if (viewModel.Extras.Any())
            {
                extras = _context.Extras
                .Include(x => x.Translations)
                .Where(x => viewModel.Extras.Select(y => y.Id).Contains(x.Id))
                .Select(x => new AMRent.Website.Models.SelectedExtra()
                {
                    Id = x.Id,
                    Name = x.Translations.FirstOrDefault(x => x.LanguageId == selectedLanguageId).Name,
                    Type = x.ExtraType,
                }).ToList();
                foreach (var extra in extras)
                {
                    extra.Quantity = viewModel.Extras.First(x => x.Id == extra.Id).Quantity;
                }
                foreach (KeyValuePair<int, decimal> extraCost in costCalculatorHelper.GetExtrasCost(extras.Select(x => x.Id).ToList(), viewModel.SelectedInsuranceLevelId))
                {
                    extras.FirstOrDefault(x => x.Id == extraCost.Key).DailyValue = extraCost.Value;
                    extras.FirstOrDefault(x => x.Id == extraCost.Key).Cost = extraCost.Value * extras.FirstOrDefault(x => x.Id == extraCost.Key).Quantity;
                }
            }
            viewModel.Extras = extras;
            Dictionary<int, Tuple<int, decimal>> extraDictionary = new();
            foreach (var extra in extras)
            {
                extraDictionary.Add(extra.Id, new Tuple<int, decimal>(extra.Quantity, extra.DailyValue));
            }

            foreach (KeyValuePair<int, Tuple<int, decimal>> pickupReturnTemporaryTax in costCalculatorHelper.GetPickupReturnTemporaryTaxes())
            {
                viewModel.PickupReturnTemporaryTaxes.Add(new Models.PickupReturnTemporaryTax()
                {
                    Id = pickupReturnTemporaryTax.Key,
                    Quantity = pickupReturnTemporaryTax.Value.Item1,
                    UnitValue = pickupReturnTemporaryTax.Value.Item2,
                    Cost = pickupReturnTemporaryTax.Value.Item1 * pickupReturnTemporaryTax.Value.Item2,
                    Name = _context.PickupReturnTemporaryTaxes.Include(x => x.Translations.Where(x => x.LanguageId == selectedLanguageId)).FirstOrDefault(x => x.Id == pickupReturnTemporaryTax.Key).Translations.FirstOrDefault().Name
                });
            }

            viewModel.SegmentName = segment.Translations?.FirstOrDefault()?.Name;

            viewModel.TotalValue = costCalculatorHelper.GetTotalCost(viewModel.RentValue,
                                                                        viewModel.PickupValue,
                                                                        viewModel.ReturnValue,
                                                                        viewModel.Insurances.FirstOrDefault(x => x.InsuranceLevelId == viewModel.SelectedInsuranceLevelId).Prices.FirstOrDefault().Value,
                                                                        extras.Sum(x => x.Cost),
                                                                        viewModel.PickupReturnTemporaryTaxes.Sum(x => x.Cost),
                                                                        0,
                                                                        viewModel.VoucherId,
                                                                        viewModel.CampaignId,
                                                                        extraDictionary);

            if (User != null && User.Claims.Any())
            {
                Guid userId = Guid.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.UserData).Value);
                Data.Models.Database.User user = _context.Users.FirstOrDefault(x => x.Id == userId);

                viewModel.DriverName = user.Name;
                viewModel.BillName = user.Name;
                if (user.BirthDate.HasValue)
                {
                    viewModel.DriverBirthDate = user.BirthDate.Value;
                }
                viewModel.DriverEmail = user.EmailAddress;
                viewModel.BillEmail = user.EmailAddress;
                if (user.TelephonePrefixCountryId.HasValue)
                {
                    viewModel.DriverTelephoneCountryId = user.TelephonePrefixCountryId.Value;
                    viewModel.BillTelephoneCountryId = user.TelephonePrefixCountryId.Value;
                }
                viewModel.DriverTelephone = user.Telephone ?? "";
                viewModel.BillTelephone = user.Telephone ?? "";
                if (user.IdentityCountryId.HasValue)
                {
                    viewModel.DriverIdentityCardCountryId = user.IdentityCountryId.Value;
                }
                viewModel.DriverIdentityCardNumber = user.IdentityNumber ?? "";
                if (user.LicenseCountryId.HasValue)
                {
                    viewModel.DriverLicenseCountryId = user.LicenseCountryId.Value;
                }
                viewModel.DriverLicenseNumber = user.LicenseNumber ?? "";
                if (user.LicenseDate.HasValue)
                {
                    viewModel.DriverLicenseDate = user.LicenseDate.Value;
                }
                if (user.LicenseExpireDate.HasValue)
                {
                    viewModel.DriverLicenseExpireDate = user.LicenseExpireDate.Value;
                }
                viewModel.BillAddress = user.Address ?? "";
                viewModel.BillPostalCode = user.PostalCode ?? "";
                viewModel.BillPostalLocation = user.PostalLocation ?? "";
                if (user.CountryId.HasValue)
                {
                    viewModel.BillCountryId = user.CountryId.Value;
                }
                viewModel.BillVatNumber = user.VatNumber ?? "";
                viewModel.DriverVatNumber = user.VatNumber ?? "";
            }

            viewModel.DataProtectionConsents = _context.DataProtectionConsents
                .Include(x => x.Translations.Where(t => t.LanguageId == selectedLanguageId))
                .Where(x => x.IsActive)
                .OrderBy(x => x.SortNumber)
                .Select(x => new Models.DataProtectionConsent()
                {
                    Id = x.Id,
                    Text = x.Translations.FirstOrDefault(t => t.LanguageId == selectedLanguageId).Text,
                    IsRequired = x.IsMandatory
                }).ToList();

            ViewBag.IdentityTypes = new SelectList(dCore.Helpers.Enum.GetWithDescription<Data.Enums.IdentityType>().Select(x => new
            {
                Text = _translationProvider.GetTranslation(selectedLanguageId, x.Value),
                Value = x.Key
            }), "Value", "Text", viewModel.DriverIdentityType);

            IQueryable<Data.Models.Database.Country> countries = _context.Countries;
            ViewBag.Countries = new SelectList(countries.Select(x => new { x.Id, x.Translations.FirstOrDefault(t => t.LanguageId == selectedLanguageId).Name }), "Id", "Name");

            BuildViewBag();

            return View(viewModel);
        }

        [HttpPost]
        [Filters.PassAlongQueryParamertersFilter()]
        public IActionResult AskQuotation(Models.BookingAskQuotation viewModel)
        {
            TempData["ViewModel"] = JsonSerializer.Serialize(viewModel);
            return RedirectToAction(nameof(AskQuotation));
        }

        [Filters.PassAlongQueryParamertersFilter()]
        public IActionResult AskQuotation()
        {
            int selectedLanguageId = GetSelectedLanguageId();

            Models.BookingAskQuotation viewModel = new();
            if (TempData.ContainsKey("ViewModel"))
            {
                viewModel = JsonSerializer.Deserialize<Models.BookingAskQuotation>(TempData["ViewModel"] as string);
                TempData.Remove("ViewModel");
            }

            Data.Models.Database.CarSegment segment = _context.CarSegments
                .Include(x => x.Translations.Where(t => t.LanguageId == selectedLanguageId))
                .Include(x => x.Insurances.Where(y => y.InsuranceLevelId == viewModel.SelectedInsuranceLevelId))
                    .ThenInclude(x => x.InsuranceLevel)
                        .ThenInclude(x => x.Translations.Where(t => t.LanguageId == selectedLanguageId))
                .FirstOrDefault(x => x.Id == viewModel.SegmentId && x.IsActive);

            if (segment == null)
            {
                return RedirectToAction(nameof(Index), "Search");
            }

            Shared.Providers.PickupReturnLocation pickupReturnLocationProvider = new Shared.Providers.PickupReturnLocation(_context);
            DateTime validatedPickupDateTime =
                pickupReturnLocationProvider.GetNextCompliantWithAnticipationDateTime(
                    viewModel.PickupLocationId,
                    viewModel.PickupDateTime);
            DateTime dateTimeToRequest = viewModel.ReturnDateTime;
            if (dateTimeToRequest <= validatedPickupDateTime.AddMinutes(15))
            {
                dateTimeToRequest = validatedPickupDateTime.AddMinutes(15);
            }
            DateTime validatedReturnDateTime =
                pickupReturnLocationProvider.GetNextAvailableDateTime(
                    viewModel.ReturnLocationId, dateTimeToRequest);
            if (dCore.Helpers.DateTime.GetWithMinutePrecision(viewModel.PickupDateTime) != dCore.Helpers.DateTime.GetWithMinutePrecision(validatedPickupDateTime)
                || dCore.Helpers.DateTime.GetWithMinutePrecision(viewModel.ReturnDateTime) != dCore.Helpers.DateTime.GetWithMinutePrecision(validatedReturnDateTime))
            {
                return RedirectToAction("Detail", "Segment", new Models.SegmentDetail()
                {
                    SegmentId = viewModel.SegmentId,
                    PickupLocationId = viewModel.PickupLocationId,
                    ReturnLocationId = viewModel.ReturnLocationId,
                    PickupDateTime = viewModel.PickupDateTime,
                    ReturnDateTime = viewModel.ReturnDateTime,
                    //SelectedExtraIds = viewModel.SelectedExtraIds,
                    SelectedInsuranceLevelId = viewModel.SelectedInsuranceLevelId
                });
            }

            // Data
            viewModel.PickupLocationName = _context.PickupReturnLocations
                .Include(x => x.Translations.Where(x => x.LanguageId == selectedLanguageId))
                .FirstOrDefault(x => x.Id == viewModel.PickupLocationId).Translations.FirstOrDefault().Name;
            viewModel.ReturnLocationName = _context.PickupReturnLocations
                .Include(x => x.Translations.Where(x => x.LanguageId == selectedLanguageId))
                .FirstOrDefault(x => x.Id == viewModel.ReturnLocationId).Translations.FirstOrDefault().Name;

            Shared.Providers.CostCalculator costCalculatorHelper = new Shared.Providers.CostCalculator(_context, viewModel.PickupDateTime, viewModel.ReturnDateTime);
            costCalculatorHelper.SetSegment(viewModel.SegmentId);

            viewModel.Days = costCalculatorHelper.GetTotalDays();

            if (!string.IsNullOrEmpty(viewModel.VoucherCode))
            {
                viewModel.VoucherId = costCalculatorHelper.GetVoucherId(viewModel.VoucherCode, extraIds: viewModel.Extras.Where(x => x.Quantity > 0).Select(x => x.Id).ToArray());

                if (User != null && User.Claims.Any())
                {
                    Guid userId = Guid.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.UserData).Value);
                    if (_context.Users.FirstOrDefault(x => x.Id == userId).Reservations.Any(x => x.Voucher.Code == viewModel.VoucherCode))
                    {
                        viewModel.VoucherId = null;
                    }
                }

                if (viewModel.VoucherId.HasValue)
                {
                    viewModel.CampaignVoucherName = viewModel.VoucherCode;
                }
            }

            if (!viewModel.VoucherId.HasValue)
            {
                viewModel.CampaignId = costCalculatorHelper.GetCampaignId(extraIds: viewModel.Extras.Where(x => x.Quantity > 0).Select(x => x.Id).ToArray());
                if (viewModel.CampaignId.HasValue)
                {
                    viewModel.CampaignVoucherName = _context.Campaigns.Include(x => x.Translations).FirstOrDefault(x => x.Id == viewModel.CampaignId).Translations.FirstOrDefault(x => x.LanguageId == selectedLanguageId).Name;
                }
            }

            viewModel.RentValue = costCalculatorHelper.GetCarCost().Value;
            viewModel.PickupValue = costCalculatorHelper.GetPickupReturnCost(viewModel.PickupLocationId);
            viewModel.ReturnValue = costCalculatorHelper.GetPickupReturnCost(viewModel.ReturnLocationId);
            foreach (Data.Models.Database.Insurance insurance in _context.CarSegments.AsNoTracking().Include(x => x.Insurances).ThenInclude(x => x.Prices).Include(x => x.Insurances).ThenInclude(x => x.InsuranceLevel).ThenInclude(x => x.Translations.Where(y => y.LanguageId == selectedLanguageId)).FirstOrDefault(x => x.Id == segment.Id).Insurances.Where(x => x.InsuranceLevelId == viewModel.SelectedInsuranceLevelId).ToList())
            {
                insurance.Prices.Add(new() { Value = costCalculatorHelper.GetInsuranceCost(insurance.InsuranceLevelId) });
                viewModel.Insurances.Add(insurance);
            }

            List<AMRent.Website.Models.SelectedExtra> extras = new();
            viewModel.Extras = viewModel.Extras.Where(x => x.Quantity > 0).ToList();
            if (viewModel.Extras.Any())
            {
                extras = _context.Extras
                .Include(x => x.Translations)
                .Where(x => viewModel.Extras.Select(y => y.Id).Contains(x.Id))
                .Select(x => new AMRent.Website.Models.SelectedExtra()
                {
                    Id = x.Id,
                    Name = x.Translations.FirstOrDefault(x => x.LanguageId == selectedLanguageId).Name,
                    Type = x.ExtraType,
                }).ToList();
                foreach (var extra in extras)
                {
                    extra.Quantity = viewModel.Extras.First(x => x.Id == extra.Id).Quantity;
                }
                foreach (KeyValuePair<int, decimal> extraCost in costCalculatorHelper.GetExtrasCost(extras.Select(x => x.Id).ToList(), viewModel.SelectedInsuranceLevelId))
                {
                    extras.FirstOrDefault(x => x.Id == extraCost.Key).DailyValue = extraCost.Value;
                    extras.FirstOrDefault(x => x.Id == extraCost.Key).Cost = extraCost.Value * extras.FirstOrDefault(x => x.Id == extraCost.Key).Quantity;
                }
            }
            viewModel.Extras = extras;
            Dictionary<int, Tuple<int, decimal>> extraDictionary = new();
            foreach (var extra in extras)
            {
                extraDictionary.Add(extra.Id, new Tuple<int, decimal>(extra.Quantity, extra.DailyValue));
            }

            foreach (KeyValuePair<int, Tuple<int, decimal>> pickupReturnTemporaryTax in costCalculatorHelper.GetPickupReturnTemporaryTaxes())
            {
                viewModel.PickupReturnTemporaryTaxes.Add(new Models.PickupReturnTemporaryTax()
                {
                    Id = pickupReturnTemporaryTax.Key,
                    Quantity = pickupReturnTemporaryTax.Value.Item1,
                    UnitValue = pickupReturnTemporaryTax.Value.Item2,
                    Cost = pickupReturnTemporaryTax.Value.Item1 * pickupReturnTemporaryTax.Value.Item2,
                    Name = _context.PickupReturnTemporaryTaxes.Include(x => x.Translations.FirstOrDefault(x => x.LanguageId == selectedLanguageId)).FirstOrDefault(x => x.Id == pickupReturnTemporaryTax.Key).Translations.FirstOrDefault().Name
                });
            }

            viewModel.SegmentName = segment.Translations?.FirstOrDefault()?.Name;

            viewModel.TotalValue = costCalculatorHelper.GetTotalCost(viewModel.RentValue,
                                                                        viewModel.PickupValue,
                                                                        viewModel.ReturnValue,
                                                                        viewModel.Insurances.FirstOrDefault(x => x.InsuranceLevelId == viewModel.SelectedInsuranceLevelId).Prices.FirstOrDefault().Value,
                                                                        extras.Sum(x => x.Cost),
                                                                        viewModel.PickupReturnTemporaryTaxes.Sum(x => x.Cost),
                                                                        0,
                                                                        viewModel.VoucherId,
                                                                        viewModel.CampaignId,
                                                                        extraDictionary);

            if (User != null && User.Claims.Any())
            {
                Guid userId = Guid.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.UserData).Value);
                Data.Models.Database.User user = _context.Users.FirstOrDefault(x => x.Id == userId);

                viewModel.DriverName = user.Name;
                if (user.BirthDate.HasValue)
                {
                    viewModel.DriverBirthDate = user.BirthDate.Value;
                }
                viewModel.DriverEmail = user.EmailAddress;
                if (user.TelephonePrefixCountryId.HasValue)
                {
                    viewModel.DriverTelephoneCountryId = user.TelephonePrefixCountryId.Value;
                }
                viewModel.DriverTelephone = user.Telephone ?? "";
            }

            viewModel.DataProtectionConsents = _context.DataProtectionConsents
                .Include(x => x.Translations.Where(t => t.LanguageId == selectedLanguageId))
                .Where(x => x.IsActive)
                .OrderBy(x => x.SortNumber)
                .Select(x => new Models.DataProtectionConsent()
                {
                    Id = x.Id,
                    Text = x.Translations.FirstOrDefault(t => t.LanguageId == selectedLanguageId).Text,
                    IsRequired = x.IsMandatory
                }).ToList();

            IQueryable<Data.Models.Database.Country> countries = _context.Countries;
            ViewBag.Countries = new SelectList(countries.Select(x => new { x.Id, x.Translations.FirstOrDefault(t => t.LanguageId == selectedLanguageId).Name }), "Id", "Name");

            BuildViewBag();

            return View(viewModel);
        }

        [HttpPost]
        [Filters.PassAlongQueryParamertersFilter()]
        public IActionResult Confirm(Models.BookingIndex viewModel)
        {
            TempData["ViewModel"] = JsonSerializer.Serialize(viewModel);
            return RedirectToAction(nameof(Confirm));
        }

        [Filters.PassAlongQueryParamertersFilter()]
        public async Task<IActionResult> Confirm()
        {
            int selectedLanguageId = GetSelectedLanguageId();

            Models.BookingIndex viewModel = new();
            if (TempData.ContainsKey("ViewModel"))
            {
                viewModel = JsonSerializer.Deserialize<Models.BookingIndex>(TempData["ViewModel"] as string);
                TempData.Remove("ViewModel");
            }

            Data.Models.Database.CarSegment segment = _context.CarSegments
                .Include(x => x.Translations.Where(t => t.LanguageId == selectedLanguageId))
                .Include(x => x.Insurances.Where(y => y.InsuranceLevelId == viewModel.SelectedInsuranceLevelId))
                    .ThenInclude(x => x.InsuranceLevel)
                        .ThenInclude(x => x.Translations.Where(t => t.LanguageId == selectedLanguageId))
                .FirstOrDefault(x => x.Id == viewModel.SegmentId && x.IsActive);

            if (segment == null)
            {
                return RedirectToAction(nameof(Index), "Search");
            }

            Shared.Providers.PickupReturnLocation pickupReturnLocationProvider = new Shared.Providers.PickupReturnLocation(_context);

            DateTime validatedPickupDateTime =
                pickupReturnLocationProvider.GetNextCompliantWithAnticipationDateTime(
                    viewModel.PickupLocationId,
                    viewModel.PickupDateTime);
            DateTime dateTimeToRequest = viewModel.ReturnDateTime;
            if (dateTimeToRequest <= validatedPickupDateTime.AddMinutes(15))
            {
                dateTimeToRequest = validatedPickupDateTime.AddMinutes(15);
            }
            DateTime validatedReturnDateTime =
                pickupReturnLocationProvider.GetNextAvailableDateTime(
                    viewModel.ReturnLocationId, dateTimeToRequest);

            if (dCore.Helpers.DateTime.GetWithMinutePrecision(viewModel.PickupDateTime) != dCore.Helpers.DateTime.GetWithMinutePrecision(validatedPickupDateTime)
                || dCore.Helpers.DateTime.GetWithMinutePrecision(viewModel.ReturnDateTime) != dCore.Helpers.DateTime.GetWithMinutePrecision(validatedReturnDateTime))
            {
                return RedirectToAction("Detail", "Segment", new Models.SegmentDetail()
                {
                    SegmentId = viewModel.SegmentId,
                    PickupLocationId = viewModel.PickupLocationId,
                    ReturnLocationId = viewModel.ReturnLocationId,
                    PickupDateTime = viewModel.PickupDateTime,
                    ReturnDateTime = viewModel.ReturnDateTime,
                    //SelectedExtraIds = viewModel.SelectedExtraIds,
                    SelectedInsuranceLevelId = viewModel.SelectedInsuranceLevelId
                });
            }

            Shared.Providers.CostCalculator costCalculatorHelper = new Shared.Providers.CostCalculator(_context, viewModel.PickupDateTime, viewModel.ReturnDateTime);
            costCalculatorHelper.SetSegment(viewModel.SegmentId);
            viewModel.Days = costCalculatorHelper.GetTotalDays();

            if (!string.IsNullOrEmpty(viewModel.VoucherCode))
            {
                viewModel.VoucherId = costCalculatorHelper.GetVoucherId(viewModel.VoucherCode, extraIds: viewModel.Extras.Where(x => x.Quantity > 0).Select(x => x.Id).ToArray());

                if (User != null && User.Claims.Any())
                {
                    Guid userId = Guid.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.UserData).Value);
                    if (_context.Users.FirstOrDefault(x => x.Id == userId).Reservations.Any(x => x.Voucher.Code == viewModel.VoucherCode))
                    {
                        viewModel.VoucherId = null;
                    }
                }

                if (viewModel.VoucherId.HasValue)
                {
                    viewModel.CampaignVoucherName = viewModel.VoucherCode;
                }
            }

            if (!viewModel.VoucherId.HasValue)
            {
                viewModel.CampaignId = costCalculatorHelper.GetCampaignId(extraIds: viewModel.Extras.Where(x => x.Quantity > 0).Select(x => x.Id).ToArray());
                if (viewModel.CampaignId.HasValue)
                {
                    viewModel.CampaignVoucherName = _context.Campaigns.Include(x => x.Translations).FirstOrDefault(x => x.Id == viewModel.CampaignId).Translations.FirstOrDefault(x => x.LanguageId == selectedLanguageId).Name;
                }
            }

            var rentValue = costCalculatorHelper.GetCarCost().Value;
            var pickupValue = costCalculatorHelper.GetPickupReturnCost(viewModel.PickupLocationId);
            var returnValue = costCalculatorHelper.GetPickupReturnCost(viewModel.ReturnLocationId);
            var insurances = _context.CarSegments.AsNoTracking().Include(x => x.Insurances).ThenInclude(x => x.InsuranceLevel).ThenInclude(x => x.Translations.Where(y => y.LanguageId == selectedLanguageId)).FirstOrDefault(x => x.Id == segment.Id).Insurances.Where(x => x.InsuranceLevelId == viewModel.SelectedInsuranceLevelId).ToList();
            foreach (var insurance in insurances)
            {
                insurance.Prices.Add(new() { Value = costCalculatorHelper.GetInsuranceCost(insurance.InsuranceLevelId) });
            }

            List<AMRent.Website.Models.SelectedExtra> extras = new();
            if (viewModel.Extras.Any())
            {
                extras = _context.Extras
                .Include(x => x.Translations)
                .Where(x => viewModel.Extras.Select(x => x.Id).Contains(x.Id))
                    .Select(x => new AMRent.Website.Models.SelectedExtra()
                    {
                        Id = x.Id,
                        Name = x.Translations.FirstOrDefault(x => x.LanguageId == selectedLanguageId).Name,
                        Type = x.ExtraType
                    }).ToList();

                foreach (KeyValuePair<int, decimal> extraCost in costCalculatorHelper.GetExtrasCost(extras.Select(x => x.Id).ToList(), viewModel.SelectedInsuranceLevelId))
                {
                    extras.FirstOrDefault(x => x.Id == extraCost.Key).Quantity = viewModel.Extras.First(x => x.Id == extraCost.Key).Quantity;
                    extras.FirstOrDefault(x => x.Id == extraCost.Key).DailyValue = extraCost.Value;
                    extras.FirstOrDefault(x => x.Id == extraCost.Key).Cost = extras.FirstOrDefault(x => x.Id == extraCost.Key).Quantity * extras.FirstOrDefault(x => x.Id == extraCost.Key).DailyValue;
                }
            }

            if (dCore.Helpers.DateTime.GetWithMinutePrecision(viewModel.PickupDateTime) != dCore.Helpers.DateTime.GetWithMinutePrecision(validatedPickupDateTime)
                || dCore.Helpers.DateTime.GetWithMinutePrecision(viewModel.ReturnDateTime) != dCore.Helpers.DateTime.GetWithMinutePrecision(validatedReturnDateTime)
                || viewModel.RentValue != rentValue || viewModel.PickupValue != pickupValue || viewModel.ReturnValue != returnValue)
            {
                return RedirectToAction("Detail", "Segment", new Models.SegmentDetail()
                {
                    SegmentId = viewModel.SegmentId,
                    PickupLocationId = viewModel.PickupLocationId,
                    ReturnLocationId = viewModel.ReturnLocationId,
                    PickupDateTime = viewModel.PickupDateTime,
                    ReturnDateTime = viewModel.ReturnDateTime,
                    //SelectedExtraIds = viewModel.SelectedExtraIds,
                    SelectedInsuranceLevelId = viewModel.SelectedInsuranceLevelId
                });
            }

            Data.Models.Database.Reservation reservation = new()
            {
                Source = Data.Enums.ReservationQuotationSources.W,
                Status = Data.Enums.ReservationStatus.Registered,

                CarSegmentId = viewModel.SegmentId,
                InsuranceLevelId = viewModel.SelectedInsuranceLevelId,

                PickupLocationId = viewModel.PickupLocationId,
                PickupDateTime = viewModel.PickupDateTime,
                ReturnLocationId = viewModel.ReturnLocationId,
                ReturnDateTime = viewModel.ReturnDateTime,

                DriverName = viewModel.DriverName,
                DriverBirthDate = viewModel.DriverBirthDate,
                DriverEmailAddress = viewModel.DriverEmail,
                DriverTelephonePrefixCountryId = viewModel.DriverTelephoneCountryId,
                DriverTelephone = viewModel.DriverTelephone,
                DriverIdentityType = viewModel.DriverIdentityType,
                DriverIdentityCountryId = viewModel.DriverIdentityCardCountryId,
                DriverIdentityNumber = viewModel.DriverIdentityCardNumber,
                DriverVatNumber = viewModel.DriverVatNumber,
                DriverLicenseCountryId = viewModel.DriverLicenseCountryId,
                DriverLicenseNumber = viewModel.DriverLicenseNumber,
                DriverLicenseDate = viewModel.DriverLicenseDate,
                DriverLicenseExpireDate = viewModel.DriverLicenseExpireDate,

                BillName = viewModel.BillName,
                BillEmailAddress = viewModel.BillEmail,
                BillTelephonePrefixCountryId = viewModel.BillTelephoneCountryId,
                BillTelephone = viewModel.BillTelephone,
                BillAddress = viewModel.BillAddress,
                BillPostalCode = viewModel.BillPostalCode,
                BillPostalLocation = viewModel.BillPostalLocation,
                BillCountryId = viewModel.BillCountryId,
                BillVatNumber = viewModel.BillVatNumber,

                Comments = viewModel.Comments,
                FlightNumber = viewModel.FlightNumber,
                PaymentType = viewModel.PaymentType,
                HasAdvancePartialPayment = viewModel.PaymentAmountType == Models.PaymentAmountType.Deposit,

                CampaignId = viewModel.CampaignId,
                VoucherId = viewModel.VoucherId,
            };

            foreach (Models.SelectedExtra selectedExtra in extras)
            {
                reservation.Extras.Add(new()
                {
                    ExtraId = selectedExtra.Id,
                    UnitValue = selectedExtra.DailyValue,
                    Quantity = selectedExtra.Quantity
                });
            }

            foreach (KeyValuePair<int, Tuple<int, decimal>> pickupReturnTemporaryTax in costCalculatorHelper.GetPickupReturnTemporaryTaxes())
            {
                reservation.PickupReturnTemporaryTaxes.Add(new Data.Models.Database.ReservationPickupReturnTemporaryTax()
                {
                    PickupReturnTemporaryTaxId = pickupReturnTemporaryTax.Key,
                    Quantity = pickupReturnTemporaryTax.Value.Item1,
                    UnitValue = pickupReturnTemporaryTax.Value.Item2,
                });
                viewModel.PickupReturnTemporaryTaxes.Add(new Models.PickupReturnTemporaryTax()
                {
                    Id = pickupReturnTemporaryTax.Key,
                    Quantity = pickupReturnTemporaryTax.Value.Item1,
                    UnitValue = pickupReturnTemporaryTax.Value.Item2,
                    Cost = pickupReturnTemporaryTax.Value.Item1 * pickupReturnTemporaryTax.Value.Item2,
                    Name = _context.PickupReturnTemporaryTaxes.Include(x => x.Translations.Where(x => x.LanguageId == selectedLanguageId)).FirstOrDefault(x => x.Id == pickupReturnTemporaryTax.Key).Translations.FirstOrDefault().Name
                });
            }

            // Data

            foreach (var extraDriver in viewModel.ExtraDrivers)
            {
                reservation.ExtraDrivers.Add(new()
                {
                    Name = extraDriver.Name,
                    BirthDate = extraDriver.BirthDate,
                    LicenseCountryId = extraDriver.LicenseCountryId,
                    LicenseNumber = extraDriver.LicenseNumber,
                    LicenseDate = extraDriver.LicenseDate,
                    LicenseExpireDate = extraDriver.LicenseExpireDate,
                });
            }

            foreach (Models.DataProtectionConsent dataProtectionConsent in viewModel.DataProtectionConsents)
            {
                reservation.DataProtectionConsents.Add(new()
                {
                    DataProtectionConsentId = dataProtectionConsent.Id,
                    HasConsented = dataProtectionConsent.HasConsent
                });
            }

            reservation.CarSegmentCost = rentValue;
            reservation.PickupCost = pickupValue;
            reservation.ReturnCost = returnValue;
            reservation.InsuranceCost = costCalculatorHelper.GetInsuranceCost(viewModel.SelectedInsuranceLevelId);
            reservation.InsuranceExcess = costCalculatorHelper.GetInsuranceExcess(viewModel.SelectedInsuranceLevelId);
            reservation.TotalCost = costCalculatorHelper.GetTotalCost(reservation);

            _context.Reservations.Add(reservation);
            _context.SaveChanges();

            List<Data.Models.Database.Country> countries = _context.Countries.ToList();

            if (reservation.PaymentType == Data.Enums.PaymentTypes.CreditCard ||
                reservation.PaymentType == Data.Enums.PaymentTypes.MBReference ||
                reservation.PaymentType == Data.Enums.PaymentTypes.MBWay)
            {
                string paymentMethod = "";
                switch (reservation.PaymentType)
                {
                    case Data.Enums.PaymentTypes.CreditCard:
                        paymentMethod = "CC";
                        break;
                    case Data.Enums.PaymentTypes.MBReference:
                        paymentMethod = "MB";
                        break;
                    case Data.Enums.PaymentTypes.MBWay:
                        paymentMethod = "MBW";
                        break;
                }
                dCore.Business.Payment.Easypay.Providers.Payment paymentProvider = new();

                decimal paymentValue = viewModel.PaymentAmountType == Models.PaymentAmountType.Deposit
                    ? Math.Round(reservation.TotalCost * 0.20m, 2)
                    : reservation.TotalCost;
                reservation.HasAdvancePartialPayment = viewModel.PaymentAmountType == Models.PaymentAmountType.Deposit;
                reservation.AdvancePartialPaymentValue = reservation.HasAdvancePartialPayment ? paymentValue : null;
                reservation.AdvancePartialPaymentPaymentType = reservation.HasAdvancePartialPayment ? reservation.PaymentType : null;

                dCore.Business.Payment.Easypay.Models.Payment<dCore.Business.Payment.Easypay.Models.SinglePaymentCreate> paymentModel = new()
                {
                    AccountId = _configuration["Easypay:AccountId"],
                    ApiKey = _configuration["Easypay:ApiKey"],
                    ApiUrl = _configuration["Easypay:ApiUrl"],
                    RequestModel = new dCore.Business.Payment.Easypay.Models.SinglePaymentCreate()
                    {
                        Key = reservation.Number,
                        Value = paymentValue,
                        Method = paymentMethod,
                        Capture = new dCore.Business.Payment.Easypay.Models.SinglePaymentCreateCapture()
                        {
                            Descriptive = $"AMRent - Reserva {reservation.Number}",
                            TransactionKey = reservation.Number
                        },
                        Customer = new dCore.Business.Payment.Easypay.Models.SinglePaymentCreateCustomer()
                        {
                            Name = reservation.BillName,
                            Key = reservation.BillEmailAddress,
                            Email = reservation.BillEmailAddress,
                            FiscalNumber = !string.IsNullOrEmpty(reservation.BillVatNumber) ?
                                $"{countries.FirstOrDefault(x => x.Id == reservation.BillCountryId).Alpha2Code}{reservation.BillVatNumber}"
                                : "",
                            Phone = reservation.PaymentType == Data.Enums.PaymentTypes.MBWay ? viewModel.MbWayNumber : reservation.BillTelephone,
                            PhoneIndicative = $"+{countries.FirstOrDefault(x => x.Id == reservation.BillTelephonePrefixCountryId).TelephoneCode}"
                        }
                    }
                };
                string singlePaymentResultString = await paymentProvider.CreateSinglePayment(paymentModel);

                if (!string.IsNullOrEmpty(singlePaymentResultString))
                {
                    dCore.Business.Payment.Easypay.Models.SinglePaymentResult singlePaymentResult = JsonSerializer.Deserialize<dCore.Business.Payment.Easypay.Models.SinglePaymentResult>(singlePaymentResultString);

                    reservation.ExternalPaymentReference = singlePaymentResult.Id;
                    switch (reservation.PaymentType)
                    {
                        case Data.Enums.PaymentTypes.MBReference:
                            reservation.MultibancoEntity = singlePaymentResult.Method.MultibancoEntity;
                            reservation.MultibancoReference = singlePaymentResult.Method.MultibancoReference;
                            break;
                        case Data.Enums.PaymentTypes.CreditCard:
                            _context.SaveChanges();
                            return Redirect(singlePaymentResult.Method.CreditCardUrl);
                            break;
                    }
                }
                else
                {
                    reservation.PaymentStatus = Data.Enums.PaymentStatus.Failed;
                    reservation.PaymentType = Data.Enums.PaymentTypes.BankTransfer;
                }
            }

            reservation.LanguageId = selectedLanguageId;

            List<int> addedChangedConsentIds = new();
            Data.Models.Database.User user = null;
            if (User != null && User.Claims.Any())
            {
                Guid userId = Guid.Parse(User.Claims.First(x => x.Type == ClaimTypes.UserData)?.Value);

                reservation.CustomerId = userId;

                user = _context.Users
                    .Include(x => x.DataProtectionConsents)
                    .FirstOrDefault(x => x.Id == userId);

                foreach (var reservationDataProtectionConsent in reservation.DataProtectionConsents)
                {
                    if (!user.DataProtectionConsents.Any(x => x.DataProtectionConsentId == reservationDataProtectionConsent.DataProtectionConsentId))
                    {
                        user.DataProtectionConsents.Add(new()
                        {
                            DataProtectionConsentId = reservationDataProtectionConsent.DataProtectionConsentId,
                            HasConsented = reservationDataProtectionConsent.HasConsented
                        });
                        addedChangedConsentIds.Add(reservationDataProtectionConsent.DataProtectionConsentId);
                    }
                    else
                    {
                        var databaseDataProtectionConsent = user.DataProtectionConsents.FirstOrDefault(x => x.DataProtectionConsentId == reservationDataProtectionConsent.DataProtectionConsentId);
                        if (databaseDataProtectionConsent.HasConsented != reservationDataProtectionConsent.HasConsented)
                        {
                            databaseDataProtectionConsent.HasConsented = reservationDataProtectionConsent.HasConsented;
                            addedChangedConsentIds.Add(reservationDataProtectionConsent.DataProtectionConsentId);
                        }
                    }
                }
            }

            _context.SaveChanges();

            dCore.Communication.Models.SmtpConfiguration smtpConfiguration = _configuration.GetSection("SmtpConfiguration").Get<dCore.Communication.Models.SmtpConfiguration>();
            Shared.Providers.EmailSender emailSender = new Shared.Providers.EmailSender(_context, _translationProvider, smtpConfiguration, selectedLanguageId);
            string[] reservationConfirmationAdminDestinationAddresses = _configuration["Notifications:Email:ReservationConfirmationDestinationAddresses"].Split(',');
            if (addedChangedConsentIds.Any())
            {
                emailSender.Send(Data.Enums.InternalEmailContentTypes.DataProtectionConsentChanged, _configuration["Environment"] == "Test", reservationConfirmationAdminDestinationAddresses, userId: user.Id, changedDataProtectionConsentIds: addedChangedConsentIds.ToArray());
            }

            if (reservation.PaymentType == Data.Enums.PaymentTypes.Paypal)
            {
                var accessToken = await _payPalService.GetAccessTokenAsync();

                var order = await _payPalService.CreateOrderAsync(accessToken, reservation.TotalCost, reservation.Number);

                var approvalLink = order.RootElement
                    .GetProperty("links")
                    .EnumerateArray()
                    .FirstOrDefault(l => l.GetProperty("rel").GetString() == "approve")
                    .GetProperty("href").GetString();

                // Guarda o ID da ordem PayPal na BD, se quiseres validar depois
                reservation.ExternalPaymentReference = order.RootElement.GetProperty("id").GetString();
                reservation.PaymentStatus = Data.Enums.PaymentStatus.Pending;
                _context.SaveChanges();

                return Redirect(approvalLink);
            }

            emailSender.Send(Data.Enums.EmailContentTypes.ReservationRegistration, _configuration["Environment"] == "Test", reservation.Id, reservationConfirmationAdminDestinationAddresses);

            Models.BookingConfirmation confirmationViewModel = new()
            {
                PickupLocationName = _context.PickupReturnLocations
                    .Include(x => x.Translations)
                    .FirstOrDefault(x => x.Id == reservation.PickupLocationId)
                    .Translations.FirstOrDefault(x => x.LanguageId == selectedLanguageId).Name,
                PickupDateTime = reservation.PickupDateTime,
                ReturnLocationName = _context.PickupReturnLocations
                    .Include(x => x.Translations)
                    .FirstOrDefault(x => x.Id == reservation.PickupLocationId)
                    .Translations.FirstOrDefault(x => x.LanguageId == selectedLanguageId).Name,
                ReturnDateTime = reservation.ReturnDateTime,
                SegmentId = segment.Id,
                SegmentName = segment.Translations?.FirstOrDefault()?.Name,
                PaymentType = reservation.PaymentType.Value,
                SelectedExtras = extras,
                RentCost = reservation.CarSegmentCost,
                PickupCost = reservation.PickupCost,
                ReturnCost = reservation.ReturnCost,
                Insurances = insurances,
                Days = costCalculatorHelper.GetTotalDays(),
                BillName = reservation.BillName,
                BillEmail = reservation.BillEmailAddress,
                BillTelephone = $"+{reservation.BillTelephonePrefixCountry.TelephoneCode} {reservation.BillTelephone}",
                BillVatNumber = $"+{reservation.BillCountry.Alpha2Code} {reservation.BillVatNumber}",
                BookingNumber = reservation.Number,
                MultibancoEntity = reservation.MultibancoEntity,
                MultibancoReference = reservation.MultibancoReference,
                TotalValue = reservation.TotalCost,
                PickupReturnTemporaryTaxes = viewModel.PickupReturnTemporaryTaxes
            };

            BuildViewBag();

            return View(confirmationViewModel);
        }

        [HttpPost]
        [Filters.PassAlongQueryParamertersFilter()]
        public IActionResult ConfirmAskQuotation(Models.BookingAskQuotation viewModel)
        {
            TempData["ViewModel"] = JsonSerializer.Serialize(viewModel);
            return RedirectToAction(nameof(ConfirmAskQuotation));
        }

        [Filters.PassAlongQueryParamertersFilter()]
        public async Task<IActionResult> ConfirmAskQuotation()
        {
            int selectedLanguageId = GetSelectedLanguageId();

            Models.BookingAskQuotation viewModel = new();
            if (TempData.ContainsKey("ViewModel"))
            {
                viewModel = JsonSerializer.Deserialize<Models.BookingAskQuotation>(TempData["ViewModel"] as string);
                TempData.Remove("ViewModel");
            }

            Data.Models.Database.CarSegment segment = _context.CarSegments
                .Include(x => x.Translations.Where(t => t.LanguageId == selectedLanguageId))
                .Include(x => x.Insurances.Where(y => y.InsuranceLevelId == viewModel.SelectedInsuranceLevelId))
                    .ThenInclude(x => x.InsuranceLevel)
                        .ThenInclude(x => x.Translations.Where(t => t.LanguageId == selectedLanguageId))
                .FirstOrDefault(x => x.Id == viewModel.SegmentId && x.IsActive);

            if (segment == null)
            {
                return RedirectToAction(nameof(Index), "Search");
            }

            Shared.Providers.PickupReturnLocation pickupReturnLocationProvider = new Shared.Providers.PickupReturnLocation(_context);

            DateTime validatedPickupDateTime =
                pickupReturnLocationProvider.GetNextCompliantWithAnticipationDateTime(
                    viewModel.PickupLocationId,
                    viewModel.PickupDateTime);
            DateTime dateTimeToRequest = viewModel.ReturnDateTime;
            if (dateTimeToRequest <= validatedPickupDateTime.AddMinutes(15))
            {
                dateTimeToRequest = validatedPickupDateTime.AddMinutes(15);
            }
            DateTime validatedReturnDateTime =
                pickupReturnLocationProvider.GetNextAvailableDateTime(
                    viewModel.ReturnLocationId, dateTimeToRequest);

            if (dCore.Helpers.DateTime.GetWithMinutePrecision(viewModel.PickupDateTime) != dCore.Helpers.DateTime.GetWithMinutePrecision(validatedPickupDateTime)
                || dCore.Helpers.DateTime.GetWithMinutePrecision(viewModel.ReturnDateTime) != dCore.Helpers.DateTime.GetWithMinutePrecision(validatedReturnDateTime))
            {
                return RedirectToAction("Detail", "Segment", new Models.SegmentDetail()
                {
                    SegmentId = viewModel.SegmentId,
                    PickupLocationId = viewModel.PickupLocationId,
                    ReturnLocationId = viewModel.ReturnLocationId,
                    PickupDateTime = viewModel.PickupDateTime,
                    ReturnDateTime = viewModel.ReturnDateTime,
                    //SelectedExtraIds = viewModel.SelectedExtraIds,
                    SelectedInsuranceLevelId = viewModel.SelectedInsuranceLevelId
                });
            }

            Shared.Providers.CostCalculator costCalculatorHelper = new Shared.Providers.CostCalculator(_context, viewModel.PickupDateTime, viewModel.ReturnDateTime);
            costCalculatorHelper.SetSegment(viewModel.SegmentId);
            viewModel.Days = costCalculatorHelper.GetTotalDays();

            if (!string.IsNullOrEmpty(viewModel.VoucherCode))
            {
                viewModel.VoucherId = costCalculatorHelper.GetVoucherId(viewModel.VoucherCode, extraIds: viewModel.Extras.Where(x => x.Quantity > 0).Select(x => x.Id).ToArray());

                if (User != null && User.Claims.Any())
                {
                    Guid userId = Guid.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.UserData).Value);
                    if (_context.Users.FirstOrDefault(x => x.Id == userId).Reservations.Any(x => x.Voucher.Code == viewModel.VoucherCode))
                    {
                        viewModel.VoucherId = null;
                    }
                }

                if (viewModel.VoucherId.HasValue)
                {
                    viewModel.CampaignVoucherName = viewModel.VoucherCode;
                }
            }

            if (!viewModel.VoucherId.HasValue)
            {
                viewModel.CampaignId = costCalculatorHelper.GetCampaignId(extraIds: viewModel.Extras.Where(x => x.Quantity > 0).Select(x => x.Id).ToArray());
                if (viewModel.CampaignId.HasValue)
                {
                    viewModel.CampaignVoucherName = _context.Campaigns.Include(x => x.Translations).FirstOrDefault(x => x.Id == viewModel.CampaignId).Translations.FirstOrDefault(x => x.LanguageId == selectedLanguageId).Name;
                }
            }

            var rentValue = costCalculatorHelper.GetCarCost().Value;
            var pickupValue = costCalculatorHelper.GetPickupReturnCost(viewModel.PickupLocationId);
            var returnValue = costCalculatorHelper.GetPickupReturnCost(viewModel.ReturnLocationId);
            var insurances = _context.CarSegments.AsNoTracking().Include(x => x.Insurances).ThenInclude(x => x.InsuranceLevel).ThenInclude(x => x.Translations.Where(y => y.LanguageId == selectedLanguageId)).FirstOrDefault(x => x.Id == segment.Id).Insurances.Where(x => x.InsuranceLevelId == viewModel.SelectedInsuranceLevelId).ToList();
            foreach (var insurance in insurances)
            {
                insurance.Prices.Add(new() { Value = costCalculatorHelper.GetInsuranceCost(insurance.InsuranceLevelId) });
            }

            List<AMRent.Website.Models.SelectedExtra> extras = new();
            if (viewModel.Extras.Any())
            {
                extras = _context.Extras
                .Include(x => x.Translations)
                .Where(x => viewModel.Extras.Select(x => x.Id).Contains(x.Id))
                    .Select(x => new AMRent.Website.Models.SelectedExtra()
                    {
                        Id = x.Id,
                        Name = x.Translations.FirstOrDefault(x => x.LanguageId == selectedLanguageId).Name,
                        Type = x.ExtraType
                    }).ToList();

                foreach (KeyValuePair<int, decimal> extraCost in costCalculatorHelper.GetExtrasCost(extras.Select(x => x.Id).ToList(), viewModel.SelectedInsuranceLevelId))
                {
                    extras.FirstOrDefault(x => x.Id == extraCost.Key).Quantity = viewModel.Extras.First(x => x.Id == extraCost.Key).Quantity;
                    extras.FirstOrDefault(x => x.Id == extraCost.Key).DailyValue = extraCost.Value;
                    extras.FirstOrDefault(x => x.Id == extraCost.Key).Cost = extras.FirstOrDefault(x => x.Id == extraCost.Key).Quantity * extras.FirstOrDefault(x => x.Id == extraCost.Key).DailyValue;
                }
            }

            if (dCore.Helpers.DateTime.GetWithMinutePrecision(viewModel.PickupDateTime) != dCore.Helpers.DateTime.GetWithMinutePrecision(validatedPickupDateTime)
                || dCore.Helpers.DateTime.GetWithMinutePrecision(viewModel.ReturnDateTime) != dCore.Helpers.DateTime.GetWithMinutePrecision(validatedReturnDateTime)
                || viewModel.RentValue != rentValue || viewModel.PickupValue != pickupValue || viewModel.ReturnValue != returnValue)
            {
                return RedirectToAction("Detail", "Segment", new Models.SegmentDetail()
                {
                    SegmentId = viewModel.SegmentId,
                    PickupLocationId = viewModel.PickupLocationId,
                    ReturnLocationId = viewModel.ReturnLocationId,
                    PickupDateTime = viewModel.PickupDateTime,
                    ReturnDateTime = viewModel.ReturnDateTime,
                    //SelectedExtraIds = viewModel.SelectedExtraIds,
                    SelectedInsuranceLevelId = viewModel.SelectedInsuranceLevelId
                });
            }

            Data.Models.Database.QuotationItem quotationItem = new()
            {
                CarSegmentId = viewModel.SegmentId,
                InsuranceLevelId = viewModel.SelectedInsuranceLevelId,
                CampaignId = viewModel.CampaignId,
                VoucherId = viewModel.VoucherId,
                CarSegmentCost = rentValue,
                PickupCost = pickupValue,
                ReturnCost = returnValue,
                InsuranceCost = costCalculatorHelper.GetInsuranceCost(viewModel.SelectedInsuranceLevelId),
                InsuranceExcess = costCalculatorHelper.GetInsuranceExcess(viewModel.SelectedInsuranceLevelId),
            };

            foreach (Models.SelectedExtra selectedExtra in extras)
            {
                quotationItem.Extras.Add(new()
                {
                    ExtraId = selectedExtra.Id,
                    UnitValue = selectedExtra.DailyValue,
                    Quantity = selectedExtra.Quantity,
                });
            }

            foreach (KeyValuePair<int, Tuple<int, decimal>> pickupReturnTemporaryTax in costCalculatorHelper.GetPickupReturnTemporaryTaxes())
            {
                quotationItem.PickupReturnTemporaryTaxes.Add(new Data.Models.Database.QuotationItemPickupReturnTemporaryTax()
                {
                    PickupReturnTemporaryTaxId = pickupReturnTemporaryTax.Key,
                    Quantity = pickupReturnTemporaryTax.Value.Item1,
                    UnitValue = pickupReturnTemporaryTax.Value.Item2,
                });
            }
            quotationItem.TotalCost = costCalculatorHelper.GetTotalCost(quotationItem);

            Data.Models.Database.Quotation quotation = new()
            {
                Source = Data.Enums.ReservationQuotationSources.W,
                Status = Data.Enums.QuotationStatus.Requested,

                PickupLocationId = viewModel.PickupLocationId,
                CustomPickupLocationName = viewModel.CustomPickupLocationName,
                PickupDateTime = viewModel.PickupDateTime,
                ReturnLocationId = viewModel.ReturnLocationId,
                CustomReturnLocationName = viewModel.CustomReturnLocationName,
                ReturnDateTime = viewModel.ReturnDateTime,

                CustomerName = viewModel.DriverName,
                CustomerEmailAddress = viewModel.DriverEmail,
                CustomerTelephonePrefixCountryId = viewModel.DriverTelephoneCountryId,
                CustomerTelephone = viewModel.DriverTelephone,

                Comments = viewModel.Comments,
            };

            quotation.QuotationItems.Add(quotationItem);

            // Data

            foreach (Models.DataProtectionConsent dataProtectionConsent in viewModel.DataProtectionConsents)
            {
                quotation.DataProtectionConsents.Add(new()
                {
                    DataProtectionConsentId = dataProtectionConsent.Id,
                    HasConsented = dataProtectionConsent.HasConsent
                });
            }

            _context.Quotations.Add(quotation);
            _context.SaveChanges();

            //List<Data.Models.Database.Country> countries = _context.Countries.ToList();

            quotation.LanguageId = selectedLanguageId;

            List<int> addedChangedConsentIds = new();
            Data.Models.Database.User user = null;
            if (User != null && User.Claims.Any())
            {
                Guid userId = Guid.Parse(User.Claims.First(x => x.Type == ClaimTypes.UserData)?.Value);

                //quotation.CustomerId = userId;

                user = _context.Users
                    .Include(x => x.DataProtectionConsents)
                    .FirstOrDefault(x => x.Id == userId);

                foreach (var reservationDataProtectionConsent in quotation.DataProtectionConsents)
                {
                    if (!user.DataProtectionConsents.Any(x => x.DataProtectionConsentId == reservationDataProtectionConsent.DataProtectionConsentId))
                    {
                        user.DataProtectionConsents.Add(new()
                        {
                            DataProtectionConsentId = reservationDataProtectionConsent.DataProtectionConsentId,
                            HasConsented = reservationDataProtectionConsent.HasConsented
                        });
                        addedChangedConsentIds.Add(reservationDataProtectionConsent.DataProtectionConsentId);
                    }
                    else
                    {
                        var databaseDataProtectionConsent = user.DataProtectionConsents.FirstOrDefault(x => x.DataProtectionConsentId == reservationDataProtectionConsent.DataProtectionConsentId);
                        if (databaseDataProtectionConsent.HasConsented != reservationDataProtectionConsent.HasConsented)
                        {
                            databaseDataProtectionConsent.HasConsented = reservationDataProtectionConsent.HasConsented;
                            addedChangedConsentIds.Add(reservationDataProtectionConsent.DataProtectionConsentId);
                        }
                    }
                }
            }

            _context.SaveChanges();

            dCore.Communication.Models.SmtpConfiguration smtpConfiguration = _configuration.GetSection("SmtpConfiguration").Get<dCore.Communication.Models.SmtpConfiguration>();
            Shared.Providers.EmailSender emailSender = new Shared.Providers.EmailSender(_context, _translationProvider, smtpConfiguration, selectedLanguageId);
            string[] reservationConfirmationAdminDestinationAddresses = _configuration["Notifications:Email:ReservationConfirmationDestinationAddresses"].Split(',');
            if (addedChangedConsentIds.Any())
            {
                emailSender.Send(Data.Enums.InternalEmailContentTypes.DataProtectionConsentChanged, _configuration["Environment"] == "Test", reservationConfirmationAdminDestinationAddresses, userId: user.Id, changedDataProtectionConsentIds: addedChangedConsentIds.ToArray());
            }

            emailSender.Send(Data.Enums.EmailContentTypes.QuotationRegistration, _configuration["Environment"] == "Test", quotation.Id, reservationConfirmationAdminDestinationAddresses);

            Models.BookingAskQuotationConfirmation confirmationViewModel = new()
            {
                PickupLocationName = $"{_context.PickupReturnLocations
                    .Include(x => x.Translations)
                    .FirstOrDefault(x => x.Id == quotation.PickupLocationId)
                    .Translations.FirstOrDefault(x => x.LanguageId == selectedLanguageId).Name} {viewModel.CustomPickupLocationName}",
                PickupDateTime = quotation.PickupDateTime,
                ReturnLocationName = $"{_context.PickupReturnLocations
                    .Include(x => x.Translations)
                    .FirstOrDefault(x => x.Id == quotation.PickupLocationId)
                    .Translations.FirstOrDefault(x => x.LanguageId == selectedLanguageId).Name} {viewModel.CustomReturnLocationName}",
                ReturnDateTime = quotation.ReturnDateTime,
                SegmentId = segment.Id,
                SegmentName = segment.Translations?.FirstOrDefault()?.Name,
                SelectedExtras = extras,
                RentCost = quotation.QuotationItems.First().CarSegmentCost,
                Insurances = insurances,
                Days = costCalculatorHelper.GetTotalDays(),
                TotalValue = quotation.QuotationItems.First().TotalCost,
                QuotationNumber = quotation.Number
            };

            BuildViewBag();

            return View(confirmationViewModel);
        }

        public async Task<IActionResult> PaypalCapture(string token) // token == orderId
        {
            var accessToken = await _payPalService.GetAccessTokenAsync();
            var result = await _payPalService.CaptureOrderAsync(token, accessToken);

            var status = result.RootElement.GetProperty("status").GetString();
            var referenceId = result.RootElement
                .GetProperty("purchase_units")[0]
                .GetProperty("reference_id").GetString();

            var reservation = _context.Reservations
                .Include(x => x.BillTelephonePrefixCountry)
                .Include(x => x.BillCountry)
                .FirstOrDefault(r => r.Number == referenceId);

            if (reservation == null)
            {
                return RedirectToAction("Error", "Home");
            }

            if (status == "COMPLETED")
            {
                reservation.PaymentStatus = Data.Enums.PaymentStatus.Paid;
                _context.SaveChanges();

                dCore.Communication.Models.SmtpConfiguration smtpConfiguration = _configuration.GetSection("SmtpConfiguration").Get<dCore.Communication.Models.SmtpConfiguration>();
                Shared.Providers.EmailSender emailSender = new Shared.Providers.EmailSender(_context, _translationProvider, smtpConfiguration, reservation.LanguageId);
                string[] reservationConfirmationAdminDestinationAddresses = _configuration["Notifications:Email:ReservationConfirmationDestinationAddresses"].Split(',');

                emailSender.Send(Data.Enums.EmailContentTypes.ReservationRegistration, _configuration["Environment"] == "Test", reservation.Id, reservationConfirmationAdminDestinationAddresses);

                Data.Models.Database.CarSegment segment = _context.CarSegments
                    .Include(x => x.Translations.Where(t => t.LanguageId == reservation.LanguageId))
                    .Include(x => x.Insurances.Where(y => y.InsuranceLevelId == reservation.InsuranceLevelId))
                        .ThenInclude(x => x.InsuranceLevel)
                            .ThenInclude(x => x.Translations.Where(t => t.LanguageId == reservation.LanguageId))
                    .FirstOrDefault(x => x.Id == reservation.CarSegmentId && x.IsActive);

                Shared.Providers.CostCalculator costCalculatorHelper = new Shared.Providers.CostCalculator(_context, reservation.PickupDateTime, reservation.ReturnDateTime);
                costCalculatorHelper.SetSegment(reservation.CarSegmentId);

                List<AMRent.Website.Models.SelectedExtra> extras = _context.ReservationExtras.
                    Include(x => x.Extra)
                        .ThenInclude(x => x.Translations.Where(y => y.LanguageId == reservation.LanguageId))
                    .Where(x => x.ReservationId == reservation.Id)
                    .Select(x => new AMRent.Website.Models.SelectedExtra()
                    {
                        Id = x.ExtraId,
                        Name = x.Extra.Translations.FirstOrDefault(y => y.LanguageId == reservation.LanguageId).Name,
                        Type = x.Extra.ExtraType,
                        Quantity = x.Quantity,
                        DailyValue = x.UnitValue,
                        Cost = x.Quantity * x.UnitValue,
                    }).ToList();

                var insurances = _context.CarSegments.AsNoTracking().Include(x => x.Insurances).ThenInclude(x => x.InsuranceLevel).ThenInclude(x => x.Translations.Where(y => y.LanguageId == reservation.LanguageId)).FirstOrDefault(x => x.Id == segment.Id).Insurances.Where(x => x.InsuranceLevelId == reservation.InsuranceLevelId).ToList();
                foreach (var insurance in insurances)
                {
                    insurance.Prices.Add(new() { Value = reservation.InsuranceCost });
                }

                Models.BookingConfirmation confirmationViewModel = new()
                {
                    PickupLocationName = _context.PickupReturnLocations
                    .Include(x => x.Translations)
                    .FirstOrDefault(x => x.Id == reservation.PickupLocationId)
                    .Translations.FirstOrDefault(x => x.LanguageId == reservation.LanguageId).Name,
                    PickupDateTime = reservation.PickupDateTime,
                    ReturnLocationName = _context.PickupReturnLocations
                    .Include(x => x.Translations)
                    .FirstOrDefault(x => x.Id == reservation.PickupLocationId)
                    .Translations.FirstOrDefault(x => x.LanguageId == reservation.LanguageId).Name,
                    ReturnDateTime = reservation.ReturnDateTime,
                    SegmentId = segment.Id,
                    SegmentName = segment.Translations?.FirstOrDefault()?.Name,
                    PaymentType = reservation.PaymentType.Value,
                    SelectedExtras = extras,
                    RentCost = reservation.CarSegmentCost,
                    PickupCost = reservation.PickupCost,
                    ReturnCost = reservation.ReturnCost,
                    Insurances = insurances,
                    Days = costCalculatorHelper.GetTotalDays(),
                    BillName = reservation.BillName,
                    BillEmail = reservation.BillEmailAddress,
                    BillTelephone = $"+{reservation.BillTelephonePrefixCountry.TelephoneCode} {reservation.BillTelephone}",
                    BillVatNumber = $"+{reservation.BillCountry.Alpha2Code} {reservation.BillVatNumber}",
                    BookingNumber = reservation.Number,
                    MultibancoEntity = reservation.MultibancoEntity,
                    MultibancoReference = reservation.MultibancoReference,
                };

                BuildViewBag();

                return View("Confirm", confirmationViewModel);
            }
            else
            {
                reservation.PaymentStatus = Data.Enums.PaymentStatus.Failed;
                _context.SaveChanges();
                return RedirectToAction("Error", "Payments", new { id = reservation.Id });
            }
        }

        public async Task<IActionResult> CreditCardConfirm(string reservationNumber)
        {
            int selectedLanguageId = GetSelectedLanguageId();

            Data.Models.Database.Reservation reservation = _context.Reservations
                .Include(x => x.Extras)
                .ThenInclude(x => x.Extra)
                .ThenInclude(x => x.Translations.Where(y => y.LanguageId == selectedLanguageId))
                .Include(x => x.InsuranceLevel)
                .ThenInclude(x => x.Translations.Where(y => y.LanguageId == selectedLanguageId))
                .Include(x => x.PickupLocation)
                .ThenInclude(x => x.Translations.Where(y => y.LanguageId == selectedLanguageId))
                .Include(x => x.ReturnLocation)
                .ThenInclude(x => x.Translations.Where(y => y.LanguageId == selectedLanguageId))
                .Include(x => x.CarSegment)
                .ThenInclude(x => x.Translations.Where(y => y.LanguageId == selectedLanguageId))
                .Include(x => x.BillTelephonePrefixCountry)
                .Include(x => x.BillCountry)
                .FirstOrDefault(x => x.Number == reservationNumber);

            Shared.Providers.CostCalculator costCalculatorHelper = new Shared.Providers.CostCalculator(_context, reservation.PickupDateTime, reservation.ReturnDateTime);
            List<Models.SelectedExtra> extras = new();
            foreach (Data.Models.Database.ReservationExtra reservationExtra in reservation.Extras)
            {
                extras.Add(new()
                {
                    Id = reservationExtra.ExtraId,
                    Name = reservationExtra.Extra.Translations.First().Name,
                    Cost = reservationExtra.UnitValue,
                    Type = reservationExtra.Extra.ExtraType
                });
            }
            List<Data.Models.Database.Insurance> insurances = new();
            insurances.Add(new()
            {
                InsuranceLevelId = reservation.InsuranceLevelId,
                Prices = new() { new() { Value = reservation.InsuranceCost } },
                Excess = reservation.InsuranceExcess,
                InsuranceLevel = new()
                {
                    Translations = new()
                    {
                        new()
                        {
                            Name = reservation.InsuranceLevel.Translations.First().Name,
                        }
                    }
                }
            });

            Models.BookingConfirmation confirmationViewModel = new()
            {
                PickupLocationName = reservation.PickupLocation.Translations.FirstOrDefault(x => x.LanguageId == selectedLanguageId).Name,
                PickupDateTime = reservation.PickupDateTime,
                ReturnLocationName = reservation.ReturnLocation.Translations.FirstOrDefault(x => x.LanguageId == selectedLanguageId).Name,
                ReturnDateTime = reservation.ReturnDateTime,
                SegmentId = reservation.CarSegmentId,
                SegmentName = reservation.CarSegment.Translations?.FirstOrDefault()?.Name,
                PaymentType = reservation.PaymentType.Value,
                SelectedExtras = extras,
                RentCost = reservation.CarSegmentCost,
                PickupCost = reservation.PickupCost,
                ReturnCost = reservation.ReturnCost,
                Insurances = insurances,
                Days = costCalculatorHelper.GetTotalDays(),
                BillName = reservation.BillName,
                BillEmail = reservation.BillEmailAddress,
                BillTelephone = $"+{reservation.BillTelephonePrefixCountry.TelephoneCode} {reservation.BillTelephone}",
                BillVatNumber = $"+{reservation.BillCountry.Alpha2Code} {reservation.BillVatNumber}",
                BookingNumber = reservation.Number,
                TotalValue = reservation.TotalCostOverride ?? reservation.TotalCost,
            };

            BuildViewBag();

            return View("Confirm", confirmationViewModel);
        }

        public async Task<IActionResult> Payment(string reservationNumber)
        {
            Data.Models.Database.Reservation reservation = _context.Reservations.FirstOrDefault(x => x.Number == reservationNumber);

            if (reservation == null || reservation.PaymentStatus == Data.Enums.PaymentStatus.Paid)
            {
                return RedirectToAction("Index", "Home");
            }

            Models.BookingPayment paymentViewModel = new()
            {
                ReservationId = reservation.Id,
                ReservationNumber = reservation.Number,
                TotalCost = reservation.TotalCostOverride ?? reservation.TotalCost,
                PaymentType = reservation.PaymentType,
                PaymentAmountType = reservation.HasAdvancePartialPayment ? Models.PaymentAmountType.Deposit : Models.PaymentAmountType.Total
            };

            BuildViewBag();

            return View(paymentViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Payment(Models.BookingPayment viewModel)
        {
            int selectedLanguageId = GetSelectedLanguageId();
            Data.Models.Database.Reservation reservation = _context.Reservations
                .Include(x => x.Extras)
                .ThenInclude(x => x.Extra)
                .ThenInclude(x => x.Translations.Where(y => y.LanguageId == selectedLanguageId))
                .Include(x => x.InsuranceLevel)
                .ThenInclude(x => x.Translations.Where(y => y.LanguageId == selectedLanguageId))
                .Include(x => x.PickupLocation)
                .ThenInclude(x => x.Translations.Where(y => y.LanguageId == selectedLanguageId))
                .Include(x => x.ReturnLocation)
                .ThenInclude(x => x.Translations.Where(y => y.LanguageId == selectedLanguageId))
                .Include(x => x.CarSegment)
                .ThenInclude(x => x.Translations.Where(y => y.LanguageId == selectedLanguageId))
                .Include(x => x.BillTelephonePrefixCountry)
                .Include(x => x.BillCountry)
                .FirstOrDefault(x => x.Number == viewModel.ReservationNumber);
            reservation.PaymentType = viewModel.PaymentType;

            Shared.Providers.CostCalculator costCalculatorHelper = new Shared.Providers.CostCalculator(_context, reservation.PickupDateTime, reservation.ReturnDateTime);
            List<Models.SelectedExtra> extras = new();
            foreach (Data.Models.Database.ReservationExtra reservationExtra in reservation.Extras)
            {
                extras.Add(new()
                {
                    Id = reservationExtra.ExtraId,
                    Name = reservationExtra.Extra.Translations.First().Name,
                    Cost = reservationExtra.UnitValue,
                    Type = reservationExtra.Extra.ExtraType
                });
            }
            List<Data.Models.Database.Insurance> insurances = new();
            insurances.Add(new()
            {
                InsuranceLevelId = reservation.InsuranceLevelId,
                Prices = new() { new() { Value = reservation.InsuranceCost } },
                Excess = reservation.InsuranceExcess,
                InsuranceLevel = new()
                {
                    Translations = new()
                    {
                        new()
                        {
                            Name = reservation.InsuranceLevel.Translations.First().Name,
                        }
                    }
                }
            });

            _context.SaveChanges();

            List<Data.Models.Database.Country> countries = _context.Countries.ToList();

            if (reservation.PaymentType == Data.Enums.PaymentTypes.CreditCard ||
                reservation.PaymentType == Data.Enums.PaymentTypes.MBReference ||
                reservation.PaymentType == Data.Enums.PaymentTypes.MBWay)
            {
                string paymentMethod = "";
                switch (reservation.PaymentType)
                {
                    case Data.Enums.PaymentTypes.CreditCard:
                        paymentMethod = "CC";
                        break;
                    case Data.Enums.PaymentTypes.MBReference:
                        paymentMethod = "MB";
                        break;
                    case Data.Enums.PaymentTypes.MBWay:
                        paymentMethod = "MBW";
                        break;
                }
                dCore.Business.Payment.Easypay.Providers.Payment paymentProvider = new();

                decimal paymentValue = viewModel.PaymentAmountType == Models.PaymentAmountType.Deposit
                    ? Math.Round((reservation.TotalCostOverride ?? reservation.TotalCost) * 0.20m, 2)
                    : reservation.TotalCostOverride ?? reservation.TotalCost;
                reservation.HasAdvancePartialPayment = viewModel.PaymentAmountType == Models.PaymentAmountType.Deposit;
                reservation.AdvancePartialPaymentValue = reservation.HasAdvancePartialPayment ? paymentValue : null;
                reservation.AdvancePartialPaymentPaymentType = reservation.HasAdvancePartialPayment ? reservation.PaymentType : null;

                dCore.Business.Payment.Easypay.Models.Payment<dCore.Business.Payment.Easypay.Models.SinglePaymentCreate> paymentModel = new()
                {
                    AccountId = _configuration["Easypay:AccountId"],
                    ApiKey = _configuration["Easypay:ApiKey"],
                    ApiUrl = _configuration["Easypay:ApiUrl"],
                    RequestModel = new dCore.Business.Payment.Easypay.Models.SinglePaymentCreate()
                    {
                        Key = reservation.Number,
                        Value = paymentValue,
                        Method = paymentMethod,
                        Capture = new dCore.Business.Payment.Easypay.Models.SinglePaymentCreateCapture()
                        {
                            Descriptive = $"AMRent - Reserva {reservation.Number}",
                            TransactionKey = reservation.Number
                        },
                        Customer = new dCore.Business.Payment.Easypay.Models.SinglePaymentCreateCustomer()
                        {
                            Name = reservation.BillName,
                            Key = reservation.BillEmailAddress,
                            Email = reservation.BillEmailAddress,
                            FiscalNumber = !string.IsNullOrEmpty(reservation.BillVatNumber) ?
                                $"{countries.FirstOrDefault(x => x.Id == reservation.BillCountryId).Alpha2Code}{reservation.BillVatNumber}"
                                : "",
                            Phone = reservation.PaymentType == Data.Enums.PaymentTypes.MBWay ? viewModel.MbWayNumber : reservation.BillTelephone,
                            PhoneIndicative = $"+{countries.FirstOrDefault(x => x.Id == reservation.BillTelephonePrefixCountryId).TelephoneCode}"
                        }
                    }
                };
                string singlePaymentResultString = await paymentProvider.CreateSinglePayment(paymentModel);
                _logger.LogDebug(singlePaymentResultString);

                if (!string.IsNullOrEmpty(singlePaymentResultString))
                {
                    dCore.Business.Payment.Easypay.Models.SinglePaymentResult singlePaymentResult = JsonSerializer.Deserialize<dCore.Business.Payment.Easypay.Models.SinglePaymentResult>(singlePaymentResultString);

                    reservation.ExternalPaymentReference = singlePaymentResult.Id;
                    switch (reservation.PaymentType)
                    {
                        case Data.Enums.PaymentTypes.MBReference:
                            reservation.MultibancoEntity = singlePaymentResult.Method.MultibancoEntity;
                            reservation.MultibancoReference = singlePaymentResult.Method.MultibancoReference;
                            break;
                        case Data.Enums.PaymentTypes.CreditCard:
                            _context.SaveChanges();
                            return Redirect(singlePaymentResult.Method.CreditCardUrl);
                            break;
                    }
                }
                else
                {
                    reservation.PaymentStatus = Data.Enums.PaymentStatus.Failed;
                    reservation.PaymentType = Data.Enums.PaymentTypes.BankTransfer;
                }
            }

            reservation.LanguageId = selectedLanguageId;

            _context.SaveChanges();

            dCore.Communication.Models.SmtpConfiguration smtpConfiguration = _configuration.GetSection("SmtpConfiguration").Get<dCore.Communication.Models.SmtpConfiguration>();
            Shared.Providers.EmailSender emailSender = new Shared.Providers.EmailSender(_context, _translationProvider, smtpConfiguration, selectedLanguageId);
            string[] reservationConfirmationAdminDestinationAddresses = _configuration["Notifications:Email:ReservationConfirmationDestinationAddresses"].Split(',');
            emailSender.Send(Data.Enums.EmailContentTypes.PaymentChoiceReservationFromQuotation, _configuration["Environment"] == "Test", reservation.Id, reservationConfirmationAdminDestinationAddresses);

            Models.BookingConfirmation confirmationViewModel = new()
            {
                PickupLocationName = _context.PickupReturnLocations
                    .Include(x => x.Translations)
                    .FirstOrDefault(x => x.Id == reservation.PickupLocationId)?
                    .Translations?.FirstOrDefault(x => x.LanguageId == selectedLanguageId)?.Name ?? "",
                PickupDateTime = reservation.PickupDateTime,
                ReturnLocationName = _context.PickupReturnLocations
                    .Include(x => x.Translations)
                    .FirstOrDefault(x => x.Id == reservation.PickupLocationId)?
                    .Translations?.FirstOrDefault(x => x.LanguageId == selectedLanguageId)?.Name ?? "",
                ReturnDateTime = reservation.ReturnDateTime,
                SegmentId = reservation.CarSegmentId,
                SegmentName = reservation.CarSegment?.Translations?.FirstOrDefault()?.Name,
                PaymentType = reservation.PaymentType.Value,
                SelectedExtras = extras,
                RentCost = reservation.CarSegmentCost,
                PickupCost = reservation.PickupCost,
                ReturnCost = reservation.ReturnCost,
                Insurances = insurances,
                Days = costCalculatorHelper.GetTotalDays(),
                BillName = reservation.BillName ?? "",
                BillEmail = reservation.BillEmailAddress ?? "",
                BillTelephone = $"+{reservation.BillTelephonePrefixCountry?.TelephoneCode} {reservation.BillTelephone}",
                BillVatNumber = $"+{reservation.BillCountry?.Alpha2Code} {reservation.BillVatNumber}",
                BookingNumber = reservation.Number,
                MultibancoEntity = reservation.MultibancoEntity,
                MultibancoReference = reservation.MultibancoReference,
                TotalValue = reservation.TotalCostOverride ?? reservation.TotalCost
            };

            BuildViewBag();

            return View("Confirm", confirmationViewModel);
        }
    }
}