using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AMRent.Data.Contexts;
using AMRent.Data.Models.Database;

namespace AMRent.Website.ViewComponents
{
    public class _LayoutSocialNetworksViewComponent : ViewComponent
    {
        internal readonly FullDatabaseContext _context;

        public _LayoutSocialNetworksViewComponent(FullDatabaseContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<SocialNetwork>? userData = await _context.SocialNetworks.OrderBy(x => x.Id).ToListAsync();

            return View("_LayoutSocialNetworks", userData);
        }
    }
}