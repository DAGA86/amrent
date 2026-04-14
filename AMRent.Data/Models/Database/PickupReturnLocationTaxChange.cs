namespace AMRent.Data.Models.Database
{
    public class PickupReturnLocationTaxChange : Base.EntityChange
    {
        public int PickupReturnLocationTaxId { get; set; }
        public PickupReturnLocationTax PickupReturnLocationTax { get; set; }
    }
}
