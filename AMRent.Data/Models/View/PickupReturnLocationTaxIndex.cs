namespace AMRent.Data.Models.View
{
    public class PickupReturnLocationTaxIndex
    {
        public int Id { get; set; }
        public string PickupReturnLocationName { get; set; }
        public int Days { get; set; }
        public decimal Value { get; set; }
    }
}
