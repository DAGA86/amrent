namespace AMRent.Data.Models.Database
{
    public class PickupReturnLocationDayOfWeekSchedule: DayOfWeekSchedule
    {
        public int PickupReturnLocationId { get; set; }

        public PickupReturnLocation? PickupReturnLocation { get; set; }
    }
}
