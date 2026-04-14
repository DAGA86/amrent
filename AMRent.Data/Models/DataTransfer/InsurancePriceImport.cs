using System.ComponentModel.DataAnnotations.Schema;

namespace AMRent.Data.Models.DataTransfer
{
    public class InsurancePriceImport
    {
        public int InsuranceLevelId { get; set; }
        public int CarSegmentId { get; set; }
        public int Days { get; set; }
        [Column(TypeName = "decimal(7, 2)")]
        public decimal Value { get; set; }
    }
}
