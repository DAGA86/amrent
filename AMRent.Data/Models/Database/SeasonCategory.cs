namespace AMRent.Data.Models.Database
{
    public class SeasonCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Season>? Seasons { get; set; }
    }
}
