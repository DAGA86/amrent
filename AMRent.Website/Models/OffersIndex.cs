using dCore.MultiLanguage.Models;

namespace AMRent.Website.Models
{
    public class OffersIndex
    {
		public List<Data.Models.Database.Campaign> Campaigns { get; set; } = new();
		//public Language SelectedLanguage { get; set; }
	}
}
