using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AMRent.Data.Contexts;
using System.Diagnostics;

namespace AMRent.BackOffice.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;

        public HomeController(FullDatabaseContext context, ILogger<HomeBannersController> logger, IWebHostEnvironment webHostEnvironment, IConfiguration configuration) : base(context, logger)
        {
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            Models.Dashboard viewModel = new Models.Dashboard();

            var startDate = new DateTime(DateTime.UtcNow.AddMonths(-11).Year, DateTime.UtcNow.AddMonths(-11).Month, 1);

            var startMonth = startDate;
            while (startMonth <= new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1))
            {
                viewModel.FinishedReservationsPerMonthByCreateDate.Add(new Models.Reservations
                {
                    Year = startMonth.Year,
                    Month = startMonth.Month
                });
                viewModel.CancelledReservationsPerMonthByCreateDate.Add(new Models.Reservations
                {
                    Year = startMonth.Year,
                    Month = startMonth.Month
                });
                viewModel.ReservationsPerMonthByPickupDate.Add(new Models.Reservations
                {
                    Year = startMonth.Year,
                    Month = startMonth.Month
                });
                startMonth = startMonth.AddMonths(1);
            }
            while (startMonth <= new DateTime(DateTime.UtcNow.AddYears(1).Year, DateTime.UtcNow.AddYears(1).Month, 1))
            {
                viewModel.ReservationsPerMonthByPickupDate.Add(new Models.Reservations
                {
                    Year = startMonth.Year,
                    Month = startMonth.Month
                });
                startMonth = startMonth.AddMonths(1);
            }

            var finishedReservationsPerMonthByCreateDate = _context.Reservations
                .Where(r => r.CreateDate >= startDate && r.Status == Data.Enums.ReservationStatus.Finished)
                .GroupBy(r => new { r.CreateDate.Year, r.CreateDate.Month })
                .Select(x => new Models.Reservations
                {
                    Year = x.Key.Year,
                    Month = x.Key.Month,
                    Count = x.Count()
                })
                .OrderBy(g => g.Year)
                .ThenBy(g => g.Month)
                .ToList();

            viewModel.FinishedReservationsPerMonthByCreateDate = viewModel.FinishedReservationsPerMonthByCreateDate
                .Join(finishedReservationsPerMonthByCreateDate, f => new { f.Year, f.Month }, r => new { r.Year, r.Month }, (f, r) =>
                {
                    f.Count = r.Count;
                    return f;
                })
                .ToList();

            var cancelledReservationsPerMonthByCreateDate = _context.Reservations
                .Where(r => r.CreateDate >= startDate && r.Status == Data.Enums.ReservationStatus.Cancelled)
                .GroupBy(r => new { r.CreateDate.Year, r.CreateDate.Month })
                .Select(x => new Models.Reservations
                {
                    Year = x.Key.Year,
                    Month = x.Key.Month,
                    Count = x.Count()
                })
                .OrderBy(g => g.Year)
                .ThenBy(g => g.Month)
                .ToList();
            viewModel.CancelledReservationsPerMonthByCreateDate = viewModel.CancelledReservationsPerMonthByCreateDate
                .Join(cancelledReservationsPerMonthByCreateDate, f => new { f.Year, f.Month }, r => new { r.Year, r.Month }, (f, r) =>
                {
                    f.Count = r.Count;
                    return f;
                })
                .ToList();

            var reservationsPerMonthByPickupDate = _context.Reservations
                .Where(r => r.PickupDateTime >= startDate && r.Status != Data.Enums.ReservationStatus.Cancelled)
                .GroupBy(r => new { r.PickupDateTime.Year, r.PickupDateTime.Month })
                .Select(x => new Models.Reservations
                {
                    Year = x.Key.Year,
                    Month = x.Key.Month,
                    Count = x.Count()
                })
                .OrderBy(g => g.Year)
                .ThenBy(g => g.Month)
                .ToList();
            viewModel.ReservationsPerMonthByPickupDate = viewModel.ReservationsPerMonthByPickupDate
                .GroupJoin(reservationsPerMonthByPickupDate, f => new { f.Year, f.Month }, r => new { r.Year, r.Month }, (f, r) =>
                {
                    f.Count = r.FirstOrDefault()?.Count ?? 0;
                    return f;
                })
                .ToList();

            return View(viewModel);
        }

        [AllowAnonymous]
        public IActionResult Forbidden()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new Models.ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}