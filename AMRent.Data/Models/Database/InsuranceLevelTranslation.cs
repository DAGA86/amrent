using System.ComponentModel.DataAnnotations;

namespace AMRent.Data.Models.Database
{
    public class InsuranceLevelTranslation : dCore.MultiLanguage.Models.Translation
    {
        public int InsuranceLevelId { get; set; }
        [StringLength(64)]
        public string Name { get; set; }
        public string? Included { get; set; }
        public string? Excluded { get; set; }

        public InsuranceLevel? InsuranceLevel { get; set; }
    }
}
