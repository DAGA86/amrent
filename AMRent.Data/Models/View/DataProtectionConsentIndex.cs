namespace AMRent.Data.Models.View
{
    public class DataProtectionConsentIndex
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public bool IsActive { get; set; }
        public int SortNumber { get; set; }
        public bool IsLowestSortNumber { get; set; } = false;
        public bool IsHighestSortNumber { get; set; } = false;
    }
}
