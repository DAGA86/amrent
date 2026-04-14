using System.ComponentModel.DataAnnotations;
using dCore.Helpers.DataAnnotations;

namespace AMRent.Data.Models.Database
{
    public class DayOfWeekSchedule
    {
        [Required]
        public Enums.DaysOfWeek DayOfWeek { get; set; }
        [RequiredIfFalse(nameof(IsClosed))]
        [TimeSpanLowerThanIfNotNull(nameof(LunchBreakStartTime), nameof(LunchBreakEndTime), nameof(ClosingTime))]
        public TimeSpan? OpeningTime { get; set; }
        [RequiredIfFalse(nameof(IsClosed))]
        [TimeSpanHigherThanIfNotNull(nameof(OpeningTime), nameof(LunchBreakStartTime), nameof(LunchBreakEndTime))]
        public TimeSpan? ClosingTime { get; set; }
        [RequiredIfHasValue(nameof(LunchBreakEndTime))]
        [TimeSpanHigherThanIfNotNull(nameof(OpeningTime))]
        [TimeSpanLowerThanIfNotNull(nameof(LunchBreakEndTime), nameof(ClosingTime))]
        public TimeSpan? LunchBreakStartTime { get; set; }
        [RequiredIfHasValue(nameof(LunchBreakStartTime))]
        [TimeSpanHigherThanIfNotNull(nameof(OpeningTime), nameof(LunchBreakStartTime))]
        [TimeSpanLowerThanIfNotNull(nameof(ClosingTime))]
        public TimeSpan? LunchBreakEndTime { get; set; }
        public bool IsClosed { get; set; }
    }
}
