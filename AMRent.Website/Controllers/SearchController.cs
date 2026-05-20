using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AMRent.Data.Contexts;
using AMRent.Data.Models.Database;

namespace AMRent.Website.Controllers
{
    public class SearchController : BaseController
    {
        public SearchController(ILogger<HomeController> logger, FullDatabaseContext context, dCore.MultiLanguage.Providers.TranslationProvider translationProvider) : base(logger, context, translationProvider)
        {
        }

        private Models.SearchIndex EnsureViewModelHasDefaults(Models.SearchIndex inputModel)
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
        public IActionResult Passengers()
        {
            Models.SearchIndex viewModel = new();
            viewModel.SelectedCategoryIds = _context.CarCategories.Where(x => !x.IsCommercial).Select(x => x.Id).ToList();
            
            TempData["ViewModel"] = JsonSerializer.Serialize(viewModel);
            return RedirectToAction(nameof(Index));
        }

        [Filters.PassAlongQueryParamertersFilter()]
        public IActionResult Commercials()
        {
            Models.SearchIndex viewModel = new();
            viewModel.SelectedCategoryIds = _context.CarCategories.Where(x => x.IsCommercial).Select(x => x.Id).ToList();
            
            TempData["ViewModel"] = JsonSerializer.Serialize(viewModel);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Filters.PassAlongQueryParamertersFilter()]
        public IActionResult Index(Models.SearchIndex viewModel)
        {
            TempData["ViewModel"] = JsonSerializer.Serialize(viewModel);

            return RedirectToAction(nameof(Index));
        }

