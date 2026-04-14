using System.ComponentModel.DataAnnotations.Schema;

namespace AMRent.Data.Models.Database
{
    public class QuotationItemExtra
    {
        public int QuotationItemId { get; set; }
        public int ExtraId { get; set; }
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(7, 2)")]
        public decimal UnitValue { get; set; }

        public QuotationItem? QuotationItem { get; set; }
        public Extra? Extra { get; set; }
    }
}
