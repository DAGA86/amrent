namespace AMRent.BackOffice.Models
{
    public class PriceExportViewModel
    {
        public string SeasonCategoryName { get; set; }
        public int Days { get; set; }
        public Dictionary<string, decimal> CarSegments { get; set; }
    }
}