        [Filters.PassAlongQueryParamertersFilter()]
        public async Task<IActionResult> Index()
        {
            Models.SearchIndex viewModel = new();
            if (TempData.ContainsKey("ViewModel"))
            {
                viewModel = JsonSerializer.Deserialize<Models.SearchIndex>(TempData["ViewModel"] as string);
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

            IQueryable<Data.Models.Database.CarSegment> carSegments = _context.CarSegments
                .Include(x => x.CarCategory)
                .Include(x => x.Translations.Where(t => t.LanguageId == GetSelectedLanguageId()))
                .Where(x => x.IsActive);

            if (viewModel.SelectedCategoryIds.Any())
            {
                carSegments = carSegments.Where(x => viewModel.SelectedCategoryIds.Contains(x.CarCategoryId));
            }
            if (viewModel.SelectedFuelIds.Any())
            {
                carSegments = carSegments.Where(x => viewModel.SelectedFuelIds.Contains(x.CarFuelId));
            }
            if (viewModel.SelectedGearboxIds.Any())
            {
                carSegments = carSegments.Where(x => viewModel.SelectedGearboxIds.Contains(x.CarGearboxId));
            }
            if (viewModel.SelectedSeatNumbers.Any())
            {
                carSegments = carSegments.Where(x => viewModel.SelectedSeatNumbers.Contains(x.Seats));
            }
            if (viewModel.SelectedCampaignIds.Any())
            {
                carSegments = carSegments.Where(x => x.Campaigns.Any(y => viewModel.SelectedCampaignIds.Contains(y.Id)));
            }

            viewModel.CarSegments = await carSegments.ToListAsync();

            Shared.Providers.CostCalculator priceHelper = new Shared.Providers.CostCalculator(_context, viewModel.PickupDateTime, viewModel.ReturnDateTime);
            foreach (CarSegment carSegment in viewModel.CarSegments.ToArray())
            {
                priceHelper.SetSegment(carSegment.Id);
                int? campaignId = priceHelper.GetCampaignId(Array.Empty<int>(), isBackofficeRequest:false);

                decimal? carSegmentPrice = priceHelper.GetTotalCost(
                    priceHelper.GetCarCost().Value,
                    priceHelper.GetPickupReturnCost(viewModel.PickupLocationId),
                    priceHelper.GetPickupReturnCost(viewModel.ReturnLocationId),
                    0, 0, 0, 0, null,
                    campaignId,
                    new Dictionary<int, Tuple<int, decimal>>());

                if (carSegmentPrice.HasValue)
                {
                    viewModel.Prices.Add(carSegment.Id, carSegmentPrice.Value);
                    if (campaignId.HasValue)
                    {
                        string campaingName = _context.CampaignTranslations.FirstOrDefault(x => x.LanguageId == GetSelectedLanguageId() && x.CampaignId == campaignId).Name;
                        viewModel.CarSegmentCampaigns.Add(carSegment.Id, campaingName);
                    }
                }
                else
                {
                    viewModel.CarSegments.Remove(carSegment);
                }
            }

            viewModel.Days = priceHelper.GetTotalDays();

            viewModel.CarCategories = await _context.CarCategories.Include(x => x.Translations).Where(x => x.CarSegments.Any(y => y.IsActive))
                .ToDictionaryAsync(x => x.Id, x => x.Translations?.FirstOrDefault(t => t.LanguageId == GetSelectedLanguageId())?.Name);
            viewModel.CarFuels = await _context.CarFuels.Include(x => x.Translations).Where(x => x.CarSegments.Any(y => y.IsActive))
                .ToDictionaryAsync(x => x.Id, x => x.Translations?.FirstOrDefault(t => t.LanguageId == GetSelectedLanguageId())?.Name);
            viewModel.CarGearboxes = await _context.CarGearboxes.Include(x => x.Translations).Where(x => x.CarSegments.Any(y => y.IsActive))
                .ToDictionaryAsync(x => x.Id, x => x.Translations?.FirstOrDefault(t => t.LanguageId == GetSelectedLanguageId())?.Name);
            viewModel.CarSeats = await _context.CarSegments.Where(x => x.IsActive).Select(x => x.Seats).Distinct().ToListAsync();
            viewModel.Campaigns = await _context.Campaigns.Include(x => x.Translations)
                .ToDictionaryAsync(x => x.Id, x => x.Translations?.FirstOrDefault(t => t.LanguageId == GetSelectedLanguageId())?.Name);

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

        //public IActionResult Index2(
        //    DateTime? PickupDate,
        //    TimeSpan? PickupTime,
        //    DateTime? ReturnDate,
        //    TimeSpan? ReturnTime,
        //    int? PickupLocation = null,
        //    int? ReturnLocation = null,
        //    string viewType = "Grid",
        //    string filteredCategoryIds = "",
        //    string filteredFuelIds = "",
        //    string filteredGearboxIds = "",
        //    string filteredSeatNumbers = "",
        //    string filteredCampaignIds = "",
        //    string? anchor = "")
        //{
        //    Models.SearchIndex viewModel = new();
        //    int defaultLocationId = _context.PickupReturnLocations.FirstOrDefault(x => x.IsDefault).Id;

        //    viewModel.PickupLocationId = PickupLocation ?? defaultLocationId;
        //    viewModel.ReturnLocationId = ReturnLocation ?? defaultLocationId;
        //    bool defaultDateTimes = (!PickupDate.HasValue || !ReturnDate.HasValue || !PickupTime.HasValue || !ReturnTime.HasValue);
        //    viewModel.PickupDate = PickupDate ?? dCore.Helpers.DateTime.RoundToNearestFutureQuarterHour(DateTime.Now).Date;
        //    viewModel.PickupTime = PickupTime ?? dCore.Helpers.DateTime.RoundToNearestFutureQuarterHour(DateTime.Now).TimeOfDay;
        //    viewModel.ReturnDate = ReturnDate ?? dCore.Helpers.DateTime.RoundToNearestFutureQuarterHour(DateTime.Now).AddDays(1).Date;
        //    viewModel.ReturnTime = ReturnTime ?? dCore.Helpers.DateTime.RoundToNearestFutureQuarterHour(DateTime.Now).AddDays(1).TimeOfDay;
        //    DateTime submittedPickupDateTime = dCore.Helpers.DateTime.GetNewBasedOn(viewModel.PickupDate, viewModel.PickupTime, withZeroSeconds:true);
        //    DateTime submittedReturnDateTime = dCore.Helpers.DateTime.GetNewBasedOn(viewModel.ReturnDate, viewModel.ReturnTime, withZeroSeconds: true);

        //    Shared.Providers.PickupReturnLocation pickupReturnLocationProvider = new Shared.Providers.PickupReturnLocation(_context);
        //    DateTime minimumPickupDateTime = pickupReturnLocationProvider.GetNextCompliantWithAnticipationDateTime(viewModel.PickupLocationId, submittedPickupDateTime);
        //    bool submittedReturnDateTimeAdjusted = false;
        //    if (submittedReturnDateTime <= minimumPickupDateTime)
        //    {
        //        submittedReturnDateTime = minimumPickupDateTime.AddHours(1);
        //        submittedReturnDateTimeAdjusted = true;
        //    }
        //    DateTime minimumReturnDateTime = pickupReturnLocationProvider.GetNextAvailableDateTime(viewModel.ReturnLocationId, submittedReturnDateTime);

        //    if (submittedPickupDateTime != minimumPickupDateTime)
        //    {
        //        viewModel.PickupDate = minimumPickupDateTime.Date;
        //        viewModel.PickupTime = minimumPickupDateTime.TimeOfDay;
        //        viewModel.PickupReturnValuesAdjusted = !defaultDateTimes;
        //    }
        //    if (minimumPickupDateTime >= minimumReturnDateTime)
        //    {
        //        minimumReturnDateTime = minimumPickupDateTime.AddHours(1);
        //    }
        //    if (submittedReturnDateTimeAdjusted || submittedReturnDateTime != minimumReturnDateTime)
        //    {
        //        viewModel.ReturnDate = minimumReturnDateTime.Date;
        //        viewModel.ReturnTime = minimumReturnDateTime.TimeOfDay;
        //        viewModel.PickupReturnValuesAdjusted = !defaultDateTimes;
        //    }

        //    IQueryable<Data.Models.Database.CarSegment> carSegments = _context.CarSegments
        //        .Include(x => x.Translations.Where(t => t.LanguageId == GetSelectedLanguageId()));

        //    if (!string.IsNullOrEmpty(filteredCategoryIds))
        //    {
        //        viewModel.SelectedCategoryIds = filteredCategoryIds.Split(',').Select(int.Parse).ToArray();
        //        carSegments = carSegments.Where(x => viewModel.SelectedCategoryIds.Contains(x.CarCategoryId));
        //    }
        //    if (!string.IsNullOrEmpty(filteredFuelIds))
        //    {
        //        viewModel.SelectedFuelIds = filteredFuelIds.Split(',').Select(int.Parse).ToArray();
        //        carSegments = carSegments.Where(x => viewModel.SelectedFuelIds.Contains(x.CarFuelId));
        //    }
        //    if (!string.IsNullOrEmpty(filteredGearboxIds))
        //    {
        //        viewModel.SelectedGearboxIds = filteredGearboxIds.Split(',').Select(int.Parse).ToArray();
        //        carSegments = carSegments.Where(x => viewModel.SelectedGearboxIds.Contains(x.CarGearboxId));
        //    }
        //    if (!string.IsNullOrEmpty(filteredSeatNumbers))
        //    {
        //        viewModel.SelectedSeatNumbers = filteredSeatNumbers.Split(',').Select(int.Parse).ToArray();
        //        carSegments = carSegments.Where(x => viewModel.SelectedSeatNumbers.Contains(x.Seats));
        //    }
        //    if (!string.IsNullOrEmpty(filteredCampaignIds))
        //    {
        //        viewModel.SelectedCampaignIds = filteredCampaignIds.Split(',').Select(int.Parse).ToArray();
        //        carSegments = carSegments.Where(x => x.Campaigns.Any(y => viewModel.SelectedCampaignIds.Contains(y.Id)));
        //    }

        //    viewModel.CarSegments = carSegments.ToList();

        //    DateTime pickupDateTime = new DateTime(viewModel.PickupDate.Year, viewModel.PickupDate.Month,
        //        viewModel.PickupDate.Day, viewModel.PickupTime.Hours, viewModel.PickupTime.Minutes, 0);
        //    DateTime returnDateTime = new DateTime(viewModel.ReturnDate.Year, viewModel.ReturnDate.Month,
        //        viewModel.ReturnDate.Day, viewModel.ReturnTime.Hours, viewModel.ReturnTime.Minutes, 0);

        //    Helpers.CostCalculator priceHelper = new Helpers.CostCalculator(_context);
        //    foreach (CarSegment carSegment in viewModel.CarSegments.ToArray())
        //    {
        //        decimal? carSegmentPrice = 
        //            priceHelper.GetRentCost(pickupDateTime, returnDateTime, carSegment.Id)
        //            + priceHelper.GetPickupReturnCost(pickupDateTime, returnDateTime, viewModel.PickupLocationId)
        //            + priceHelper.GetPickupReturnCost(pickupDateTime, returnDateTime, viewModel.ReturnLocationId);
        //        if (carSegmentPrice.HasValue)
        //        {
        //            viewModel.Prices.Add(carSegment.Id, carSegmentPrice.Value);
        //        }
        //        else
        //        {
        //            viewModel.CarSegments.Remove(carSegment);
        //        }
        //    }

        //    viewModel.Days = priceHelper.GetDaysDifference(pickupDateTime, returnDateTime);
        //    viewModel.Anchor = anchor;

        //    viewModel.CarCategories = _context.CarCategories.Include(x => x.Translations)
        //        .ToDictionary(x => x.Id, x => x.Translations?.FirstOrDefault(t => t.LanguageId == GetSelectedLanguageId())?.Name);
        //    viewModel.CarFuels = _context.CarFuels.Include(x => x.Translations)
        //        .ToDictionary(x => x.Id, x => x.Translations?.FirstOrDefault(t => t.LanguageId == GetSelectedLanguageId())?.Name);
        //    viewModel.CarGearboxes = _context.CarGearboxes.Include(x => x.Translations)
        //            .ToDictionary(x => x.Id, x => x.Translations?.FirstOrDefault(t => t.LanguageId == GetSelectedLanguageId())?.Name);
        //    viewModel.CarSeats = _context.CarSegments.Select(x => x.Seats).Distinct().ToList();
        //    viewModel.Campaigns = _context.Campaigns.Include(x => x.Translations)
        //        .ToDictionary(x => x.Id, x => x.Translations?.FirstOrDefault(t => t.LanguageId == GetSelectedLanguageId())?.Name);
        //    viewModel.ViewLayout = viewType;

        //    var locations = _context.PickupReturnLocations
        //        .Include(x => x.Translations)
        //        .Select(x => new
        //        {
        //            x.Id,
        //            x.Translations.FirstOrDefault(t => t.LanguageId == GetSelectedLanguageId()).Name
        //        }).ToList();
        //    ViewBag.PickupLocations = new SelectList(locations, "Id", "Name", viewModel.PickupLocationId);
        //    ViewBag.ReturnLocations = new SelectList(locations, "Id", "Name", viewModel.ReturnLocationId);
        //    BuildViewBag();

        //    return View(viewModel);
        //}
    }
}