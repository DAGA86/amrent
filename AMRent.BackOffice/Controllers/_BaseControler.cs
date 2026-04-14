using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AMRent.Data.Contexts;

namespace AMRent.BackOffice.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        internal readonly ILogger<BaseController> _logger;
        internal readonly FullDatabaseContext _context;

        public BaseController(FullDatabaseContext context, ILogger<BaseController> logger)
        {
            _context = context;
            _logger = logger;
        }
    }
}
