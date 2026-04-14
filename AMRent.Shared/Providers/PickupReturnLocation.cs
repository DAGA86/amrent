namespace AMRent.Shared.Providers
{
    public class PickupReturnLocation
    {
        private readonly Data.Contexts.FullDatabaseContext _context;

        public PickupReturnLocation(Data.Contexts.FullDatabaseContext context)
        {
            _context = context;
        }

        public bool IsAvailableAtDateTime(int pickupReturnLocationId, DateTime checkedDateTime)
        {
            if (pickupReturnLocationId == -1)
                return true;

            bool result = false;
            Data.Enums.DaysOfWeek dayOfWeek = DaysOfWeekConverter(checkedDateTime.DayOfWeek);
            Data.Models.Database.PickupReturnLocationDayOfWeekSchedule? dayOfWeekSchedule =
                _context.PickupReturnLocationDayOfWeekSchedules.FirstOrDefault(x =>
                    x.PickupReturnLocationId == pickupReturnLocationId && x.DayOfWeek == dayOfWeek);

            if (dayOfWeekSchedule == null)
                throw new ApplicationException(
                    $"Missing schedule configuration for the PickupReturnLocation with Id {pickupReturnLocationId}");

            if (!dayOfWeekSchedule.IsClosed)
            {
                if (checkedDateTime.TimeOfDay >= dayOfWeekSchedule.OpeningTime && checkedDateTime.TimeOfDay <= dayOfWeekSchedule.ClosingTime)
                {
                    if (dayOfWeekSchedule.LunchBreakStartTime.HasValue && dayOfWeekSchedule.LunchBreakEndTime.HasValue)
                    {
                        if (checkedDateTime.TimeOfDay <= dayOfWeekSchedule.LunchBreakStartTime && checkedDateTime.TimeOfDay >= dayOfWeekSchedule.LunchBreakEndTime)
                        {
                            result = true;
                        }
                    }
                    else
                    {
                        result = true;
                    }
                }
            }

            return result;
        }

        public DateTime GetNextAvailableDateTime(int pickupReturnLocationId, DateTime requestedDateTime)
        {
            DateTime? result = null;

            Data.Models.Database.PickupReturnLocation pickupReturnLocation = _context.PickupReturnLocations.FirstOrDefault(x => x.Id == pickupReturnLocationId);
            if (pickupReturnLocation != null && pickupReturnLocation.IsAlwaysAvailableForPickupAndReturn)
            {
                result = requestedDateTime;
            }

            for (int i = 0; i < 7; i++)
            {
                Data.Enums.DaysOfWeek dayOfWeek = DaysOfWeekConverter(requestedDateTime.DayOfWeek);
                Data.Models.Database.PickupReturnLocationDayOfWeekSchedule? dayOfWeekSchedule =
                    _context.PickupReturnLocationDayOfWeekSchedules.FirstOrDefault(x =>
                        x.PickupReturnLocationId == pickupReturnLocationId && x.DayOfWeek == dayOfWeek);

                if (dayOfWeekSchedule == null)
                    throw new ApplicationException(
                        $"Missing schedule configuration for the PickupReturnLocation with Id {pickupReturnLocationId}");

                if (!dayOfWeekSchedule.IsClosed)
                {
                    if (requestedDateTime.TimeOfDay >= dayOfWeekSchedule.OpeningTime && requestedDateTime.TimeOfDay <= dayOfWeekSchedule.ClosingTime)
                    {
                        if (dayOfWeekSchedule.LunchBreakStartTime.HasValue && dayOfWeekSchedule.LunchBreakEndTime.HasValue)
                        {
                            if (requestedDateTime.TimeOfDay <= dayOfWeekSchedule.LunchBreakStartTime || requestedDateTime.TimeOfDay >= dayOfWeekSchedule.LunchBreakEndTime)
                            {
                                result = requestedDateTime;
                            }
                            else
                            {
                                result = dCore.Helpers.DateTime.GetNewBasedOn(requestedDateTime, dayOfWeekSchedule.LunchBreakEndTime.Value, withZeroSeconds: true);
                            }
                        }
                        else
                        {
                            result = requestedDateTime;
                        }
                    }
                    else
                    {
                        if (requestedDateTime.TimeOfDay < dayOfWeekSchedule.OpeningTime)
                        {
                            result = dCore.Helpers.DateTime.GetNewBasedOn(requestedDateTime, dayOfWeekSchedule.OpeningTime.Value, withZeroSeconds: true);
                        }
                    }
                }

                if (!result.HasValue)
                    requestedDateTime = dCore.Helpers.DateTime.GetNewBasedOn(requestedDateTime.AddDays(1), new TimeSpan(0));
                else
                    break;
            }

            if (!result.HasValue)
                throw new ApplicationException($"Incorrect schedule configuration for the PickupReturnLocation with Id {pickupReturnLocationId}");
            
            return result.Value;
        }

        public DateTime GetNextCompliantWithAnticipationDateTime(int pickupReturnLocationId, DateTime requestedDateTime)
        {
            Data.Models.Database.PickupReturnLocation? pickupReturnLocation = _context.PickupReturnLocations
                .FirstOrDefault(x => x.Id == pickupReturnLocationId);
            int? workingOfficePickupReturnLocationId = _context.PickupReturnLocations.FirstOrDefault(x => x.IsWorkingOffice)?.Id;

            if (pickupReturnLocation == null)
                throw new ApplicationException($"Invalid PickupReturnLocation Id {pickupReturnLocationId}");
            if (!workingOfficePickupReturnLocationId.HasValue)
                throw new ApplicationException("Missing working office PickupReturnLocation");

            DateTime defaultPickupReturnLocationNextAvailability = 
                GetNextAvailableDateTime(workingOfficePickupReturnLocationId.Value, dCore.Helpers.DateTime.RoundToNearestFutureQuarterHour(DateTime.Now).AddMinutes(pickupReturnLocation.MinimumAnticipationMinutes));
            
            if (defaultPickupReturnLocationNextAvailability > requestedDateTime)
                return GetNextAvailableDateTime(pickupReturnLocationId, defaultPickupReturnLocationNextAvailability);

            return GetNextAvailableDateTime(pickupReturnLocationId, requestedDateTime);
        }

        private Data.Enums.DaysOfWeek DaysOfWeekConverter(DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Monday:
                    return Data.Enums.DaysOfWeek.Monday;
                case DayOfWeek.Tuesday:
                    return Data.Enums.DaysOfWeek.Tuesday;
                case DayOfWeek.Wednesday:
                    return Data.Enums.DaysOfWeek.Wednesday;
                case DayOfWeek.Thursday:
                    return Data.Enums.DaysOfWeek.Thursday;
                case DayOfWeek.Friday:
                    return Data.Enums.DaysOfWeek.Friday;
                case DayOfWeek.Saturday:
                    return Data.Enums.DaysOfWeek.Saturday;
                case DayOfWeek.Sunday:
                    return Data.Enums.DaysOfWeek.Sunday;
            }

            throw new ArgumentException("Invalid parameter");
        }
    }
}
