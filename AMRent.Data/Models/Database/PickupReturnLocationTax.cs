using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AMRent.Data.Models.Database
{
    public class PickupReturnLocationTax
    {
        public int Id { get; set; }
        public int PickupReturnLocationId { get; set; }
        [Range(0, int.MaxValue)]
        public int Days { get; set; }
        [Column(TypeName = "decimal(7, 2)")]
        public decimal Value { get; set; }

        public PickupReturnLocation? PickupReturnLocation { get; set; }
        public List<PickupReturnLocationTaxChange>? Changes { get; set; } = new();
    }
}
