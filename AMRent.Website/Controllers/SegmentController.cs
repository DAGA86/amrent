using AMRent.Data.Contexts;
using AMRent.Data.Models.Database;
using AMRent.Shared.Providers;
using AMRent.Website.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

namespace AMRent.Website.Controllers
{
    public class SegmentController : BaseController
    {
        public SegmentController(ILogger<HomeController> logger, FullDatabaseContext context, dCore.MultiLanguage.Providers.TranslationProvider translationProvider) : base(logger, context, translationProvider)
        {

        }

        private Models.SegmentDetail EnsureViewModelHasDefaults(Models.SegmentDetail inputModel)
        {
            if (inputModel.PickupLocationId == 0 | inputModel.ReturnLocationId == 0)
            {
                int? defaultPickupReturnLocationId = _context.PickupReturnLocations.FirstOrDefault(x => x.IsSelectedByDefault)?.Id;
                if (!defaultPickupReturnLocationId.HasValue)
                    throw new ApplicationException("Sem definição de localização de recolha / devolução por defeito.");

                if (inputModel.PickupLocationId == 0)
                    inputModel.PickupLocationId = defaultPickupReturnLocationId.Value;
                if (inputModel.ReturnLocationId == 0)
                    inputModel.ReturnLocationId = defaultPickupReturnLocationId.Value;

                Shared.Providers.PickupReturnLocation pickupReturnLocationProvider = new Shared.Providers.PickupReturnLocation(_context);
                inputModel.PickupDateTime = pickupReturnLocationProvider.GetNextCompliantWithAnticipationDateTime(
                    inputModel.PickupLocationId, dCore.Helpers.DateTime.RoundToNearestFutureQuarterHour(DateTime.Now));
                inputModel.ReturnDateTime = pickupReturnLocationProvider.GetNextAvailableDateTime(
                    inputModel.PickupLocationId, dCore.Helpers.DateTime.RoundToNearestFutureQuarterHour(inputModel.PickupDateTime.AddDays(1)));
            }

            return inputModel;
        }

        [Filters.PassAlongQueryParamertersFilter()]
        [HttpGet("Detail/{segmentId}")]
        public async Task<IActionResult> Detail(int segmentId)
        {
            Models.SegmentDetail viewModel = new();
            if (TempData.ContainsKey("ViewModel"))
            {
                viewModel = JsonSerializer.Deserialize<Models.SegmentDetail>(TempData["ViewModel"] as string);
                TempData.Remove("ViewModel");
            }
            viewModel.SegmentId = segmentId;
            TempData["ViewModel"] = JsonSerializer.Serialize(viewModel);
            return await Detail();
        }

        [HttpPost]
        [Filters.PassAlongQueryParamertersFilter()]
        public IActionResult Detail(Models.SegmentDetail viewModel)
        {
            TempData["ViewModel"] = JsonSerializer.Serialize(viewModel);
            return RedirectToRoute(new
            {
                controller = "Segment",
                action = "Detail",
                segmentId = viewModel.SegmentId
            });
        }

        [Filters.PassAlongQueryParamertersFilter()]
        public async Task<IActionResult> Detail()
        {
            int selectedLanguageId = GetSelectedLanguageId();

            Models.SegmentDetail viewModel = new();
            if (TempData.ContainsKey("ViewModel"))
            {
                viewModel = JsonSerializer.Deserialize<Models.SegmentDetail>(TempData["ViewModel"] as string);
                TempData.Remove("ViewModel");
            }
            viewModel = EnsureViewModelHasDefaults(viewModel);

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
                viewModel.PickupReturnValuesAdjusted = true;
            }

            viewModel.PickupDateTime = validatedPickupDateTime;
            viewModel.ReturnDateTime = validatedReturnDateTime;

            Data.Models.Database.CarSegment segment = _context.CarSegments
                .Include(x => x.CarCategory)
                .Include(x => x.Translations.Where(t => t.LanguageId == selectedLanguageId))
                .Include(x => x.CarGearbox)
                    .ThenInclude(x => x.Translations.Where(t => t.LanguageId == selectedLanguageId))
                .Include(x => x.CarFuel)
                    .ThenInclude(x => x.Translations.Where(t => t.LanguageId == selectedLanguageId))
                .Include(x => x.Insurances)
                    .ThenInclude(x => x.InsuranceLevel)
                    .ThenInclude(x => x.Translations.Where(t => t.LanguageId == selectedLanguageId))
                .FirstOrDefault(x => x.Id == viewModel.SegmentId && x.IsActive);

            if (segment == null)
            {
                return RedirectToAction(nameof(Index), "Search");
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
                    viewModel.CampaignVoucherName = _context.Campaigns
                                                        .Include(x => x.Translations.Where(y => y.LanguageId == GetSelectedLanguageId()))
                                                        .FirstOrDefault(x => x.Id == viewModel.CampaignId)
                                                        .Translations.First().Name;
                }
            }
            viewModel.RentValue = costCalculatorHelper.GetCarCost();
            viewModel.PickupValue = costCalculatorHelper.GetPickupReturnCost(viewModel.PickupLocationId);
            viewModel.ReturnValue = costCalculatorHelper.GetPickupReturnCost(viewModel.ReturnLocationId);

