using AMRent.BackOffice.Models;
using AMRent.BackOffice.Models.DataTables;
using AMRent.Data.Contexts;
using AMRent.Data.Enums;
using AMRent.Data.Models.Database;
using CsvHelper;
using CsvHelper.Configuration;
using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Configuration;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AMRent.BackOffice.Controllers
{
    [Extensions.AuthorizePermission(Data.Enums.Permissions.Reservations)]
    public class ReservationsController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly dCore.MultiLanguage.Providers.TranslationProvider _translationProvider;

        public ReservationsController(FullDatabaseContext context, ILogger<ReservationsController> logger, dCore.MultiLanguage.Providers.TranslationProvider translationProvider, IConfiguration configuration) : base(context, logger)
        {
            _configuration = configuration;
            _translationProvider = translationProvider;
        }

        public void BuildViewBag(Reservation reservation, bool isCreateAction = false)
        {
            SelectList reservationSources;
            SelectList reservationStatus;
            if (isCreateAction)
            {
                reservationSources = new SelectList(dCore.Helpers.Enum.GetWithDescription<Data.Enums.ReservationQuotationSources>()
                    .Where(x => x.Key != Data.Enums.ReservationQuotationSources.W.ToString()).Select(x => new SelectListItem
                    {
                        Text = x.Value,
                        Value = x.Key
                    }), "Value", "Text");

                reservationStatus = new SelectList(dCore.Helpers.Enum.GetWithDescription<Data.Enums.ReservationStatus>()
                    .Where(x => //x.Key != Data.Enums.ReservationStatus.Quotation.ToString() &&
                                x.Key != Data.Enums.ReservationStatus.Cancelled.ToString()).Select(x => new SelectListItem
                                {
                                    Text = x.Value,
                                    Value = x.Key
                                }), "Value", "Text", reservation.Status);
            }
            else
            {
                reservationSources = new SelectList(dCore.Helpers.Enum.GetWithDescription<Data.Enums.ReservationQuotationSources>().Select(x => new SelectListItem
                {
                    Text = x.Value,
                    Value = x.Key
                }), "Value", "Text", reservation.Source);

                reservationStatus = new SelectList(dCore.Helpers.Enum.GetWithDescription<Data.Enums.ReservationStatus>().Select(x => new SelectListItem
                {
                    Text = x.Value,
                    Value = x.Key
                }), "Value", "Text", reservation.Status);
            }

            ViewBag.ReservationSources = reservationSources;
            ViewBag.ReservationStatus = reservationStatus;

            ViewBag.CarSegments = new SelectList(_context.CarSegments.Select(x => new
            {
                x.Id,
                Name = $"{x.Code} - {x.Translations.FirstOrDefault(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese).Name}"
            }).ToList().OrderBy(x => x.Name), "Id", "Name", reservation.CarSegmentId);

            ViewBag.InsuranceLevels = new SelectList(_context.InsuranceLevels.Select(x => new
            {
                x.Id,
                x.Translations.FirstOrDefault(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese).Name
            }), "Id", "Name", reservation.InsuranceLevelId);

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
            }), "Id", "Name", reservation.PickupLocationId);

            ViewBag.ReturnLocations = new SelectList(pickupReturnLocations.Select(x => new
            {
                x.Id,
                x.Translations.FirstOrDefault(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese).Name
            }), "Id", "Name", reservation.ReturnLocationId);

            ViewBag.DriverTelehonePrefixCountries = new SelectList(_context.Countries.Select(x => new
            {
                x.Id,
                Text = $"{x.Translations.FirstOrDefault(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese).Name} (+{x.TelephoneCode})"
            }), "Id", "Text", reservation.DriverTelephonePrefixCountryId);

            ViewBag.DriverIdentificationCountries = new SelectList(_context.Countries.Select(x => new
            {
                x.Id,
                x.Translations.FirstOrDefault(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese).Name
            }), "Id", "Name", reservation.DriverIdentityCountryId);

            ViewBag.DriverLicenceCountries = new SelectList(_context.Countries.Select(x => new
            {
                x.Id,
                x.Translations.FirstOrDefault(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese).Name
            }), "Id", "Name", reservation.DriverLicenseCountryId);

            ViewBag.BillTelehonePrefixCountries = new SelectList(_context.Countries.Select(x => new
            {
                x.Id,
                Text = $"+{x.TelephoneCode} ({x.Translations.FirstOrDefault(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese).Name})"
            }), "Id", "Text", reservation.BillTelephonePrefixCountryId);

            ViewBag.BillCountries = new SelectList(_context.Countries.Select(x => new
            {
                x.Id,
                x.Translations.FirstOrDefault(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese).Name
            }), "Id", "Name", reservation.BillCountryId);

            ViewBag.PaymentTypes = new SelectList(dCore.Helpers.Enum.GetWithDescription<Data.Enums.PaymentTypes>().Select(x => new SelectListItem
            {
                Text = x.Value,
                Value = x.Key
            }), "Value", "Text", reservation.PaymentType);

            ViewBag.PaymentStatus = new SelectList(dCore.Helpers.Enum.GetWithDescription<Data.Enums.PaymentStatus>().Select(x => new SelectListItem
            {
                Text = x.Value,
                Value = x.Key
            }), "Value", "Text", reservation.PaymentStatus);

            ViewBag.IdentityTypes = new SelectList(dCore.Helpers.Enum.GetWithDescription<Data.Enums.IdentityType>().Select(x => new SelectListItem
            {
                Text = _translationProvider.GetTranslation((int)Data.Enums.Languages.Portuguese, x.Value),
                Value = x.Key
            }), "Value", "Text", reservation.DriverIdentityType);

            ViewBag.Users = new SelectList(_context.Users.Where(x => !x.Roles.All(y => y.Id == (int)Data.Enums.UserRoles.Customer)).Select(x => new
            {
                x.Id,
                Name = $"{x.FirstName} {x.LastName}"
            }), "Id", "Name", reservation.AssignedUserId);

            ViewBag.Campaigns = new SelectList(_context.Campaigns.Where(x => x.IsUsableForBackoffice).Select(x => new
            {
                x.Id,
                Name = $"{x.Translations.FirstOrDefault(t => t.LanguageId == (int)Data.Enums.Languages.Portuguese).Name}"
            }).ToList().OrderBy(x => x.Name), "Id", "Name", reservation.CampaignId);

            ViewBag.Vouchers = new SelectList(_context.Vouchers.Select(x => new
            {
                x.Id,
                Name = x.Code
            }).ToList().OrderBy(x => x.Name), "Id", "Name", reservation.VoucherId);

            ViewBag.Languages = new SelectList(_context.Languages.Select(x => new
            {
                x.Id,
                x.Name
            }), "Id", "Name", reservation.LanguageId);

            if (reservation.Extras.Any())
            {
                foreach (ReservationExtra reservationExtra in reservation.Extras)
                {
                    reservationExtra.Extra = _context.Extras.Include(x => x.Translations.Where(y => y.LanguageId == (int)Languages.Portuguese)).FirstOrDefault(x => x.Id == reservationExtra.ExtraId);
                }
            }

            foreach (Extra extra in _context.Extras.Where(x => x.IsActive && !reservation.Extras.Select(y => y.ExtraId).Contains(x.Id)).Include(x => x.Translations.Where(y => y.LanguageId == (int)Languages.Portuguese)))
            {
                reservation.Extras.Add(new ReservationExtra()
                {
                    ExtraId = extra.Id,
                    UnitValue = 0,
                    Quantity = 0,
                    Extra = extra
                });
            }

            reservation.Extras = reservation.Extras.OrderBy(x => x.Extra.Translations.First().Name).ToList();

            if (reservation.Services.Any())
            {
                foreach (ReservationService reservationService in reservation.Services)
                {
                    reservationService.Service = _context.Services.Include(x => x.Translations.Where(y => y.LanguageId == (int)Languages.Portuguese)).FirstOrDefault(x => x.Id == reservationService.ServiceId);
                }
            }

            foreach (Service service in _context.Services.Where(x => x.IsActive && !reservation.Services.Select(y => y.ServiceId).Contains(x.Id)).Include(x => x.Translations.Where(y => y.LanguageId == (int)Languages.Portuguese)))
            {
                reservation.Services.Add(new ReservationService()
                {
                    ServiceId = service.Id,
                    Value = 0,
                    Service = service
                });
            }

            reservation.Services = reservation.Services.OrderBy(x => x.Service.Translations.First().Name).ToList();

            //if (!reservation.Extras.Any())
            //{
            //    foreach (Extra extra in _context.Extras.Include(x => x.Translations.Where(y => y.LanguageId == (int)Languages.Portuguese)))
            //    {
            //        reservation.Extras.Add(new ReservationExtra()
            //        {
            //            ExtraId = extra.Id,
            //            UnitValue = 0,
            //            Quantity = 0,
            //            Extra = extra
            //        });
            //    }
            //}
            //else
            //{
            //    foreach (ReservationExtra reservationExtra in reservation.Extras)
            //    {
            //        reservationExtra.Extra = _context.Extras.Include(x => x.Translations.Where(y => y.LanguageId == (int)Languages.Portuguese)).FirstOrDefault(x => x.Id == reservationExtra.ExtraId);
            //    }
            //}

            //if (!reservation.Services.Any())
            //{
            //    foreach (Service service in _context.Services.Include(x => x.Translations.Where(y => y.LanguageId == (int)Languages.Portuguese)))
            //    {
            //        reservation.Services.Add(new ReservationService()
            //        {
            //            ServiceId = service.Id,
            //            Value = 0,
            //            Service = service
            //        });
            //    }
            //}
            //else
            //{
            //    foreach (ReservationService reservationService in reservation.Services)
            //    {
            //        reservationService.Service = _context.Services.Include(x => x.Translations.Where(y => y.LanguageId == (int)Languages.Portuguese)).FirstOrDefault(x => x.Id == reservationService.ServiceId);
            //    }
            //}
        }

        [HttpPost]
        public JsonResult Get(Models.DataTables.ViewModel viewModel)
        {
            JsonResult result = new JsonResult("");
            try
            {
                IQueryable<Reservation> recordsTotal = _context.Reservations
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
                                recordsFiltered = recordsFiltered.Where(x => x.AssignedUserId.HasValue && searchedCollaboratorIds.Contains(x.AssignedUserId.Value));
                                break;
                            case 2:
                                List<Data.Enums.ReservationStatus> searchedReservationStatus = new();
                                foreach (var item in searchedValue.Split('|'))
                                {
                                    searchedReservationStatus.Add(System.Enum.Parse<Data.Enums.ReservationStatus>(item));
                                }
                                recordsFiltered = recordsFiltered.Where(x => searchedReservationStatus.Contains(x.Status));
                                break;
                            case 3:
                                List<Data.Enums.PaymentStatus> searchedPaymentStatus = new();
                                foreach (var item in searchedValue.Split('|'))
                                {
                                    searchedPaymentStatus.Add(System.Enum.Parse<Data.Enums.PaymentStatus>(item));
                                }
                                recordsFiltered = recordsFiltered.Where(x => searchedPaymentStatus.Contains(x.PaymentStatus));
                                break;
                            case 4:
                                recordsFiltered = recordsFiltered.Where(x => x.DriverName.Contains(searchedValue));
                                break;
                            case 5:
                                List<int> searchedPickupLocationIds = new();
                                foreach (var item in searchedValue.Split('|'))
                                {
                                    searchedPickupLocationIds.Add(int.Parse(item));
                                }
                                recordsFiltered = recordsFiltered.Where(x => searchedPickupLocationIds.Contains(x.PickupLocationId));
                                break;
                            case 6:
                                string[] searchedPickupDates = searchedValue.Split('|');
                                if (DateTime.TryParse(searchedPickupDates.ElementAtOrDefault(0), out var from))
                                    recordsFiltered = recordsFiltered.Where(x => x.PickupDateTime.Date >= from);
                                if (DateTime.TryParse(searchedPickupDates.ElementAtOrDefault(1), out var to))
                                    recordsFiltered = recordsFiltered.Where(x => x.PickupDateTime.Date <= to);
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

                if (viewModel.order.Any())
                {
                    if (viewModel.order.Count > 1)
                    {
                        recordsFiltered = recordsFiltered.OrderBy(x => x.AssignedUser.FirstName).ThenBy(x => x.AssignedUser.LastName).ThenByDescending(x => x.PickupDateTime);
                    }
                    else
                    {
                        switch (viewModel.order.First().column)
                        {
                            case 0:
                                recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                    recordsFiltered.OrderBy(x => x.Number) : recordsFiltered.OrderByDescending(x => x.Number);
                                break;
                            case 1:
                                recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                    recordsFiltered.OrderBy(x => x.AssignedUser.FirstName).ThenBy(x => x.AssignedUser.LastName) : recordsFiltered.OrderByDescending(x => x.AssignedUser.FirstName).ThenByDescending(x => x.AssignedUser.LastName);
                                break;
                            case 2:
                                recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                    recordsFiltered.OrderBy(x => x.Status) : recordsFiltered.OrderByDescending(x => x.Status);
                                break;
                            case 3:
                                recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                    recordsFiltered.OrderBy(x => x.PaymentStatus) : recordsFiltered.OrderByDescending(x => x.PaymentStatus);
                                break;
                            case 4:
                                recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                    recordsFiltered.OrderBy(x => x.DriverName) : recordsFiltered.OrderByDescending(x => x.DriverName);
                                break;
                            case 5:
                                recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                    recordsFiltered.OrderBy(x => x.PickupLocation.Translations.FirstOrDefault().Name) : recordsFiltered.OrderByDescending(x => x.PickupLocation.Translations.FirstOrDefault().Name);
                                break;
                            case 6:
                                recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                    recordsFiltered.OrderBy(x => x.PickupDateTime) : recordsFiltered.OrderByDescending(x => x.PickupDateTime);
                                break;
                            case 7:
                                recordsFiltered = (viewModel.order.First().dir == "asc") ?
                                    recordsFiltered.OrderBy(x => x.CreateDate) : recordsFiltered.OrderByDescending(x => x.CreateDate);
                                break;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(viewModel.search.value))
                {
                    var predicate = PredicateBuilder.New<Reservation>();
                    predicate = predicate.Or(x => x.Number.Contains(viewModel.search.value));
                    predicate = predicate.Or(x => x.DriverName.Contains(viewModel.search.value));
                    predicate = predicate.Or(x => x.DriverEmailAddress.Contains(viewModel.search.value));
                    predicate = predicate.Or(x => x.DriverTelephone.Contains(viewModel.search.value));
                    predicate = predicate.Or(x => x.BillName.Contains(viewModel.search.value));
                    predicate = predicate.Or(x => x.BillEmailAddress.Contains(viewModel.search.value));
                    predicate = predicate.Or(x => x.BillTelephone.Contains(viewModel.search.value));
                    predicate = predicate.Or(x => x.BillVatNumber.Contains(viewModel.search.value));
                    predicate = predicate.Or(x => x.Comments.Contains(viewModel.search.value));
                    predicate = predicate.Or(x => x.PickupLocation.Translations.FirstOrDefault().Name.Contains(viewModel.search.value));
                    predicate = predicate.Or(x => x.ReturnLocation.Translations.FirstOrDefault().Name.Contains(viewModel.search.value));
                    predicate = predicate.Or(x => x.Voucher.Code.Contains(viewModel.search.value));
                    predicate = predicate.Or(x => x.CarSegment.Code.Contains(viewModel.search.value));
                    predicate = predicate.Or(x => x.SourceQuotation.Number.Contains(viewModel.search.value));

                    recordsFiltered = recordsFiltered.Where(predicate);
                }
                var recordsFilteredCount = recordsFiltered.Count();
                var recordsFilteredPage = recordsFiltered.Skip(viewModel.start).Take(viewModel.length).Select(x => new AMRent.Data.Models.View.ReservationIndex()
                {
                    Id = x.Id,
                    Number = x.Number,
                    CollaboratorName = x.AssignedUser != null ? $"{x.AssignedUser.FirstName} {x.AssignedUser.LastName}" : null,
                    Status = dCore.Helpers.Enum.GetDescription(x.Status),
                    PaymentStatus = dCore.Helpers.Enum.GetDescription(x.PaymentStatus),
                    DriverName = x.DriverName,
                    PickupLocation = x.PickupLocation.Translations.FirstOrDefault().Name,
                    PickupDateTime = x.PickupDateTime,
                    CreateDateTime = x.CreateDate
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

        // GET: Reservations
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

            var reservationStatus = dCore.Helpers.Enum.GetWithDescription<Data.Enums.ReservationStatus>();

            ViewBag.Status = new SelectList(reservationStatus.Select(x => new SelectListItem
            {
                Text = x.Value,
                Value = x.Key
            }), "Value", "Text");

            var paymentStatus = dCore.Helpers.Enum.GetWithDescription<Data.Enums.PaymentStatus>();

            ViewBag.PaymentStatus = new SelectList(paymentStatus.Select(x => new SelectListItem
            {
                Text = x.Value,
                Value = x.Key
            }), "Value", "Text");

            return View();
        }

        public IActionResult Export(DateTime? startPickupDate, DateTime? endPickupDate, DateTime? startCreateDate, DateTime? endCreateDate)
        {
            // Get the filtered Reservations
            IQueryable<Reservation> reservations = _context.Reservations
                .Include(x => x.AssignedUser)
                .Include(x => x.SourceQuotation)
                .Include(x => x.InsuranceLevel)
                    .ThenInclude(y => y.Translations.Where(y => y.LanguageId == (int)Data.Enums.Languages.Portuguese))
                .Include(x => x.BillCountry)
                    .ThenInclude(y => y.Translations.Where(y => y.LanguageId == (int)Data.Enums.Languages.Portuguese))
                .Include(x => x.DriverIdentityCountry)
                    .ThenInclude(y => y.Translations.Where(y => y.LanguageId == (int)Data.Enums.Languages.Portuguese))
                .Include(x => x.DriverTelephonePrefixCountry)
                .Include(x => x.CarSegment)
                .Include(x => x.Voucher)
                .Include(x => x.Campaign)
                    .ThenInclude(y => y.Translations.Where(y => y.LanguageId == (int)Data.Enums.Languages.Portuguese))
                .Include(x => x.Extras)
                    .ThenInclude(y => y.Extra)
                        .ThenInclude(z => z.Translations.Where(y => y.LanguageId == (int)Data.Enums.Languages.Portuguese))
                .Include(x => x.DataProtectionConsents)
                    .ThenInclude(y => y.DataProtectionConsent)
                        .ThenInclude(z => z.Translations.Where(y => y.LanguageId == (int)Data.Enums.Languages.Portuguese));

            if (startPickupDate.HasValue)
            {
                reservations = reservations.Where(x => x.PickupDateTime >= startPickupDate.Value);
            }
            if (endPickupDate.HasValue)
            {
                endPickupDate = endPickupDate.Value.AddDays(1);
                reservations = reservations.Where(x => x.PickupDateTime <= endPickupDate.Value);
            }
            if (startCreateDate.HasValue)
            {
                reservations = reservations.Where(x => x.CreateDate >= startCreateDate.Value);
            }
            if (endCreateDate.HasValue)
            {
                endCreateDate = endCreateDate.Value.AddDays(1);
                reservations = reservations.Where(x => x.CreateDate <= endCreateDate.Value);
            }

            // Build the CSV
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (StreamWriter streamWriter = new StreamWriter(memoryStream, Encoding.UTF8, leaveOpen: true))
                {
                    using (CsvWriter csvWriter = new CsvWriter(streamWriter, new CsvConfiguration(CultureInfo.InvariantCulture)))
                    {
                        // Header
                        csvWriter.WriteField("Reserva");
                        csvWriter.WriteField("Cotação origem");
                        csvWriter.WriteField("Colaborador");
                        csvWriter.WriteField("Estado");
                        csvWriter.WriteField("Valor");
                        csvWriter.WriteField("Pagamento");
                        csvWriter.WriteField("Voucher");
                        csvWriter.WriteField("Campanha");
                        csvWriter.WriteField("Nome condutor");
                        csvWriter.WriteField("Telefone condutor");
                        csvWriter.WriteField("Email condutor");
                        csvWriter.WriteField("País identificação condutor");
                        csvWriter.WriteField("País faturação");
                        csvWriter.WriteField("Segmento");
                        csvWriter.WriteField("Cobertura");
                        csvWriter.WriteField("Data registo");
                        csvWriter.WriteField("Data levantamento");
                        csvWriter.WriteField("Data devolução");
                        csvWriter.WriteField("Total extras");
                        csvWriter.WriteField("Extras");
                        csvWriter.WriteField("RGPD");
                        csvWriter.NextRecordAsync();

                        // Data
                        foreach (Reservation reservation in reservations.ToList())
                        {
                            csvWriter.WriteField(reservation.Number);
                            csvWriter.WriteField(reservation.SourceQuotation?.Number);
                            csvWriter.WriteField($"{reservation.AssignedUser?.FirstName} {reservation.AssignedUser?.LastName}");
                            csvWriter.WriteField(dCore.Helpers.Enum.GetDescription(reservation.Status));
                            csvWriter.WriteField(reservation.TotalCostOverride ?? reservation.TotalCost);
                            csvWriter.WriteField(dCore.Helpers.Enum.GetDescription(reservation.PaymentStatus));
                            csvWriter.WriteField(reservation.Voucher?.Code);
                            csvWriter.WriteField(reservation.Campaign?.Translations?.First()?.Name);
                            csvWriter.WriteField(reservation.DriverName);
                            csvWriter.WriteField($"{(reservation.DriverTelephonePrefixCountry != null && !string.IsNullOrEmpty(reservation.DriverTelephone) ? $"+{reservation.DriverTelephonePrefixCountry.TelephoneCode} " : "")}{(!string.IsNullOrEmpty(reservation.DriverTelephone) ? reservation.DriverTelephone : "")}");
                            csvWriter.WriteField(reservation.DriverEmailAddress);
                            csvWriter.WriteField(reservation.DriverIdentityCountry?.Translations?.First()?.Name);
                            csvWriter.WriteField(reservation.BillCountry?.Translations?.First()?.Name);
                            csvWriter.WriteField(reservation.CarSegment?.Code);
                            csvWriter.WriteField(reservation.InsuranceLevel?.Translations?.First()?.Name);
                            csvWriter.WriteField(reservation.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"));
                            csvWriter.WriteField(reservation.PickupDateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                            csvWriter.WriteField(reservation.ReturnDateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                            csvWriter.WriteField(reservation.Extras.Sum(x => x.UnitValue * x.Quantity));
                            csvWriter.WriteField(string.Join(',', reservation.Extras.Select(x => x.Extra?.Translations?.FirstOrDefault()?.Name)));
                            List<string> dataProtectionConsents = new();
                            foreach (var consent in reservation.DataProtectionConsents)
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
                var fileName = $"ExportReservas_{DateTime.Now::yyyy-MM-dd_HH_mm_ss}.csv";
                return File(memoryStream.ToArray(), "text/csv", fileName);
            }
        }

        // GET: Reservations/Create
        public IActionResult Create()
        {
            Reservation viewModel = new();
            if (TempData.ContainsKey("ViewModel"))
            {
                viewModel = JsonSerializer.Deserialize<Reservation>(TempData["ViewModel"] as string);
                TempData.Remove("ViewModel");
            }
            else
            {
                viewModel.Status = Data.Enums.ReservationStatus.Registered;
                viewModel.PickupDateTime = DateTime.Now.AddDays(1);
                viewModel.ReturnDateTime = DateTime.Now.AddDays(2);
            }
            BuildViewBag(viewModel, isCreateAction: true);
            return View(nameof(Create), viewModel);
        }

        // POST: Reservations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Reservation reservation, bool isRefresh = false, bool sendEmail = false)
        {
            if (!isRefresh && !ModelState.IsValid)
            {
                BuildViewBag(reservation);
                return View(reservation);
            }

            reservation.CarSegment = _context.CarSegments.IgnoreAutoIncludes()
            .Include(x => x.CarFuel)
            .ThenInclude(x => x.Translations.Where(y => y.LanguageId == (int)Data.Enums.Languages.Portuguese))
            .Include(x => x.Insurances)
            .FirstOrDefault(x => x.Id == reservation.CarSegmentId);

            Shared.Providers.CostCalculator costCalculatorHelper = new Shared.Providers.CostCalculator(_context, reservation.PickupDateTime, reservation.ReturnDateTime);

            reservation.TotalDays = costCalculatorHelper.GetTotalDays();

            reservation.Extras.RemoveAll(x => x.Quantity == 0);
            reservation.Services.RemoveAll(x => x.Value == 0);

            if (reservation.SourceQuotationId.HasValue)
            {
                _context.Quotations.FirstOrDefault(x => x.Id == reservation.SourceQuotationId).Status = Data.Enums.QuotationStatus.Finished;
            }
            else
            {
                costCalculatorHelper.SetSegment(reservation.CarSegmentId);

                if (!reservation.CarSegment.Insurances.Select(x => x.InsuranceLevelId).Contains(reservation.InsuranceLevelId))
                {
                    reservation.InsuranceLevelId = reservation.CarSegment.Insurances.FirstOrDefault().InsuranceLevelId;
                }

                if (reservation.VoucherId.HasValue)
                {
                    string? voucherCode = _context.Vouchers.FirstOrDefault(x => x.Id == reservation.VoucherId)?.Code;

                    if (!string.IsNullOrEmpty(voucherCode))
                    {
                        reservation.VoucherId = costCalculatorHelper.GetVoucherId(voucherCode, extraIds: reservation.Extras.Select(x => x.ExtraId).ToArray(), isBackofficeRequest: true);
                    }
                }

                if (!reservation.VoucherId.HasValue)
                {
                    reservation.CampaignId = costCalculatorHelper.GetCampaignId(extraIds: reservation.Extras.Select(x => x.ExtraId).ToArray(), isBackofficeRequest: true);
                }

                reservation.CarSegmentCost = costCalculatorHelper.GetCarCost() ?? 0;
                reservation.PickupCost = costCalculatorHelper.GetPickupReturnCost(reservation.PickupLocationId);
                reservation.ReturnCost = costCalculatorHelper.GetPickupReturnCost(reservation.ReturnLocationId);
                reservation.InsuranceCost = costCalculatorHelper.GetInsuranceCost(reservation.InsuranceLevelId);
                reservation.InsuranceExcess = costCalculatorHelper.GetInsuranceExcess(reservation.InsuranceLevelId);

                if (reservation.Extras.Any())
                {
                    foreach (KeyValuePair<int, decimal> extraCost in costCalculatorHelper.GetExtrasCost(reservation.Extras?.Select(x => x.ExtraId).ToList(), reservation.InsuranceLevelId))
                    {
                        reservation.Extras.FirstOrDefault(x => x.ExtraId == extraCost.Key).UnitValue = extraCost.Value;
                    }

                    int additionalDriverQuantity = reservation.Extras.Where(x =>
                        _context.Extras.Where(x => x.ExtraType == Data.Enums.ExtraTypes.AdditionalDriver).Select(y => y.Id).Contains(x.ExtraId)).Sum(x => x.Quantity);
                    while (reservation.ExtraDrivers.Count < additionalDriverQuantity)
                    {
                        reservation.ExtraDrivers.Add(new ReservationExtraDriver());
                    }
                    while (reservation.ExtraDrivers.Count > additionalDriverQuantity)
                    {
                        reservation.ExtraDrivers.RemoveAt(reservation.ExtraDrivers.Count - 1);
                    }
                }

                var pickupReturnTemporaryTaxesCosts = costCalculatorHelper.GetPickupReturnTemporaryTaxes();
                reservation.PickupReturnTemporaryTaxes.Clear();

                foreach (var pickupReturnTemporaryTaxCost in pickupReturnTemporaryTaxesCosts)
                {
                    reservation.PickupReturnTemporaryTaxes.Add(new ReservationPickupReturnTemporaryTax()
                    {
                        PickupReturnTemporaryTaxId = pickupReturnTemporaryTaxCost.Key,
                        Quantity = pickupReturnTemporaryTaxCost.Value.Item1,
                        UnitValue = pickupReturnTemporaryTaxCost.Value.Item2,
                        PickupReturnTemporaryTax = _context.PickupReturnTemporaryTaxes.Include(x => x.Translations).FirstOrDefault(x => x.Id == pickupReturnTemporaryTaxCost.Key)
                    });
                }

                reservation.TotalCost = costCalculatorHelper.GetTotalCost(reservation);
            }

            if (!isRefresh && ModelState.IsValid)
            {
                reservation.AssignedUserId = Guid.Parse(User.Claims.First(x => x.Type == ClaimTypes.UserData).Value);

                _context.Add(reservation);
                _context.SaveChanges();

                if (sendEmail)
                {
                    dCore.Communication.Models.SmtpConfiguration smtpConfiguration = _configuration.GetSection("SmtpConfiguration").Get<dCore.Communication.Models.SmtpConfiguration>();
                    Shared.Providers.EmailSender emailSender = new Shared.Providers.EmailSender(_context, _translationProvider, smtpConfiguration, reservation.LanguageId);

                    Data.Enums.EmailContentTypes emailType;

                    switch (reservation.Status)
                    {
                        default:
                        case ReservationStatus.Registered:
                            if (reservation.SourceQuotationId.HasValue)
                            {
                                emailType = Data.Enums.EmailContentTypes.ReservationRegistrationFromQuotation;
                            }
                            else
                            {
                                emailType = Data.Enums.EmailContentTypes.ReservationRegistration;
                            }
                            break;
                        case ReservationStatus.Confirmed:
                            emailType = Data.Enums.EmailContentTypes.ReservationApproval;
                            break;
                        case ReservationStatus.Finished:
                            emailType = Data.Enums.EmailContentTypes.ReservationFinished;
                            break;
                    }

                    emailSender.Send(emailType, _configuration["Environment"] == "Test", reservation.Id);
                }

                return RedirectToAction(nameof(Index));
            }

            BuildViewBag(reservation);
            return View(reservation);
        }

        //// POST: Reservations/CreateRefresh
        //[HttpPost]
        //public async Task<IActionResult> CreateRefresh(Reservation reservation)
        //{
        //    BuildViewBag(reservation, isCreateAction: true);
        //    Shared.Providers.CostCalculator costCalculatorHelper = new Shared.Providers.CostCalculator(_context);

        //    int additionalDriverQuantity = reservation.Extras.Where(x =>
        //        _context.Extras.Where(x => x.ExtraType == Data.Enums.ExtraTypes.AdditionalDriver).Select(y => y.Id).Contains(x.ExtraId)).Sum(x => x.Quantity);

        //    while (reservation.ExtraDrivers.Count < additionalDriverQuantity)
        //    {
        //        reservation.ExtraDrivers.Add(new ReservationExtraDriver());
        //    }
        //    while (reservation.ExtraDrivers.Count > additionalDriverQuantity)
        //    {
        //        reservation.ExtraDrivers.RemoveAt(reservation.ExtraDrivers.Count-1);
        //    }

        //    reservation.CarSegment = await _context.CarSegments.IgnoreAutoIncludes()
        //        .Include(x => x.CarFuel)
        //        .ThenInclude(x => x.Translations.Where(y => y.LanguageId == (int)Data.Enums.Languages.Portuguese))
        //        .Include(x => x.Insurances)
        //        .FirstOrDefaultAsync(x => x.Id == reservation.CarSegmentId);
        //    if (!reservation.CarSegment.Insurances.Select(x => x.InsuranceLevelId).Contains(reservation.InsuranceLevelId))
        //    {
        //        reservation.InsuranceLevelId = reservation.CarSegment.Insurances.FirstOrDefault().InsuranceLevelId;
        //    }
        //    string? voucherCode = null;
        //    if (reservation.VoucherId.HasValue)
        //    {
        //        voucherCode = _context.Vouchers.FirstOrDefault(x => x.Id == reservation.VoucherId)?.Code;
        //    }

        //    Shared.Providers.RentCostResult rentCostResult = costCalculatorHelper.GetRentCost(reservation.PickupDateTime, reservation.ReturnDateTime, reservation.CarSegmentId, voucherCode);
        //    reservation.VoucherId = rentCostResult.VoucherId;
        //    reservation.CampaignId = rentCostResult.CampaignId;
        //    reservation.CarSegmentCost = rentCostResult.RentCost;
        //    reservation.PickupCost = costCalculatorHelper.GetPickupReturnCost(reservation.PickupDateTime, reservation.ReturnDateTime, reservation.PickupLocationId);
        //    reservation.ReturnCost = costCalculatorHelper.GetPickupReturnCost(reservation.PickupDateTime, reservation.ReturnDateTime, reservation.ReturnLocationId);
        //    List<Insurance> insurances = costCalculatorHelper.GetInsuranceCost(reservation.PickupDateTime,
        //        reservation.ReturnDateTime,
        //        _context.CarSegments.AsNoTracking().Include(x => x.Insurances).FirstOrDefault(x => x.Id == reservation.CarSegmentId).Insurances.ToList());
        //    ViewBag.Insurances = insurances;
        //    reservation.InsuranceCost = insurances.FirstOrDefault(x => x.InsuranceLevelId == reservation.InsuranceLevelId).Value;
        //    reservation.InsuranceExcess = insurances.FirstOrDefault(x => x.InsuranceLevelId == reservation.InsuranceLevelId).Excess;
        //    if (reservation.Extras.Any())
        //    {
        //        foreach (KeyValuePair<int, decimal> extraCost in costCalculatorHelper.GetExtraCost(reservation.PickupDateTime, reservation.ReturnDateTime, reservation.Extras.Select(x => x.ExtraId).ToList(), rentCostResult.VoucherId, rentCostResult.CampaignId))
        //        {
        //            reservation.Extras.FirstOrDefault(x => x.ExtraId == extraCost.Key).UnitValue = extraCost.Value * reservation.Extras.FirstOrDefault(x => x.ExtraId == extraCost.Key).Quantity;
        //        }
        //    }

        //    decimal subTotal = reservation.CarSegmentCost + reservation.PickupCost + reservation.ReturnCost;

        //    if (rentCostResult.VoucherId.HasValue)
        //    {
        //        Data.Models.Database.Voucher voucher = _context.Vouchers.FirstOrDefault(x => x.Id == rentCostResult.VoucherId);
        //        switch (voucher.ValueUnit)
        //        {
        //            case Data.Enums.DiscountValueUnits.Percentage:
        //                subTotal = Math.Round((reservation.CarSegmentCost + reservation.PickupCost + reservation.ReturnCost) * ((decimal)(100 - voucher.Value) / 100), 2);
        //                break;
        //            case Data.Enums.DiscountValueUnits.Euro:
        //                subTotal = Math.Round((reservation.CarSegmentCost + reservation.PickupCost + reservation.ReturnCost) - voucher.Value, 2);
        //                break;
        //        }
        //    }
        //    else if (rentCostResult.CampaignId.HasValue)
        //    {
        //        Data.Models.Database.Campaign campaign = _context.Campaigns.Include(x => x.Translations).FirstOrDefault(x => x.Id == rentCostResult.CampaignId);
        //        switch (campaign.ValueUnit)
        //        {
        //            case Data.Enums.DiscountValueUnits.Percentage:
        //                subTotal = Math.Round((reservation.CarSegmentCost + reservation.PickupCost + reservation.ReturnCost) * ((decimal)(100 - campaign.Value) / 100), 2);
        //                break;
        //            case Data.Enums.DiscountValueUnits.Euro:
        //                subTotal = Math.Round((reservation.CarSegmentCost + reservation.PickupCost + reservation.ReturnCost) - campaign.Value, 2);
        //                break;
        //        }
        //    }

        //    reservation.TotalCost = subTotal + +reservation.InsuranceCost + reservation.Extras.Sum(x => x.UnitValue) + reservation.Services.Sum(x => x.Value);

        //    TempData["ViewModel"] = JsonSerializer.Serialize(reservation, new JsonSerializerOptions() { ReferenceHandler = ReferenceHandler.IgnoreCycles });

        //    return Create();
        //}

        // GET: Reservations/CreateFromQuotation
        public IActionResult CreateFromQuotation(int quotationId, int quotationItemId)
        {
            Reservation viewModel = new();

            Quotation sourceQuotation = _context.Quotations
                .Include(x => x.QuotationItems.Where(y => y.Id == quotationItemId))
                    .ThenInclude(x => x.Extras)
                        .ThenInclude(x => x.Extra)
                .Include(x => x.QuotationItems.Where(y => y.Id == quotationItemId))
                    .ThenInclude(x => x.PickupReturnTemporaryTaxes)
                        .ThenInclude(x => x.PickupReturnTemporaryTax)
                .Include(x => x.QuotationItems.Where(y => y.Id == quotationItemId))
                    .ThenInclude(x => x.Services)
                        .ThenInclude(x => x.Service)
                .FirstOrDefault(x => x.Id == quotationId);

            if (sourceQuotation == null || !sourceQuotation.QuotationItems.Any())
            {
                return NotFound();
            }

            Shared.Providers.CostCalculator costCalculatorHelper = new Shared.Providers.CostCalculator(_context, sourceQuotation.PickupDateTime, sourceQuotation.ReturnDateTime);

            viewModel.TotalDays = costCalculatorHelper.GetTotalDays();

            viewModel.SourceQuotationId = quotationId;
            viewModel.Source = sourceQuotation.Source;
            viewModel.Status = Data.Enums.ReservationStatus.Registered;
            viewModel.PickupLocationId = sourceQuotation.PickupLocationId;
            viewModel.PickupDateTime = sourceQuotation.PickupDateTime;
            viewModel.ReturnLocationId = sourceQuotation.ReturnLocationId;
            viewModel.ReturnDateTime = sourceQuotation.ReturnDateTime;
            viewModel.DriverName = sourceQuotation.CustomerName;
            viewModel.BillName = sourceQuotation.CustomerName;
            viewModel.DriverEmailAddress = sourceQuotation.CustomerEmailAddress;
            viewModel.BillEmailAddress = sourceQuotation.CustomerEmailAddress;
            viewModel.DriverTelephonePrefixCountryId = sourceQuotation.CustomerTelephonePrefixCountryId;
            viewModel.BillTelephonePrefixCountryId = sourceQuotation.CustomerTelephonePrefixCountryId;
            viewModel.DriverTelephone = sourceQuotation.CustomerTelephone;
            viewModel.BillTelephone = sourceQuotation.CustomerTelephone;
            viewModel.Comments = sourceQuotation.Comments;
            viewModel.LanguageId = sourceQuotation.LanguageId;

            viewModel.CarSegmentId = sourceQuotation.QuotationItems.First().CarSegmentId;
            viewModel.CampaignId = sourceQuotation.QuotationItems.First().CampaignId;
            viewModel.VoucherId = sourceQuotation.QuotationItems.First().VoucherId;
            viewModel.InsuranceLevelId = sourceQuotation.QuotationItems.First().InsuranceLevelId;
            viewModel.CarSegmentCost = sourceQuotation.QuotationItems.First().CarSegmentCost;
            viewModel.PickupCost = sourceQuotation.QuotationItems.First().PickupCost;
            viewModel.ReturnCost = sourceQuotation.QuotationItems.First().ReturnCost;
            viewModel.InsuranceCost = sourceQuotation.QuotationItems.First().InsuranceCost;
            viewModel.InsuranceExcess = sourceQuotation.QuotationItems.First().InsuranceExcess;
            viewModel.TotalCost = sourceQuotation.QuotationItems.First().TotalCost;
            viewModel.TotalCostOverride = sourceQuotation.QuotationItems.First().TotalCostOverride;

            if (sourceQuotation.QuotationItems.First().Extras.Any())
            {
                foreach (QuotationItemExtra sourceQuotationItemExtra in sourceQuotation.QuotationItems.First().Extras)
                {
                    viewModel.Extras.Add(new ReservationExtra()
                    {
                        ExtraId = sourceQuotationItemExtra.ExtraId,
                        UnitValue = sourceQuotationItemExtra.UnitValue,
                        Quantity = sourceQuotationItemExtra.Quantity
                    });
                    if (sourceQuotationItemExtra.Extra.ExtraType == Data.Enums.ExtraTypes.AdditionalDriver)
                    {
                        for (int i = 0; i < sourceQuotationItemExtra.Quantity; i++)
                        {
                            viewModel.ExtraDrivers.Add(new ReservationExtraDriver());
                        }
                    }
                }
            }

            if (sourceQuotation.QuotationItems.First().Services.Any())
            {
                foreach (QuotationItemService sourceQuotationItemService in sourceQuotation.QuotationItems.First().Services)
                {
                    viewModel.Services.Add(new ReservationService()
                    {
                        ServiceId = sourceQuotationItemService.ServiceId,
                        Value = sourceQuotationItemService.Value
                    });
                }
            }

            if (sourceQuotation.QuotationItems.First().PickupReturnTemporaryTaxes.Any())
            {
                foreach (QuotationItemPickupReturnTemporaryTax sourceQuotationItemPickupReturnTemporaryTax in sourceQuotation.QuotationItems.First().PickupReturnTemporaryTaxes)
                {
                    viewModel.PickupReturnTemporaryTaxes.Add(new ReservationPickupReturnTemporaryTax()
                    {
                        PickupReturnTemporaryTax = _context.PickupReturnTemporaryTaxes.Include(x => x.Translations.Where(y => y.LanguageId == (int)Data.Enums.Languages.Portuguese)).FirstOrDefault(x => x.Id == sourceQuotationItemPickupReturnTemporaryTax.PickupReturnTemporaryTaxId),
                        PickupReturnTemporaryTaxId = sourceQuotationItemPickupReturnTemporaryTax.PickupReturnTemporaryTaxId,
                        UnitValue = sourceQuotationItemPickupReturnTemporaryTax.UnitValue,
                        Quantity = sourceQuotationItemPickupReturnTemporaryTax.Quantity
                    });
                }
            }

            TempData["ViewModel"] = JsonSerializer.Serialize(viewModel, new JsonSerializerOptions() { ReferenceHandler = ReferenceHandler.IgnoreCycles });

            return Create();
        }

        // GET: Reservations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Reservations == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(x => x.CarSegment)
                    .ThenInclude(x => x.CarFuel)
                        .ThenInclude(x => x.Translations.Where(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese))
                .Include(x => x.ExtraDrivers)
                .Include(x => x.Extras)
                    .ThenInclude(x => x.Extra)
                        .ThenInclude(x => x.Translations.Where(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese))
                .Include(x => x.Services)
                    .ThenInclude(x => x.Service)
                        .ThenInclude(x => x.Translations.Where(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese))
                .Include(x => x.PickupReturnTemporaryTaxes)
                    .ThenInclude(x => x.PickupReturnTemporaryTax)
                        .ThenInclude(x => x.Translations.Where(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese))
                .Include(x => x.Changes)
                    .ThenInclude(x => x.User)
                .Include(x => x.DataProtectionConsents)
                    .ThenInclude(x => x.DataProtectionConsent)
                        .ThenInclude(x => x.Translations.Where(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese))
                .Include(x => x.DataProtectionConsents)
                    .ThenInclude(x => x.DataProtectionConsentReservationChanges)
                        .ThenInclude(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == id);

            List<DataProtectionConsent> dataProtectionConsents = _context.DataProtectionConsents
                .Include(x => x.Translations.Where(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese))
                .Where(x => x.IsActive)
                .ToList();
            foreach (var dataProtectionConsent in dataProtectionConsents)
            {
                if (!reservation.DataProtectionConsents.Any(x => x.DataProtectionConsentId == dataProtectionConsent.Id))
                {
                    reservation.DataProtectionConsents.Add(new()
                    {
                        DataProtectionConsentId = dataProtectionConsent.Id,
                        ReservationId = reservation.Id,
                        DataProtectionConsent = dataProtectionConsent,
                    });
                }
            }

            Shared.Providers.CostCalculator costCalculatorHelper = new Shared.Providers.CostCalculator(_context, reservation.PickupDateTime, reservation.ReturnDateTime);
            reservation.TotalDays = costCalculatorHelper.GetTotalDays();

            int additionalDriverQuantity = reservation.Extras.Where(x =>
                _context.Extras.Where(x => x.ExtraType == Data.Enums.ExtraTypes.AdditionalDriver).Select(y => y.Id).Contains(x.ExtraId)).Sum(x => x.Quantity);

            while (reservation.ExtraDrivers.Count < additionalDriverQuantity)
            {
                reservation.ExtraDrivers.Add(new ReservationExtraDriver());
            }
            while (reservation.ExtraDrivers.Count > additionalDriverQuantity)
            {
                reservation.ExtraDrivers.RemoveAt(reservation.ExtraDrivers.Count - 1);
            }

            if (reservation == null)
            {
                return NotFound();
            }

            BuildViewBag(reservation);
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Reservation reservation, TimeOnly pickupTime, TimeOnly returnTime, bool isRefresh = false, bool sendEmail = false)
        {
            if (id != reservation.Id)
            {
                return NotFound();
            }

            DateTime pickupDate = reservation.PickupDateTime;
            DateTime returnDate = reservation.ReturnDateTime;
            reservation.PickupDateTime = new DateTime(pickupDate.Year, pickupDate.Month, pickupDate.Day, pickupTime.Hour, pickupTime.Minute, 0);
            reservation.ReturnDateTime = new DateTime(returnDate.Year, returnDate.Month, returnDate.Day, returnTime.Hour, returnTime.Minute, 0);

            if (!isRefresh && ModelState.IsValid)
            {
                reservation.Extras.RemoveAll(x => x.Quantity == 0);
                reservation.Services.RemoveAll(x => x.Value == 0);
            }

            Shared.Providers.CostCalculator costCalculatorHelper = new Shared.Providers.CostCalculator(_context, reservation.PickupDateTime, reservation.ReturnDateTime);

            reservation.TotalDays = costCalculatorHelper.GetTotalDays();

            costCalculatorHelper.SetSegment(reservation.CarSegmentId);

            if (_context.Reservations.AsNoTracking().First(x => x.Id == reservation.Id).InsuranceLevelId != reservation.InsuranceLevelId)
            {
                reservation.InsuranceCost = costCalculatorHelper.GetInsuranceCost(reservation.InsuranceLevelId);
                reservation.InsuranceExcess = costCalculatorHelper.GetInsuranceExcess(reservation.InsuranceLevelId);
            }

            if (_context.Reservations.AsNoTracking().First(x => x.Id == reservation.Id).PickupLocationId != reservation.PickupLocationId)
            {
                reservation.PickupCost = costCalculatorHelper.GetPickupReturnCost(reservation.PickupLocationId);
            }

            if (_context.Reservations.AsNoTracking().First(x => x.Id == reservation.Id).ReturnLocationId != reservation.ReturnLocationId)
            {
                reservation.PickupCost = costCalculatorHelper.GetPickupReturnCost(reservation.ReturnLocationId);
            }

            _context.ReservationServices.RemoveRange(_context.ReservationServices.Where(x => x.ReservationId == reservation.Id && !reservation.Services.Select(y => y.ServiceId).Contains(x.ServiceId)));
            _context.ReservationExtras.RemoveRange(_context.ReservationExtras.Where(x => x.ReservationId == reservation.Id && !reservation.Extras.Select(y => y.ExtraId).Contains(x.ExtraId)));

            foreach (var reservationExtra in reservation.Extras)
            {
                if (reservationExtra.UnitValue == 0)
                {
                    foreach (KeyValuePair<int, decimal> extraCost in costCalculatorHelper.GetExtrasCost(new List<int> { reservationExtra.ExtraId }, reservation.InsuranceLevelId))
                    {
                        reservationExtra.UnitValue = extraCost.Value;
                    }
                }
            }

            int additionalDriverQuantity = reservation.Extras.Where(x =>
                _context.Extras.Where(x => x.ExtraType == Data.Enums.ExtraTypes.AdditionalDriver).Select(y => y.Id).Contains(x.ExtraId)).Sum(x => x.Quantity);
            while (reservation.ExtraDrivers.Count < additionalDriverQuantity)
            {
                reservation.ExtraDrivers.Add(new ReservationExtraDriver());
                ModelState.AddModelError("", "The Name field is required.");
            }
            while (reservation.ExtraDrivers.Count > additionalDriverQuantity)
            {
                reservation.ExtraDrivers.RemoveAt(reservation.ExtraDrivers.Count - 1);
            }
            _context.ReservationExtraDrivers.RemoveRange(_context.ReservationExtraDrivers.Where(x => x.ReservationId == reservation.Id && !reservation.ExtraDrivers.Select(y => y.Id).Contains(x.Id)));

            reservation.TotalCost = costCalculatorHelper.GetTotalCost(reservation);

            if (!isRefresh)
            {
                if (ModelState.IsValid)
                {
                    Reservation originalReservation = _context.Reservations.AsNoTracking().FirstOrDefault(x => x.Id == id);
                    _context.UpdateWithTracking(reservation, Guid.Parse(User.Claims.First(x => x.Type == ClaimTypes.UserData).Value));
                    await _context.SaveChangesAsync();

                    if (sendEmail)
                    {
                        dCore.Communication.Models.SmtpConfiguration smtpConfiguration = _configuration.GetSection("SmtpConfiguration").Get<dCore.Communication.Models.SmtpConfiguration>();
                        Shared.Providers.EmailSender emailSender = new Shared.Providers.EmailSender(_context, _translationProvider, smtpConfiguration, reservation.LanguageId);
                        //emailSender.Send(Data.Enums.EmailContentTypes.rese, _configuration["Environment"] == "Test", reservation.Id);


                        if (originalReservation.PaymentStatus != reservation.PaymentStatus || originalReservation.Status != reservation.Status)
                        {
                            if (originalReservation.PaymentStatus != reservation.PaymentStatus)
                            {
                                smtpConfiguration = _configuration.GetSection("SmtpConfiguration").Get<dCore.Communication.Models.SmtpConfiguration>();
                                emailSender = new Shared.Providers.EmailSender(_context, _translationProvider, smtpConfiguration, reservation.LanguageId);

                                switch (reservation.PaymentStatus)
                                {
                                    case Data.Enums.PaymentStatus.Paid:
                                        emailSender.Send(Data.Enums.EmailContentTypes.PaymentConfirmation, _configuration["Environment"] == "Test", reservation.Id);
                                        break;
                                    case Data.Enums.PaymentStatus.Failed:
                                        emailSender.Send(Data.Enums.EmailContentTypes.PaymentFailure, _configuration["Environment"] == "Test", reservation.Id);
                                        break;
                                    case Data.Enums.PaymentStatus.Pending:
                                        emailSender.Send(Data.Enums.EmailContentTypes.ReservationRegistration, _configuration["Environment"] == "Test", reservation.Id);
                                        break;
                                }
                            }
                            if (originalReservation.Status != reservation.Status)
                            {
                                if (smtpConfiguration == null || emailSender == null)
                                {
                                    smtpConfiguration = _configuration.GetSection("SmtpConfiguration").Get<dCore.Communication.Models.SmtpConfiguration>();
                                    emailSender = new Shared.Providers.EmailSender(_context, _translationProvider, smtpConfiguration, reservation.LanguageId);
                                }

                                switch (reservation.Status)
                                {
                                    //case Data.Enums.ReservationStatus.Quotation:
                                    //    emailSender.Send(reservation.Id, Data.Enums.EmailContentTypes.QuotationRegistration, _configuration["Environment"] == "Test");
                                    //    break;
                                    case Data.Enums.ReservationStatus.Registered:
                                        emailSender.Send(Data.Enums.EmailContentTypes.ReservationRegistration, _configuration["Environment"] == "Test", reservation.Id);
                                        break;
                                    case Data.Enums.ReservationStatus.Cancelled:
                                        emailSender.Send(Data.Enums.EmailContentTypes.ReservationCancellation, _configuration["Environment"] == "Test", reservation.Id);
                                        break;
                                    case Data.Enums.ReservationStatus.Confirmed:
                                        emailSender.Send(Data.Enums.EmailContentTypes.ReservationApproval, _configuration["Environment"] == "Test", reservation.Id);
                                        break;
                                    case Data.Enums.ReservationStatus.Finished:
                                        emailSender.Send(Data.Enums.EmailContentTypes.ReservationFinished, _configuration["Environment"] == "Test", reservation.Id);
                                        break;
                                }
                            }
                        }
                    }

                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                foreach (var dataProtectionConsent in reservation.DataProtectionConsents)
                {
                    dataProtectionConsent.DataProtectionConsent = _context.DataProtectionConsents
                        .Include(x => x.Translations.Where(y => y.LanguageId == (int)Data.Enums.Languages.Portuguese))
                        .FirstOrDefault(x => x.Id == dataProtectionConsent.DataProtectionConsentId);
                }
            }

            var databaseReservation = await _context.Reservations.AsNoTracking()
                .Include(x => x.CarSegment)
                    .ThenInclude(x => x.CarFuel)
                        .ThenInclude(x => x.Translations.Where(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese))
                .Include(x => x.ExtraDrivers)
                .Include(x => x.Extras)
                    .ThenInclude(x => x.Extra)
                        .ThenInclude(x => x.Translations.Where(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese))
                .Include(x => x.Services)
                    .ThenInclude(x => x.Service)
                        .ThenInclude(x => x.Translations.Where(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese))
                .Include(x => x.Changes)
                    .ThenInclude(x => x.User)
                .Include(x => x.DataProtectionConsents)
                    .ThenInclude(x => x.DataProtectionConsent)
                        .ThenInclude(x => x.Translations.Where(x => x.LanguageId == (int)Data.Enums.Languages.Portuguese))
                .Include(x => x.DataProtectionConsents)
                    .ThenInclude(x => x.DataProtectionConsentReservationChanges)
                .FirstOrDefaultAsync(x => x.Id == id);

            reservation.CarSegment = databaseReservation.CarSegment;
            reservation.Changes = databaseReservation.Changes;

            BuildViewBag(reservation);
            return View(reservation);
        }

        private bool ReservationExists(int id)
        {
            return (_context.Reservations?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
