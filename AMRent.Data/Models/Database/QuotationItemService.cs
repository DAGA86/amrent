using System.ComponentModel.DataAnnotations.Schema;

namespace AMRent.Data.Models.Database
{
    public class QuotationItemService
    {
        public int QuotationItemId { get; set; }
        public int ServiceId { get; set; }

        [Column(TypeName = "decimal(7, 2)")]
        public decimal Value { get; set; }

        public QuotationItem? QuotationItem { get; set; }
        public Service? Service { get; set; }
    }
}
