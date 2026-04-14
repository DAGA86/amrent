namespace AMRent.Data.Models.Database
{
    public class CarGearbox
    {
        public int Id { get; set; }

        public ICollection<CarSegment>? CarSegments { get; set; }
        public ICollection<CarGearboxTranslation>? Translations { get; set; } = new List<CarGearboxTranslation>();
    }
}
