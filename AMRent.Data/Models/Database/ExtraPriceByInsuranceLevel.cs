using System.ComponentModel.DataAnnotations.Schema;

namespace AMRent.Data.Models.Database
{
    public class ExtraPriceByInsuranceLevel
    {
        [Column(TypeName = "decimal(7, 2)")]
        public decimal Value { get; set; }
        [Column(TypeName = "decimal(7, 2)")]
        public decimal? MaximumValue { get; set; }

        public int ExtraId { get; set; }
        public Extra? Extra { get; set; }
        public int InsuranceLevelId { get; set; }
        public InsuranceLevel? InsuranceLevel { get; set; }
    }
}
