namespace AMRent.Data.Models.Database
{
    public class Season
    {
        public int Id { get; set; }
        public DateTime StartDateUtc { get; set; }
        public DateTime EndDateUtc { get; set; }
        public int SeasonCategoryId { get; set; }

        public SeasonCategory? SeasonCategory { get; set; }

        public ICollection<Price>? Prices { get; set; } = [];
        public List<SeasonChange>? Changes { get; set; } = new();
    }
}
