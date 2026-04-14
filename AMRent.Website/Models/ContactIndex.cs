namespace AMRent.Website.Models
{
	public class ContactIndex
    {
        public List<Data.Models.Database.OfficeLocation> OfficeLocations { get; set; } = new();
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactTelephoneNumber { get; set; }
        public string ContactMessage { get; set; }
    }
}
