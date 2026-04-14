using System.ComponentModel.DataAnnotations.Schema;

namespace AMRent.Data.Models.Database
{
    public class Insurance
	{
        public int Id { get; set; }

		[Column(TypeName = "decimal(7, 2)")]
		public decimal Excess { get; set; }
		[Column(TypeName = "decimal(7, 2)")]
		public decimal? MinimumValue { get; set; }
		[Column(TypeName = "decimal(7, 2)")]
		public decimal? MaximumValue { get; set; }

        public int InsuranceLevelId { get; set; }
        public InsuranceLevel? InsuranceLevel { get; set; }
        public int CarSegmentId { get; set; }
        public CarSegment? CarSegment { get; set; }

        public List<InsuranceChange>? Changes { get; set; } = new();
        public List<InsurancePrice>? Prices { get; set; } = new();
    }
}
