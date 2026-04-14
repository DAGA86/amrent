namespace AMRent.Data.Models.View
{
    public class PickupReturnLocationIndex
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public bool IsSelectedByDefault { get; set; }
        public bool IsWorkingOffice { get; set; }
    }
}
