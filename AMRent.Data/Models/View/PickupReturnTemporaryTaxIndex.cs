namespace AMRent.Data.Models.View
{
    public class PickupReturnTemporaryTaxIndex
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }

        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }

        public decimal Value { get; set; }
    }
}
