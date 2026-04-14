namespace AMRent.Data.Models.Database
{
    public class CarCategory
    {
        public int Id { get; set; }
        public bool IsCommercial { get; set; }

        public ICollection<CarSegment>? CarSegments { get; set; }
        public ICollection<CarCategoryTranslation>? Translations { get; set; } = new List<CarCategoryTranslation>();
    }
}
