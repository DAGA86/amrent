namespace AMRent.Website.Models
{
    public class SearchIndex
    {
        // Dynamic Filters
        public List<int> SelectedCategoryIds { get; set; } = new();
        public List<int> SelectedFuelIds { get; set; } = new();
        public List<int> SelectedGearboxIds { get; set; } = new();
        public List<int> SelectedSeatNumbers { get; set; } = new();
        public List<int> SelectedCampaignIds { get; set; } = new();
        public Dictionary<int, string?> CarCategories { get; set; } = new();
        public Dictionary<int, string?> CarFuels { get; set; } = new();
        public Dictionary<int, string?> CarGearboxes { get; set; } = new();
        public List<int> CarSeats { get; set; } = new();
        public Dictionary<int, string?> Campaigns { get; set; } = new();

        // Fixed Filters
        public int PickupLocationId { get; set; }
        public DateTime PickupDateTime { get; set; }
        public int ReturnLocationId { get; set; }
        public DateTime ReturnDateTime { get; set; }

        // Data
        public List<Data.Models.Database.CarSegment> CarSegments { get; set; }
        public Dictionary<int, decimal> Prices { get; set; } = new();
        public Dictionary<int, string> CarSegmentCampaigns { get; set; } = new();
        public int Days { get; set; }

        // Other
        public string? ViewLayout { get; set; } = "Grid";
        public string? Anchor { get; set; }
        public bool PickupReturnValuesAdjusted { get; set; } = false;
    }
}