            foreach (var insurance in _context.CarSegments.AsNoTracking()
                .Include(x => x.Insurances)
                    .ThenInclude(x => x.InsuranceLevel)
                        .ThenInclude(x => x.Translations.Where(y => y.LanguageId == GetSelectedLanguageId()))
                .FirstOrDefault(x => x.Id == segment.Id).Insurances.ToList())
            {
                insurance.Prices.Add(new() { Value = costCalculatorHelper.GetInsuranceCost(insurance.InsuranceLevelId)});
                viewModel.Insurances.Add(insurance);
            }

            List<AMRent.Website.Models.Extra> extras = _context.Extras
                .Where(x => x.IsActive)
                .Select(x => new AMRent.Website.Models.Extra()
                {
                    Id = x.Id,
                    DailyValue = x.ExtraPricesByInsuranceLevel.FirstOrDefault(y => y.InsuranceLevelId == viewModel.SelectedInsuranceLevelId).Value,
                    MaximumValue = x.ExtraPricesByInsuranceLevel.FirstOrDefault(y => y.InsuranceLevelId == viewModel.SelectedInsuranceLevelId).MaximumValue,
                    Name = x.Translations.FirstOrDefault(y => y.LanguageId == GetSelectedLanguageId()).Name,
                    AllowMultiple = x.AllowMultiple
                }).ToList();
            
            foreach (KeyValuePair<int, decimal> extraCost in costCalculatorHelper.GetExtrasCost(extras.Select(x => x.Id).ToList(), viewModel.SelectedInsuranceLevelId))
            {
                extras.FirstOrDefault(x => x.Id == extraCost.Key).Cost = extraCost.Value * viewModel.Extras.FirstOrDefault(x => x.Id == extraCost.Key)?.Quantity ?? 0;
                extras.FirstOrDefault(x => x.Id == extraCost.Key).Quantity = viewModel.Extras.FirstOrDefault(x => x.Id == extraCost.Key)?.Quantity ?? 0;
            }
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

            viewModel.TotalValue = costCalculatorHelper.GetTotalCost(viewModel.RentValue.Value,
                                                                        viewModel.PickupValue.Value,
                                                                        viewModel.ReturnValue.Value,
                                                                        viewModel.Insurances.FirstOrDefault(x => x.InsuranceLevelId == viewModel.SelectedInsuranceLevelId).Prices.FirstOrDefault().Value,
                                                                        extras.Sum(x => x.Cost.Value),
                                                                        viewModel.PickupReturnTemporaryTaxes.Sum(x => x.Cost),
                                                                        0,
                                                                        viewModel.VoucherId,
                                                                        viewModel.CampaignId,
                                                                        extraDictionary);

            viewModel.Name = segment.Translations?.FirstOrDefault()?.Name;
            viewModel.Gearbox = segment.CarGearbox.Translations?.FirstOrDefault()?.Name;
            viewModel.Fuel = segment.CarFuel.Translations?.FirstOrDefault()?.Name;
            viewModel.Seats = segment.Seats;
            viewModel.IsCommercial = segment.CarCategory.IsCommercial;
            viewModel.LoadingSpaceWidthInMilimeters = segment.LoadingSpaceWidthInMilimeters;
            viewModel.LoadingSpaceLengthInMilimeters = segment.LoadingSpaceLengthInMilimeters;
            viewModel.LoadingSpaceHeightInMilimeters = segment.LoadingSpaceHeightInMilimeters;

            viewModel.Extras = extras;

            List<Data.Models.Database.PickupReturnLocation> pickupReturnLocations = _context.PickupReturnLocations
                .Include(x => x.Translations.Where(t => t.LanguageId == GetSelectedLanguageId()))
                .Where(x => x.Id > 0)
                .OrderBy(x => x.Translations.FirstOrDefault(t => t.LanguageId == GetSelectedLanguageId()).Name)
                .ToList();
            pickupReturnLocations.Add(_context.PickupReturnLocations
                .Include(x => x.Translations.Where(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese))
                .FirstOrDefault(x => x.Id == -1));

            ViewBag.PickupLocations = new SelectList(pickupReturnLocations
                .Select(x => new
                {
                    x.Id,
                    x.Translations.First().Name
                }), "Id", "Name", viewModel.PickupLocationId);

            ViewBag.ReturnLocations = new SelectList(pickupReturnLocations
                .Select(x => new
                {
                    x.Id,
                    x.Translations.First().Name
                }), "Id", "Name", viewModel.ReturnLocationId);

            BuildViewBag();

            return View(viewModel);
        }
    }
}