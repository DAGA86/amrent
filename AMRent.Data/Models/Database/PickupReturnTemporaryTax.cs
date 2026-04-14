using System.ComponentModel.DataAnnotations.Schema;

namespace AMRent.Data.Models.Database
{
    public class PickupReturnTemporaryTax
    {
        public int Id { get; set; }

        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }

        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }

        [Column(TypeName = "decimal(7, 2)")]
        public decimal Value { get; set; }

        public List<PickupReturnTemporaryTaxChange>? Changes { get; set; } = new();
        public List<PickupReturnTemporaryTaxTranslation>? Translations { get; set; } = new List<PickupReturnTemporaryTaxTranslation>();
    }
}
