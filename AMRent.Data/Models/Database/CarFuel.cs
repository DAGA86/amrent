namespace AMRent.Data.Models.Database
{
    public class CarFuel
    {
        public int Id { get; set; }

        public ICollection<CarSegment>? CarSegments { get; set; }
        public ICollection<CarFuelTranslation>? Translations { get; set; } = new List<CarFuelTranslation>();
    }
}
