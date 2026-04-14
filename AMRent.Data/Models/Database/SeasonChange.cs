namespace AMRent.Data.Models.Database
{
    public class SeasonChange : Base.EntityChange
    {
        public int SeasonId { get; set; }
        public Season Season { get; set; }
    }
}
