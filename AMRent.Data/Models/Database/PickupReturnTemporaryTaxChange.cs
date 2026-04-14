namespace AMRent.Data.Models.Database
{
    public class PickupReturnTemporaryTaxChange : Base.EntityChange
    {
        public int PickupReturnTemporaryTaxId { get; set; }
        public PickupReturnTemporaryTax PickupReturnTemporaryTax { get; set; }
    }
}
