using AMRent.BackOffice.Models.DataTables;
using AMRent.Data.Contexts;
using AMRent.Data.Enums;
using AMRent.Data.Models.Database;
using AMRent.Shared.Providers;
using CsvHelper;
using CsvHelper.Configuration;
using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NLog.Filters;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace AMRent.BackOffice.Controllers
{
    [Extensions.AuthorizePermission(Data.Enums.Permissions.Reservations)]
    public class QuotationsController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly dCore.MultiLanguage.Providers.TranslationProvider _translationProvider;

        public QuotationsController(FullDatabaseContext context, ILogger<QuotationsController> logger, dCore.MultiLanguage.Providers.TranslationProvider translationProvider, IConfiguration configuration) : base(context, logger)
        {
            _configuration = configuration;
            _translationProvider = translationProvider;
        }

        public void BuildViewBag(Quotation quotation)
        {
            List<Data.Models.Database.PickupReturnLocation> pickupReturnLocations = _context.PickupReturnLocations
                .Include(x => x.Translations.Where(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese))
                .Where(x => x.Id > 0)
                .OrderBy(x => x.Translations.FirstOrDefault(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese).Name)
                .ToList();
            pickupReturnLocations.Add(_context.PickupReturnLocations
                .Include(x => x.Translations.Where(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese))
                .FirstOrDefault(x => x.Id == -1));

            ViewBag.PickupLocations = new SelectList(pickupReturnLocations.Select(x => new
            {
                x.Id,
                x.Translations.FirstOrDefault(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese).Name
            }), "Id", "Name", quotation.PickupLocationId);

            ViewBag.ReturnLocations = new SelectList(pickupReturnLocations.Select(x => new
            {
                x.Id,
                x.Translations.FirstOrDefault(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese).Name
            }), "Id", "Name", quotation.ReturnLocationId);

            ViewBag.CustomerTelephonePrefixCountries = new SelectList(_context.Countries.Select(x => new
            {
                x.Id,
                Text = $"{x.Translations.FirstOrDefault(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese).Name} (+{x.TelephoneCode})"
            }), "Id", "Text", quotation.CustomerTelephonePrefixCountryId);

            ViewBag.Users = new SelectList(_context.Users.Where(x => !x.Roles.All(y => y.Id == (int)Data.Enums.UserRoles.Customer)).Select(x => new
            {
                x.Id,
                Name = $"{x.FirstName} {x.LastName}"
            }), "Id", "Name", quotation.UserId);

            ViewBag.QuotationSources = new SelectList(dCore.Helpers.Enum.GetWithDescription<Data.Enums.ReservationQuotationSources>()
                .Where(x => x.Key != Data.Enums.ReservationQuotationSources.W.ToString()).Select(x => new SelectListItem
                {
                    Text = x.Value,
                    Value = x.Key
                }), "Value", "Text", quotation.Source);

            var quotationStatus = dCore.Helpers.Enum.GetWithDescription<Data.Enums.QuotationStatus>();
            quotationStatus.Remove($"{Data.Enums.QuotationStatus.Requested}");

            ViewBag.QuotationStatus = new SelectList(quotationStatus.Select(x => new SelectListItem
            {
                Text = x.Value,
                Value = x.Key
            }), "Value", "Text", quotation.Status);

            ViewBag.Languages = new SelectList(_context.Languages.Select(x => new
            {
                x.Id,
                x.Name
            }), "Id", "Name", quotation.LanguageId);

            var carSegments = _context.CarSegments.Select(x => new
            {
                x.Id,
                Name = $"{x.Code} - {x.Translations.FirstOrDefault(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese).Name}"
            }).ToList().OrderBy(x => x.Name);
            ViewBag.CarSegments = new List<SelectList>();

            var insuranceLevels = _context.InsuranceLevels.Select(x => new
            {
                x.Id,
                x.Translations.FirstOrDefault(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese).Name
            });
            ViewBag.InsuranceLevels = new List<SelectList>();

            var campaigns = _context.Campaigns.Select(x => new
            {
                x.Id,
                Name = x.Translations.FirstOrDefault(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese).Name
            }).ToList();

            ViewBag.Campaigns = new List<string>();

            var vouchers = _context.Vouchers.Select(x => new
            {
                x.Id,
                x.Code
            });
            ViewBag.Vouchers = new List<SelectList>();

            foreach (var quotationItem in quotation.QuotationItems)
            {
                ViewBag.CarSegments.Add(new SelectList(carSegments, "Id", "Name", quotationItem.CarSegmentId));
                ViewBag.InsuranceLevels.Add(new SelectList(insuranceLevels, "Id", "Name", quotationItem.InsuranceLevelId));
                ViewBag.Vouchers.Add(new SelectList(vouchers, "Id", "Code", quotationItem.VoucherId));
                ViewBag.Campaigns.Add(campaigns.FirstOrDefault(x => x.Id == quotationItem.CampaignId)?.Name ?? "");

                if (quotationItem.Extras.Any())
                {
                    foreach (QuotationItemExtra quotationItemExtra in quotationItem.Extras)
                    {
                        quotationItemExtra.Extra = _context.Extras.Include(x => x.Translations.Where(y => y.LanguageId == (int)Languages.Portuguese)).FirstOrDefault(x => x.Id == quotationItemExtra.ExtraId);
                    }
                }

                foreach (Extra extra in _context.Extras.Where(x => x.IsActive && !quotationItem.Extras.Select(y => y.ExtraId).Contains(x.Id)).Include(x => x.Translations.Where(y => y.LanguageId == (int)Languages.Portuguese)))
                {
                    quotationItem.Extras.Add(new QuotationItemExtra()
                    {
                        ExtraId = extra.Id,
                        UnitValue = 0,
                        Quantity = 0,
                        Extra = extra
                    });
                }

                quotationItem.Extras = quotationItem.Extras.OrderBy(x => x.Extra.Translations.First().Name).ToList();

                if (quotationItem.Services.Any())
                {
                    foreach (QuotationItemService quotationItemService in quotationItem.Services)
                    {
                        quotationItemService.Service = _context.Services.Include(x => x.Translations.Where(y => y.LanguageId == (int)Languages.Portuguese)).FirstOrDefault(x => x.Id == quotationItemService.ServiceId);
                    }
                }

                foreach (Service service in _context.Services.Where(x => x.IsActive && !quotationItem.Services.Select(y => y.ServiceId).Contains(x.Id)).Include(x => x.Translations.Where(y => y.LanguageId == (int)Languages.Portuguese)))
                {
                    quotationItem.Services.Add(new QuotationItemService()
                    {
                        ServiceId = service.Id,
                        Value = 0,
                        Service = service
                    });
                }

                quotationItem.Services = quotationItem.Services.OrderBy(x => x.Service.Translations.First().Name).ToList();
            }
        }

        [HttpPost]
        public JsonResult Get(Models.DataTables.ViewModel viewModel)
        {
            JsonResult result = new JsonResult("");
            try
            {
                IQueryable<Quotation> recordsTotal = _context.Quotations
                    .Include(x => x.PickupLocation)
                        .ThenInclude(x => x.Translations.Where(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese))
                    .Include(x => x.ReturnLocation)
                        .ThenInclude(x => x.Translations.Where(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese));
                var recordsTotalCount = recordsTotal.Count();
                var recordsFiltered = recordsTotal;

                for (int i = 0; i < viewModel.columns.Count; i++)
                {
                    if (!string.IsNullOrEmpty(viewModel.columns[i].search.value))
                    {
                        string searchedValue = viewModel.columns[i].search.value;
                        switch (i)
                        {
                            case 0:
                                recordsFiltered = recordsFiltered.Where(x => x.Number.Contains(searchedValue));
                                break;
                            case 1:
                                List<Guid> searchedCollaboratorIds = new();
                                foreach (var item in searchedValue.Split('|'))
                                {
                                    searchedCollaboratorIds.Add(Guid.Parse(item));
                                }
                                recordsFiltered = recordsFiltered.Where(x => x.UserId.HasValue && searchedCollaboratorIds.Contains(x.UserId.Value));
                                break;
                            case 2:
                                List<Data.Enums.QuotationStatus> searchedQuotationStatus = new();
                                foreach (var item in searchedValue.Split('|'))
                                {
                                    searchedQuotationStatus.Add(System.Enum.Parse<Data.Enums.QuotationStatus>(item));
                                }
                                recordsFiltered = recordsFiltered.Where(x => searchedQuotationStatus.Contains(x.Status));
                                break;
                            case 3:
                                recordsFiltered = recordsFiltered.Where(x => x.CustomerName.Contains(searchedValue));
                                break;
                            case 4:
                                List<int> searchedPickupLocationIds = new();
                                foreach (var item in searchedValue.Split('|'))
                                {
                                    searchedPickupLocationIds.Add(int.Parse(item));
                                }
                                recordsFiltered = recordsFiltered.Where(x => searchedPickupLocationIds.Contains(x.PickupLocationId));
                                break;
                            case 5:
                                string[] searchedPickupDates = searchedValue.Split('|');
                                if (DateTime.TryParse(searchedPickupDates.ElementAtOrDefault(0), out var from))
                                    recordsFiltered = recordsFiltered.Where(x => x.PickupDateTime.Date >= from);
                                if (DateTime.TryParse(searchedPickupDates.ElementAtOrDefault(1), out var to))
                                    recordsFiltered = recordsFiltered.Where(x => x.PickupDateTime.Date <= to);
                                break;
                            case 6:
                                string[] searchedExpireDates = searchedValue.Split('|');
                                if (DateTime.TryParse(searchedExpireDates.ElementAtOrDefault(0), out var expireDatefrom))
                                    recordsFiltered = recordsFiltered.Where(x => x.ExpireDateTime.Date >= expireDatefrom);
                                if (DateTime.TryParse(searchedExpireDates.ElementAtOrDefault(1), out var expireDateTo))
                                    recordsFiltered = recordsFiltered.Where(x => x.ExpireDateTime.Date <= expireDateTo);
                                break;
                            case 7:
                                string[] searchedCreatedAtDates = searchedValue.Split('|');
                                if (DateTime.TryParse(searchedCreatedAtDates.ElementAtOrDefault(0), out var createdAtDatefrom))
                                    recordsFiltered = recordsFiltered.Where(x => x.CreateDate.Date >= createdAtDatefrom);
                                if (DateTime.TryParse(searchedCreatedAtDates.ElementAtOrDefault(1), out var createdAtDateTo))
                                    recordsFiltered = recordsFiltered.Where(x => x.CreateDate.Date <= createdAtDateTo);
                                break;
                        }
                    }
                }
                if (viewModel.order != null && viewModel.order.Any())
                {
                    switch (viewModel.order.First().column)
                    {
                        case 0:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.Number) : recordsFiltered.OrderByDescending(x => x.Number);
                            break;
                        case 1:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => (x.User.FirstName ?? "") + (x.User.LastName ?? "")) : recordsFiltered.OrderByDescending(x => (x.User.FirstName ?? "") + (x.User.LastName ?? ""));
                            break;
                        case 2:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.Status) : recordsFiltered.OrderByDescending(x => x.Status);
                            break;
                        case 3:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.CustomerName) : recordsFiltered.OrderByDescending(x => x.CustomerName);
                            break;
                        case 4:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.PickupLocation.Translations.FirstOrDefault().Name) : recordsFiltered.OrderByDescending(x => x.PickupLocation.Translations.FirstOrDefault().Name);
                            break;
                        case 5:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.PickupDateTime) : recordsFiltered.OrderByDescending(x => x.PickupDateTime);
                            break;
                        case 6:
                            recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                recordsFiltered.OrderBy(x => x.CreateDate) : recordsFiltered.OrderByDescending(x => x.CreateDate);
                            break;
                    }
                }
                if (!string.IsNullOrEmpty(viewModel.search.value))
                {
                    var predicate = PredicateBuilder.New<Quotation>();
                    predicate = predicate.Or(x => x.Number.Contains(viewModel.search.value));
                    predicate = predicate.Or(x => x.CustomerName.Contains(viewModel.search.value));
                    predicate = predicate.Or(x => x.CustomerEmailAddress.Contains(viewModel.search.value));
                    predicate = predicate.Or(x => x.CustomerTelephone.Contains(viewModel.search.value));
                    predicate = predicate.Or(x => x.Comments.Contains(viewModel.search.value));
                    predicate = predicate.Or(x => x.PickupLocation.Translations.FirstOrDefault().Name.Contains(viewModel.search.value));
                    predicate = predicate.Or(x => x.ReturnLocation.Translations.FirstOrDefault().Name.Contains(viewModel.search.value));
                    predicate = predicate.Or(x => x.QuotationItems.Any(x => x.Voucher.Code.Contains(viewModel.search.value)));
                    predicate = predicate.Or(x => x.QuotationItems.Any(x => x.CarSegment.Code.Contains(viewModel.search.value)));

                    recordsFiltered = recordsFiltered.Where(predicate);
                }
                var recordsFilteredCount = recordsFiltered.Count();

                Data.Models.View.QuotationIndex[] recordsFilteredPage;
                if (viewModel.order == null || (viewModel.order.Any() && viewModel.order.First().column == 9))
                {
                    recordsFilteredPage = recordsFiltered.Select(x => new AMRent.Data.Models.View.QuotationIndex()
                    {
                        Id = x.Id,
                        Number = x.Number,
                        CollaboratorName = x.User != null ? $"{x.User.FirstName} {x.User.LastName}" : null,
                        Status = dCore.Helpers.Enum.GetDescription(x.Status),
                        CustomerName = x.CustomerName,
                        PickupLocation = x.PickupLocation.Translations.FirstOrDefault().Name,
                        PickupDateTime = x.PickupDateTime,
                        CreateDateTime = x.CreateDate,
                        ExpireDateTime = x.ExpireDateTime,
                        Priority = 4
                    }).ToArray();
                }
                else
                {
                    recordsFilteredPage = recordsFiltered.Skip(viewModel.start).Take(viewModel.length).Select(x => new AMRent.Data.Models.View.QuotationIndex()
                    {
                        Id = x.Id,
                        Number = x.Number,
                        CollaboratorName = x.User != null ? $"{x.User.FirstName} {x.User.LastName}" : null,
                        Status = dCore.Helpers.Enum.GetDescription(x.Status),
                        CustomerName = x.CustomerName,
                        PickupLocation = x.PickupLocation.Translations.FirstOrDefault().Name,
                        PickupDateTime = x.PickupDateTime,
                        CreateDateTime = x.CreateDate,
                        ExpireDateTime = x.ExpireDateTime
                    }).ToArray();
                }

                int defaultQuotationExpireDays = int.Parse(_context.Settings.First(x => x.Key == "DefaultQuotationExpireDateDays").Value);

                string[] notIncludedStatus =
                {
                    dCore.Helpers.Enum.GetDescription(Data.Enums.QuotationStatus.Cancelled),
                    dCore.Helpers.Enum.GetDescription(Data.Enums.QuotationStatus.Finished)
                };

                foreach (var item in recordsFilteredPage.Where(x => !notIncludedStatus.Contains(x.Status)))
                {
                    if (string.IsNullOrEmpty(item.CollaboratorName))
                    {
                        item.BackgroundColor = "#d9883c";
                        item.Priority = 3;
                    }
                    else if (item.ExpireDateTime < DateTime.Now)
                    {
                        item.BackgroundColor = "red";
                        item.Priority = 1;
                    }
                    else if (item.ExpireDateTime < DateTime.Now.AddDays(defaultQuotationExpireDays / 2))
                    {
                        item.BackgroundColor = "yellow";
                        item.Priority = 2;
                    }
                }

                if (viewModel.order == null || (viewModel.order.Any() && viewModel.order.First().column == 9))
                {
                    recordsFilteredPage = recordsFilteredPage.OrderBy(x => x.Priority).ToArray();
                    recordsFilteredPage = recordsFilteredPage.Skip(viewModel.start).Take(viewModel.length).ToArray();
                }

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

        // GET: Quotations
        public async Task<IActionResult> Index()
        {
            ViewBag.PickupLocations = new SelectList(_context.PickupReturnLocations.Select(x => new
            {
                x.Id,
                x.Translations.FirstOrDefault(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese).Name
            }).OrderBy(x => x.Name), "Id", "Name");
            ViewBag.Collaborators = new SelectList(_context.Users.Where(x => !x.Roles.All(y => y.Id == (int)Data.Enums.UserRoles.Customer)).Select(x => new
            {
                x.Id,
                Name = $"{x.FirstName} {x.LastName}"
            }), "Id", "Name");

            var quotationStatus = dCore.Helpers.Enum.GetWithDescription<Data.Enums.QuotationStatus>();

            ViewBag.Status = new SelectList(quotationStatus.Select(x => new SelectListItem
            {
                Text = x.Value,
                Value = x.Key
            }), "Value", "Text");
            return View();
        }

        public IActionResult Export(DateTime? startPickupDate, DateTime? endPickupDate, DateTime? startCreateDate, DateTime? endCreateDate)
        {
            // Get the filtered Reservations
            IQueryable<Quotation> quotations = _context.Quotations
                .Include(x => x.User)
                .Include(x => x.QuotationItems)
                    .ThenInclude(y => y.Voucher)
                .Include(x => x.QuotationItems)
                    .ThenInclude(y => y.Campaign)
                        .ThenInclude(z => z.Translations.Where(y => y.LanguageId == (int)Data.Enums.Languages.Portuguese))
                .Include(x => x.CustomerTelephonePrefixCountry)
                .Include(x => x.DataProtectionConsents)
                    .ThenInclude(y => y.DataProtectionConsent)
                        .ThenInclude(z => z.Translations.Where(y => y.LanguageId == (int)Data.Enums.Languages.Portuguese));

            if (startPickupDate.HasValue)
            {
                quotations = quotations.Where(x => x.PickupDateTime >= startPickupDate.Value);
            }
            if (endPickupDate.HasValue)
            {
                endPickupDate = endPickupDate.Value.AddDays(1);
                quotations = quotations.Where(x => x.PickupDateTime <= endPickupDate.Value);
            }
            if (startCreateDate.HasValue)
            {
                quotations = quotations.Where(x => x.CreateDate >= startCreateDate.Value);
            }
            if (endCreateDate.HasValue)
            {
                endCreateDate = endCreateDate.Value.AddDays(1);
                quotations = quotations.Where(x => x.CreateDate <= endCreateDate.Value);
            }

            // Build the CSV
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (StreamWriter streamWriter = new StreamWriter(memoryStream, Encoding.UTF8, leaveOpen: true))
                {
                    using (CsvWriter csvWriter = new CsvWriter(streamWriter, new CsvConfiguration(CultureInfo.InvariantCulture)))
                    {
                        // Header
                        csvWriter.WriteField("Cotação");
                        csvWriter.WriteField("Colaborador");
                        csvWriter.WriteField("Estado");
                        csvWriter.WriteField("Vouchers");
                        csvWriter.WriteField("Campanhas");
                        csvWriter.WriteField("Nome cliente");
                        csvWriter.WriteField("Telefone cliente");
                        csvWriter.WriteField("Email cliente");
                        csvWriter.WriteField("Data registo");
                        csvWriter.WriteField("Validade");
                        csvWriter.WriteField("Data levantamento");
                        csvWriter.WriteField("Data devolução");
                        csvWriter.WriteField("RGPD");
                        csvWriter.NextRecordAsync();

                        // Data
                        foreach (Quotation quotation in quotations.ToList())
                        {
                            csvWriter.WriteField(quotation.Number);
                            csvWriter.WriteField($"{quotation.User?.FirstName} {quotation.User?.LastName}");
                            csvWriter.WriteField(dCore.Helpers.Enum.GetDescription(quotation.Status));
                            List<string> usedVouchers = new();
                            List<string> usedCampaigns = new();
                            foreach (var item in quotation.QuotationItems)
                            {
                                if (item.Voucher != null && !usedVouchers.Contains(item.Voucher.Code))
                                {
                                    usedVouchers.Add(item.Voucher.Code);
                                }
                                if (item.Campaign != null && !usedCampaigns.Contains(item.Campaign.Translations.FirstOrDefault().Name))
                                {
                                    usedCampaigns.Add(item.Campaign.Translations.FirstOrDefault().Name);
                                }
                            }
                            csvWriter.WriteField(string.Join(',', usedVouchers));
                            csvWriter.WriteField(string.Join(',', usedCampaigns));
                            csvWriter.WriteField(quotation.CustomerName);
                            csvWriter.WriteField($"{(quotation.CustomerTelephonePrefixCountry != null && !string.IsNullOrEmpty(quotation.CustomerTelephone) ? $"+{quotation.CustomerTelephonePrefixCountry.TelephoneCode} " : "")}{(!string.IsNullOrEmpty(quotation.CustomerTelephone) ? quotation.CustomerTelephone : "")}");
                            csvWriter.WriteField(quotation.CustomerEmailAddress);
                            csvWriter.WriteField(quotation.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"));
                            csvWriter.WriteField(quotation.ExpireDateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                            csvWriter.WriteField(quotation.PickupDateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                            csvWriter.WriteField(quotation.ReturnDateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                            List<string> dataProtectionConsents = new();
                            foreach (var consent in quotation.DataProtectionConsents)
                            {
                                dataProtectionConsents.Add($"{consent.DataProtectionConsent?.Translations?.FirstOrDefault()?.Text}: {(consent.HasConsented ? "Sim" : "Não")}");
                            }
                            csvWriter.WriteField(string.Join(" / ", dataProtectionConsents));
                            csvWriter.NextRecordAsync();
                        }
                    }
                    streamWriter.Flush();
                }
                memoryStream.Position = 0;
                var fileName = $"ExportCotacoes_{DateTime.Now::yyyy-MM-dd_HH_mm_ss}.csv";
                return File(memoryStream.ToArray(), "text/csv", fileName);
            }
        }

        // GET: Quotations/Create
        public IActionResult Create()
        {
            Quotation viewModel = new();
            if (TempData.ContainsKey("ViewModel"))
            {
                viewModel = JsonSerializer.Deserialize<Quotation>(TempData["ViewModel"] as string);
                TempData.Remove("ViewModel");
            }
            else
            {
                viewModel.QuotationItems = new List<QuotationItem>()
                {
                    new QuotationItem()
                };
                viewModel.PickupDateTime = DateTime.Now.AddDays(1);
                viewModel.ReturnDateTime = DateTime.Now.AddDays(2);
            }
            BuildViewBag(viewModel);
            return View(viewModel);
        }

        // POST: Quotations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Quotation quotation, bool addItem = false, int removeIndex = 0, bool isRefresh = false, bool sendEmail = false)
        {
            if (isRefresh)
            {
                if (removeIndex > 0)
                {
                    quotation.QuotationItems.RemoveAt(removeIndex);
                }
            }
            else
            {
                if (!ModelState.IsValid)
                {
                    BuildViewBag(quotation);
                    return View(quotation);
                }

            }

            Shared.Providers.CostCalculator costCalculatorHelper = new Shared.Providers.CostCalculator(_context, quotation.PickupDateTime, quotation.ReturnDateTime);

            quotation.TotalDays = costCalculatorHelper.GetTotalDays();

            foreach (var quotationItem in quotation.QuotationItems)
            {
                costCalculatorHelper.SetSegment(quotationItem.CarSegmentId);

                quotationItem.Extras.RemoveAll(x => x.Quantity == 0);
                quotationItem.Services.RemoveAll(x => x.Value == 0);

                quotationItem.CarSegment = await _context.CarSegments.IgnoreAutoIncludes()
                .Include(x => x.CarFuel)
                .ThenInclude(x => x.Translations.Where(y => y.LanguageId == (int)Data.Enums.Languages.Portuguese))
                .Include(x => x.Insurances)
                .FirstOrDefaultAsync(x => x.Id == quotationItem.CarSegmentId);
                if (!quotationItem.CarSegment.Insurances.Select(x => x.InsuranceLevelId).Contains(quotationItem.InsuranceLevelId))
                {
                    quotationItem.InsuranceLevelId = quotationItem.CarSegment.Insurances.FirstOrDefault().InsuranceLevelId;
                }

                if (quotationItem.VoucherId.HasValue)
                {
                    string? voucherCode = _context.Vouchers.FirstOrDefault(x => x.Id == quotationItem.VoucherId)?.Code;

                    if (!string.IsNullOrEmpty(voucherCode))
                    {
                        quotationItem.VoucherId = costCalculatorHelper.GetVoucherId(voucherCode, extraIds: quotationItem.Extras.Select(x => x.ExtraId).ToArray(), isBackofficeRequest: true);
                    }
                }

                if (!quotationItem.VoucherId.HasValue)
                {
                    quotationItem.CampaignId = costCalculatorHelper.GetCampaignId(extraIds: quotationItem.Extras.Select(x => x.ExtraId).ToArray(), isBackofficeRequest: true);
                }

                quotationItem.CarSegmentCost = costCalculatorHelper.GetCarCost() ?? 0;
                quotationItem.PickupCost = costCalculatorHelper.GetPickupReturnCost(quotation.PickupLocationId);
                quotationItem.ReturnCost = costCalculatorHelper.GetPickupReturnCost(quotation.ReturnLocationId);
                quotationItem.InsuranceCost = costCalculatorHelper.GetInsuranceCost(quotationItem.InsuranceLevelId);
                quotationItem.InsuranceExcess = costCalculatorHelper.GetInsuranceExcess(quotationItem.InsuranceLevelId);

                if (quotationItem.Extras.Any())
                {
                    foreach (KeyValuePair<int, decimal> extraCost in costCalculatorHelper.GetExtrasCost(quotationItem.Extras?.Select(x => x.ExtraId).ToList(), quotationItem.InsuranceLevelId))
                    {
                        quotationItem.Extras.FirstOrDefault(x => x.ExtraId == extraCost.Key).UnitValue = extraCost.Value;
                    }
                }

                var pickupReturnTemporaryTaxesCosts = costCalculatorHelper.GetPickupReturnTemporaryTaxes();
                quotationItem.PickupReturnTemporaryTaxes.Clear();

                foreach (var pickupReturnTemporaryTaxCost in pickupReturnTemporaryTaxesCosts)
                {
                    quotationItem.PickupReturnTemporaryTaxes.Add(new QuotationItemPickupReturnTemporaryTax()
                    {
                        PickupReturnTemporaryTaxId = pickupReturnTemporaryTaxCost.Key,
                        Quantity = pickupReturnTemporaryTaxCost.Value.Item1,
                        UnitValue = pickupReturnTemporaryTaxCost.Value.Item2,
                        PickupReturnTemporaryTax = _context.PickupReturnTemporaryTaxes.Include(x => x.Translations).FirstOrDefault(x => x.Id == pickupReturnTemporaryTaxCost.Key)
                    });
                }

                quotationItem.TotalCost = costCalculatorHelper.GetTotalCost(quotationItem);
            }

            if (isRefresh)
            {
                if (addItem)
                {
                    quotation.QuotationItems.Add(new QuotationItem());
                }
            }
            else
            {
                if (ModelState.IsValid)
                {
                    quotation.UserId = Guid.Parse(User.Claims.First(x => x.Type == ClaimTypes.UserData).Value);

                    _context.Add(quotation);
                    _context.SaveChanges();

                    if (sendEmail)
                    {
                        dCore.Communication.Models.SmtpConfiguration smtpConfiguration = _configuration.GetSection("SmtpConfiguration").Get<dCore.Communication.Models.SmtpConfiguration>();
                        Shared.Providers.EmailSender emailSender = new Shared.Providers.EmailSender(_context, _translationProvider, smtpConfiguration, quotation.LanguageId);
                        emailSender.Send(Data.Enums.EmailContentTypes.QuotationRegistration, _configuration["Environment"] == "Test", quotation.Id);
                    }

                    return RedirectToAction(nameof(Index));
                }
            }

            BuildViewBag(quotation);
            return View(quotation);
        }

        // GET: Quotations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Quotations == null)
            {
                return NotFound();
            }

            Quotation viewModel = new();
            if (TempData.ContainsKey("ViewModel"))
            {
                viewModel = JsonSerializer.Deserialize<Quotation>(TempData["ViewModel"] as string);
                TempData.Remove("ViewModel");
            }
            else
            {
                viewModel = await _context.Quotations
                .Include(x => x.QuotationItems)
                    .ThenInclude(x => x.CarSegment)
                        .ThenInclude(x => x.CarFuel)
                            .ThenInclude(x => x.Translations.Where(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese))
                .Include(x => x.QuotationItems)
                    .ThenInclude(x => x.Extras)
                        .ThenInclude(x => x.Extra)
                            .ThenInclude(x => x.Translations.Where(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese))
                .Include(x => x.QuotationItems)
                    .ThenInclude(x => x.Services)
                        .ThenInclude(x => x.Service)
                            .ThenInclude(x => x.Translations.Where(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese))
                .Include(x => x.QuotationItems)
                    .ThenInclude(x => x.PickupReturnTemporaryTaxes)
                        .ThenInclude(x => x.PickupReturnTemporaryTax)
                            .ThenInclude(x => x.Translations.Where(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese))
                .Include(x => x.Changes)
                    .ThenInclude(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id);

                Shared.Providers.CostCalculator costCalculatorHelper = new Shared.Providers.CostCalculator(_context, viewModel.PickupDateTime, viewModel.ReturnDateTime);

                viewModel.TotalDays = costCalculatorHelper.GetTotalDays();
            }

            if (viewModel == null)
            {
                return NotFound();
            }

            BuildViewBag(viewModel);
            return View(nameof(Edit), viewModel);
        }

        // POST: Quotations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Quotation quotation, TimeOnly pickupTime, TimeOnly returnTime, bool addItem = false, int removeIndex = 0, bool isRefresh = false, bool sendEmail = false)
        {
            if (id != quotation.Id)
            {
                return NotFound();
            }

            DateTime pickupDate = quotation.PickupDateTime;
            DateTime returnDate = quotation.ReturnDateTime;
            quotation.PickupDateTime = new DateTime(pickupDate.Year, pickupDate.Month, pickupDate.Day, pickupTime.Hour, pickupTime.Minute, 0);
            quotation.ReturnDateTime = new DateTime(returnDate.Year, returnDate.Month, returnDate.Day, returnTime.Hour, returnTime.Minute, 0);

            if (isRefresh)
            {
                if (removeIndex > 0)
                {
                    quotation.QuotationItems.RemoveAt(removeIndex);
                }
            }
            else
            {
                if (ModelState.IsValid)
                {
                    _context.QuotationItems.RemoveRange(_context.QuotationItems.Where(x => x.QuotationId == quotation.Id && !quotation.QuotationItems.Select(y => y.Id).Contains(x.Id)));

                    foreach (var quotationItem in quotation.QuotationItems)
                    {
                        quotationItem.Extras.RemoveAll(x => x.Quantity == 0);
                        quotationItem.Services.RemoveAll(x => x.Value == 0);
                    }
                }
            }

            Shared.Providers.CostCalculator costCalculatorHelper = new Shared.Providers.CostCalculator(_context, quotation.PickupDateTime, quotation.ReturnDateTime);

            quotation.TotalDays = costCalculatorHelper.GetTotalDays();

            foreach (var quotationItem in quotation.QuotationItems.Where(x => x.Id == 0))
            {
                costCalculatorHelper.SetSegment(quotationItem.CarSegmentId);

                quotationItem.CarSegment = await _context.CarSegments.IgnoreAutoIncludes()
                .Include(x => x.CarFuel)
                .ThenInclude(x => x.Translations.Where(y => y.LanguageId == (int)Data.Enums.Languages.Portuguese))
                .Include(x => x.Insurances)
                .FirstOrDefaultAsync(x => x.Id == quotationItem.CarSegmentId);
                if (!quotationItem.CarSegment.Insurances.Select(x => x.InsuranceLevelId).Contains(quotationItem.InsuranceLevelId))
                {
                    quotationItem.InsuranceLevelId = quotationItem.CarSegment.Insurances.FirstOrDefault().InsuranceLevelId;
                }

                if (quotationItem.VoucherId.HasValue)
                {
                    string? voucherCode = _context.Vouchers.FirstOrDefault(x => x.Id == quotationItem.VoucherId)?.Code;

                    if (!string.IsNullOrEmpty(voucherCode))
                    {
                        quotationItem.VoucherId = costCalculatorHelper.GetVoucherId(voucherCode, extraIds: quotationItem.Extras.Select(x => x.ExtraId).ToArray(), isBackofficeRequest: true);
                    }
                }

                if (!quotationItem.VoucherId.HasValue)
                {
                    quotationItem.CampaignId = costCalculatorHelper.GetCampaignId(extraIds: quotationItem.Extras.Select(x => x.ExtraId).ToArray(), isBackofficeRequest: true);
                }

                quotationItem.CarSegmentCost = costCalculatorHelper.GetCarCost() ?? 0;
                quotationItem.PickupCost = costCalculatorHelper.GetPickupReturnCost(quotation.PickupLocationId);
                quotationItem.ReturnCost = costCalculatorHelper.GetPickupReturnCost(quotation.ReturnLocationId);
                quotationItem.InsuranceCost = costCalculatorHelper.GetInsuranceCost(quotationItem.InsuranceLevelId);
                quotationItem.InsuranceExcess = costCalculatorHelper.GetInsuranceExcess(quotationItem.InsuranceLevelId);

                if (quotationItem.Extras.Any())
                {
                    foreach (KeyValuePair<int, decimal> extraCost in costCalculatorHelper.GetExtrasCost(quotationItem.Extras?.Select(x => x.ExtraId).ToList(), quotationItem.InsuranceLevelId))
                    {
                        quotationItem.Extras.FirstOrDefault(x => x.ExtraId == extraCost.Key).UnitValue = extraCost.Value;
                    }
                }

                var pickupReturnTemporaryTaxesCosts = costCalculatorHelper.GetPickupReturnTemporaryTaxes();
                quotationItem.PickupReturnTemporaryTaxes.Clear();

                foreach (var pickupReturnTemporaryTaxCost in pickupReturnTemporaryTaxesCosts)
                {
                    var newQuotationItemPickupReturnTemporaryTax = new QuotationItemPickupReturnTemporaryTax()
                    {
                        PickupReturnTemporaryTaxId = pickupReturnTemporaryTaxCost.Key,
                        Quantity = pickupReturnTemporaryTaxCost.Value.Item1,
                        UnitValue = pickupReturnTemporaryTaxCost.Value.Item2,
                    };

                    if (isRefresh)
                    {
                        newQuotationItemPickupReturnTemporaryTax.PickupReturnTemporaryTax = _context.PickupReturnTemporaryTaxes.Include(x => x.Translations).AsNoTracking().FirstOrDefault(x => x.Id == pickupReturnTemporaryTaxCost.Key);
                    }
                    quotationItem.PickupReturnTemporaryTaxes.Add(newQuotationItemPickupReturnTemporaryTax);
                }

                quotationItem.TotalCost = costCalculatorHelper.GetTotalCost(quotationItem);
            }
            foreach (var quotationItem in quotation.QuotationItems.Where(x => x.Id != 0))
            {
                costCalculatorHelper.SetSegment(quotationItem.CarSegmentId);

                if (_context.QuotationItems.AsNoTracking().First(x => x.Id == quotationItem.Id).InsuranceLevelId != quotationItem.InsuranceLevelId)
                {
                    quotationItem.InsuranceCost = costCalculatorHelper.GetInsuranceCost(quotationItem.InsuranceLevelId);
                    quotationItem.InsuranceExcess = costCalculatorHelper.GetInsuranceExcess(quotationItem.InsuranceLevelId);
                }

                if (_context.Quotations.AsNoTracking().First(x => x.Id == quotation.Id).PickupLocationId != quotation.PickupLocationId)
                {
                    quotationItem.PickupCost = costCalculatorHelper.GetPickupReturnCost(quotation.PickupLocationId);
                }

                if (_context.Quotations.AsNoTracking().First(x => x.Id == quotation.Id).ReturnLocationId != quotation.ReturnLocationId)
                {
                    quotationItem.ReturnCost = costCalculatorHelper.GetPickupReturnCost(quotation.ReturnLocationId);
                }

                _context.QuotationItemServices.RemoveRange(_context.QuotationItemServices.Where(x => x.QuotationItemId == quotationItem.Id && !quotationItem.Services.Select(y => y.ServiceId).Contains(x.ServiceId)));
                _context.QuotationItemExtras.RemoveRange(_context.QuotationItemExtras.Where(x => x.QuotationItemId == quotationItem.Id && !quotationItem.Extras.Select(y => y.ExtraId).Contains(x.ExtraId)));

                foreach (var quotationItemExtra in quotationItem.Extras)
                {
                    if (quotationItemExtra.UnitValue == 0)
                    {
                        foreach (KeyValuePair<int, decimal> extraCost in costCalculatorHelper.GetExtrasCost(new List<int> { quotationItemExtra.ExtraId }, quotationItem.InsuranceLevelId))
                        {
                            quotationItemExtra.UnitValue = extraCost.Value;
                        }
                    }
                }

                quotationItem.TotalCost = costCalculatorHelper.GetTotalCost(quotationItem);

                if (isRefresh)
                {
                    foreach (var pickupReturnTemporaryTax in quotationItem.PickupReturnTemporaryTaxes)
                    {
                        pickupReturnTemporaryTax.PickupReturnTemporaryTax = _context.PickupReturnTemporaryTaxes.Include(x => x.Translations).AsNoTracking().FirstOrDefault(x => x.Id == pickupReturnTemporaryTax.PickupReturnTemporaryTaxId);
                    }
                }
            }

            if (isRefresh)
            {
                if (addItem)
                {
                    quotation.QuotationItems.Add(new QuotationItem());
                }

                BuildViewBag(quotation);
                return View(quotation);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    _context.UpdateWithTracking(quotation, Guid.Parse(User.Claims.First(x => x.Type == ClaimTypes.UserData).Value));
                    await _context.SaveChangesAsync();

                    if (sendEmail && quotation.Status != QuotationStatus.Cancelled && quotation.Status != QuotationStatus.Finished)
                    {
                        dCore.Communication.Models.SmtpConfiguration smtpConfiguration = _configuration.GetSection("SmtpConfiguration").Get<dCore.Communication.Models.SmtpConfiguration>();
                        Shared.Providers.EmailSender emailSender = new Shared.Providers.EmailSender(_context, _translationProvider, smtpConfiguration, quotation.LanguageId);
                        emailSender.Send(Data.Enums.EmailContentTypes.QuotationRegistration, _configuration["Environment"] == "Test", quotation.Id);
                    }

                    return RedirectToAction(nameof(Index));
                }

                BuildViewBag(quotation);
                return View(quotation);
            }
        }

        private bool QuotationExists(int id)
        {
            return (_context.Quotations?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
