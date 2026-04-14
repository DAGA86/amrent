using dCore.MultiLanguage.Models;

namespace AMRent.Website.Models
{
	public class HomeIndex
    {
        public Language? SelectedLanguage { get; set; }

        public int PickupLocationId { get; set; }
        public int ReturnLocationId { get; set; }
        public DateTime PickupDateTime { get; set; }
        public DateTime ReturnDateTime { get; set; }

        public List<Data.Models.Database.Campaign> Campaigns { get; set; } = new();
        public List<Data.Models.Database.Process> Processes { get; set; } = new();
        public List<Data.Models.Database.Fact> Facts { get; set; } = new();
        public List<Data.Models.Database.ReasonToChooseUs> ReasonsToChooseUs { get; set; } = new();
        public List<Data.Models.Database.CarSegment> MostRented { get; set; } = new();
        public List<Data.Models.Database.Testimonial> Testimonials { get; set; } = new();
        public List<Data.Models.Database.HomeBanner> HomeBanners { get; set; } = new();
    }
}
