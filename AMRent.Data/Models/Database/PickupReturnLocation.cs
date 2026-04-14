using System.ComponentModel.DataAnnotations;

namespace AMRent.Data.Models.Database
{
    public class PickupReturnLocation
    {
        public int Id { get; set; }
        public bool IsSelectedByDefault { get; set; } = false;
        public bool IsWorkingOffice { get; set; } = false;
        public int MinimumAnticipationMinutes { get; set; }
        public bool IsAlwaysAvailableForPickupAndReturn { get; set; } = false;

        public ICollection<PickupReturnLocationTranslation>? Translations { get; set; } = new List<PickupReturnLocationTranslation>();
        public ICollection<PickupReturnLocationTax>? Taxes { get; set; } = new List<PickupReturnLocationTax>();
        [EnsureSevenItems(ErrorMessage = "DayOfWeekSchedules must have 7 items.")]
        public List<PickupReturnLocationDayOfWeekSchedule>? DayOfWeekSchedules { get; set; } = new List<PickupReturnLocationDayOfWeekSchedule>();
        public ICollection<Reservation>? PickupReservations { get; set; } = new List<Reservation>();
        public ICollection<Reservation>? ReturnReservations { get; set; } = new List<Reservation>();
        public ICollection<Quotation>? PickupQuotations { get; set; } = new List<Quotation>();
        public ICollection<Quotation>? ReturnQuotations { get; set; } = new List<Quotation>();
    }

    public class EnsureSevenItemsAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is ICollection<PickupReturnLocationDayOfWeekSchedule> collection)
            {
                return collection.Count == 7;
            }
            return false;
        }
    }
}
