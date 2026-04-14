namespace AMRent.Data.Models.Database
{
    public class InsuranceLevel
	{
		public int Id { get; set; }

        public List<Insurance>? Insurances { get; set; } = new();
        public List<InsuranceLevelTranslation>? Translations { get; set; } = new();
        public ICollection<ExtraPriceByInsuranceLevel>? ExtraPricesByInsuranceLevel { get; set; } = new List<ExtraPriceByInsuranceLevel>();
    }
}
