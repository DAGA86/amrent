using System.ComponentModel.DataAnnotations.Schema;

namespace AMRent.Data.Models.Database
{
    public class QuotationItemPickupReturnTemporaryTax
    {
        public int QuotationItemId { get; set; }
        public int PickupReturnTemporaryTaxId { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(7, 2)")]
        public decimal UnitValue { get; set; }

        public QuotationItem? QuotationItem { get; set; }
        public PickupReturnTemporaryTax? PickupReturnTemporaryTax { get; set; }
    }
}
