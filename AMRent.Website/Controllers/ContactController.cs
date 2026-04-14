using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AMRent.Data.Contexts;
using System.Web;

namespace AMRent.Website.Controllers
{
    public class ContactController : BaseController
    {
        private readonly IConfiguration _configuration;

        public ContactController(ILogger<HomeController> logger, FullDatabaseContext context, dCore.MultiLanguage.Providers.TranslationProvider translationProvider, IConfiguration configuration) : base(logger, context, translationProvider)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            var viewModel = new Models.ContactIndex()
            {
                OfficeLocations = _context.OfficeLocations.Include(x => x.Translations.Where(t => t.LanguageId == GetSelectedLanguageId())).ToList(),
            };
            BuildViewBag();

            return View(viewModel);
        }

        [HttpPost]
        [Filters.PassAlongQueryParamertersFilter()]
        public async Task<IActionResult> Contact(Models.ContactIndex model)
        {
            dCore.Communication.Models.SmtpConfiguration smtpConfiguration = _configuration.GetSection("SmtpConfiguration").Get<dCore.Communication.Models.SmtpConfiguration>();

            dCore.Communication.Providers.Email emailProvider = new dCore.Communication.Providers.Email();

            List<string> recipientEmailAddresses = new List<string>();
            string[] contactDestinationAddresses = _configuration["Notifications:Email:ContactDestinationAddresses"].Split(',');
            string subject = "AMRent - Contacto";
            string content = "<html style='width: 100%' width='100%'><body style='width: 100%' width='100%'>";

            content += $"<p>Nome: {model.ContactName}</p>";
            content += $"<p>Email: {model.ContactEmail}</p>";
            content += $"<p>Telefone: {model.ContactTelephoneNumber}</p>";

            foreach (string line in model.ContactMessage.Split("\n"))
            {
                content += $"<p>{HttpUtility.HtmlEncode(line)}</p>";
            }
            content += "</body></html>";

            await emailProvider.Send(contactDestinationAddresses, subject, content, smtpConfiguration);
            return RedirectToAction(nameof(Index));
        }
    }
}