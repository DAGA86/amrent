using AMRent.Data.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace AMRent.Shared.Providers
{
    //public class CostResult
    //{
    //    public decimal PickupCost { get; set; } = 0;
    //    public decimal ReturnCost { get; set; } = 0;
    //    public decimal CarCost { get; set; } = 0;
    //    public Dictionary<int, decimal> ExtraCost { get; set; } = new();
    //    public decimal InsuranceCost { get; set; }
    //    public decimal InsuranceExcess { get; set; }
    //    public decimal Total { get; set; }
    //    public decimal? Discount { get; set; }

    //    public int? CampaignId { get; set; }
    //    public int? VoucherId { get; set; }
    //}

    //public class RentCostResult
    //{
    //    public decimal RentCost { get; set; }
    //    public int? CampaignId { get; set; }
    //    public int? VoucherId { get; set; }
    //}

    public class CostCalculator
    {
        private readonly Data.Contexts.FullDatabaseContext _context;
        private DateTime _pickupDate;
        private DateTime _returnDate;
        private int TotalDays { get; set; }
        private int _segmentId;

        public CostCalculator(Data.Contexts.FullDatabaseContext context, DateTime pickupDate, DateTime returnDate)
        {
            _context = context;
            _pickupDate = pickupDate;
            _returnDate = returnDate;
        }

        public int GetTotalDays()
        {
            if (TotalDays == 0)
            {
                TimeSpan difference = _returnDate - _pickupDate;
                int daysDifference = difference.Days;

                if (difference.Hours > 0 || difference.Minutes > 0 || difference.Seconds > 0)
                {
                    daysDifference++;
                }
                TotalDays = daysDifference;
            }
            return TotalDays;
        }

        public int? GetVoucherId(string voucherCode, int[] extraIds, bool isBackofficeRequest = false)
        {
            int? voucherId = null;

            for (int i = 0; i < GetTotalDays(); i++)
            {
                voucherId = _context.Vouchers.FirstOrDefault(x =>
                ((isBackofficeRequest && x.IsUsableForBackoffice) || !isBackofficeRequest) &&
                (x.DiscountType != Data.Enums.DiscountTypes.Extra
                    || x.Extras.Any(x => extraIds.Contains(x.Id))
                ) &&
                x.AppliesToBookingsMadeFromUtc <= DateTime.UtcNow && x.AppliesToBookingsMadeUntilUtc >= DateTime.UtcNow &&
                x.ValidFromUtc <= _pickupDate && x.ValidUntilUtc >= _returnDate &&
                x.Code == voucherCode)?.Id;

                if (voucherId.HasValue)
                {
                    break;
                }
            }

            return voucherId;
        }

        public void SetSegment(int segmentId)
        {
            _segmentId = segmentId;
        }

        public int? GetCampaignId(int[] extraIds, bool isBackofficeRequest = false)
        {
            int? campaignId = null;

            for (int i = 0; i < GetTotalDays(); i++)
            {
                campaignId = _context.Campaigns.FirstOrDefault(x =>
                    ((isBackofficeRequest && x.IsUsableForBackoffice) || !isBackofficeRequest) &&
                    (x.DiscountType != Data.Enums.DiscountTypes.Extra
                        || x.Extras.Any(e => extraIds.Contains(e.Id))
                    ) &&
                    x.AppliesToBookingsMadeFromUtc <= DateTime.UtcNow && x.AppliesToBookingsMadeUntilUtc >= DateTime.UtcNow &&
                    x.ValidFromUtc <= _pickupDate && x.ValidUntilUtc >= _returnDate &&
                    (x.CarSegments.Any(x => x.Id == _segmentId) || !x.CarSegments.Any())
                    && x.IsActive)
                ?.Id;

                if (campaignId.HasValue)
                {
                    break;
                }
            }

            return campaignId;
        }

        public decimal GetPickupReturnCost(int pickupReturnLocationId)
        {
            return _context.PickupReturnLocationTaxes
                        .OrderByDescending(x => x.Days)
                        .FirstOrDefault(x => x.PickupReturnLocationId == pickupReturnLocationId && x.Days <= GetTotalDays())?.Value ?? 0;
        }

        public decimal? GetCarCost()
        {
            decimal carCost = 0;

            for (int i = 0; i < GetTotalDays(); i++)
            {
                DateTime currentDay = new DateTime(_pickupDate.AddDays(i).Year, _pickupDate.AddDays(i).Month, _pickupDate.AddDays(i).Day);
                Data.Models.Database.Price currentDatePrice = _context.Prices.OrderByDescending(x => x.Days).FirstOrDefault(x =>
                    x.Season.StartDateUtc <= currentDay && x.Season.EndDateUtc >= currentDay &&
                    x.CarSegmentId == _segmentId && x.Days <= GetTotalDays());

                if (currentDatePrice == null)
                    return null;

                carCost += currentDatePrice.Value;
            }

            return carCost;
        }

        public decimal GetInsuranceCost(int insuranceLevelId)
        {
            decimal insuranceValue;

            int insurancePriceDays = _context.InsurancePrices.Where(x => x.Insurance.InsuranceLevelId == insuranceLevelId && x.Insurance.CarSegmentId == _segmentId && x.Days <= GetTotalDays()).Max(x => x.Days);

            Data.Models.Database.Insurance insurance = _context.CarSegments.AsNoTracking()
                                                            .Include(x => x.Insurances)
                                                                .ThenInclude(x => x.Prices.Where(y => y.Days == insurancePriceDays))
                                                            .FirstOrDefault(x => x.Id == _segmentId).Insurances.FirstOrDefault(x => x.InsuranceLevelId == insuranceLevelId);

            if (insurance == null || !insurance.Prices.Any())
            {
                return 0;
            }

            insuranceValue = insurance.Prices.First().Value * GetTotalDays();
            if (insurance.MaximumValue.HasValue && insuranceValue > insurance.MaximumValue)
            {
                insuranceValue = insurance.MaximumValue.Value;
            }
            else if (insurance.MinimumValue.HasValue && insuranceValue < insurance.MinimumValue)
            {
                insuranceValue = insurance.MinimumValue.Value;
            }

            return insuranceValue;
        }

        public decimal GetInsuranceExcess(int insuranceLevelId)
        {
            Data.Models.Database.Insurance? insurance = _context.CarSegments.AsNoTracking()
                                                            .Include(x => x.Insurances)
                                                            .FirstOrDefault(x => x.Id == _segmentId)?.Insurances?.FirstOrDefault(x => x.InsuranceLevelId == insuranceLevelId);

            return insurance?.Excess ?? 0;
        }

        public Dictionary<int, decimal> GetExtrasCost(List<int>? extraIds, int insuranceLevelId)
        {
            Dictionary<int, decimal> result = new();

            if (extraIds != null && extraIds.Any())
            {
                foreach (int extraId in extraIds)
                {
                    Data.Models.Database.Extra extra = _context.Extras.Include(x => x.ExtraPricesByInsuranceLevel.Where(y => y.InsuranceLevelId == insuranceLevelId)).FirstOrDefault(x => x.Id == extraId);

                    if (extra != null && extra.ExtraPricesByInsuranceLevel.Any())
                    {
                        decimal extraTotal = extra.ExtraPricesByInsuranceLevel.First().Value * GetTotalDays();
                        if (extra.ExtraPricesByInsuranceLevel.First().MaximumValue.HasValue && extraTotal > extra.ExtraPricesByInsuranceLevel.First().MaximumValue)
                        {
                            extraTotal = extra.ExtraPricesByInsuranceLevel.First().MaximumValue.Value;
                        }

                        result.Add(extra.Id, extraTotal);
                    }
                }
            }

            return result;
        }

        public Dictionary<int, Tuple<int, decimal>> GetPickupReturnTemporaryTaxes()
        {
            // Key: PickupReturnTemporaryTaxId, Value: Tuple<Quantity, UnitValue>
            Dictionary<int, Tuple<int, decimal>> result = new();

            // Get pickup temporary taxes by hour only
            var temporaryTaxes = _context.PickupReturnTemporaryTaxes
                                        .Where(x =>
                                            !x.StartDate.HasValue && !x.EndDate.HasValue
                                            &&  (
                                                    (
                                                        x.StartTime < x.EndTime
                                                        && x.StartTime <= TimeOnly.FromDateTime(_pickupDate)
                                                        && x.EndTime >= TimeOnly.FromDateTime(_pickupDate)
                                                    )
                                                    ||
                                                    (
                                                        x.StartTime > x.EndTime
                                                        && x.StartTime <= TimeOnly.FromDateTime(_pickupDate)
                                                        && TimeOnly.FromDateTime(_pickupDate) <= TimeOnly.MaxValue
                                                    )
                                                    ||
                                                    (
                                                        x.StartTime > x.EndTime
                                                        && x.EndTime >= TimeOnly.FromDateTime(_pickupDate)
                                                        && TimeOnly.FromDateTime(_pickupDate) >= TimeOnly.MinValue
                                                    )
                                                )
                                            ).ToList();
            foreach (var temporaryTax in temporaryTaxes)
            {
                result.Add(temporaryTax.Id, new Tuple<int, decimal>(1, temporaryTax.Value));
            }

            // Get return temporary taxes by hour only
            temporaryTaxes = _context.PickupReturnTemporaryTaxes
                                        .Where(x =>
                                            !x.StartDate.HasValue && !x.EndDate.HasValue
                                            && (
                                                    (
                                                        x.StartTime < x.EndTime
                                                        && x.StartTime <= TimeOnly.FromDateTime(_returnDate)
                                                        && x.EndTime >= TimeOnly.FromDateTime(_returnDate)
                                                    )
                                                    ||
                                                    (
                                                        x.StartTime > x.EndTime
                                                        && x.StartTime <= TimeOnly.FromDateTime(_returnDate)
                                                        && TimeOnly.FromDateTime(_returnDate) <= TimeOnly.MaxValue
                                                    )
                                                    ||
                                                    (
                                                        x.StartTime > x.EndTime
                                                        && x.EndTime >= TimeOnly.FromDateTime(_returnDate)
                                                        && TimeOnly.FromDateTime(_returnDate) >= TimeOnly.MinValue
                                                    )
                                                )
                                            ).ToList();
            foreach (var temporaryTax in temporaryTaxes)
            {
                if (result.ContainsKey(temporaryTax.Id))
                {
                    var existing = result[temporaryTax.Id];
                    result[temporaryTax.Id] = new Tuple<int, decimal>(existing.Item1 + 1, existing.Item2);
                }
                else
                {
                    result.Add(temporaryTax.Id, new Tuple<int, decimal>(1, temporaryTax.Value));
                }
            }

            // Get pickup temporary taxes by date only
            temporaryTaxes = _context.PickupReturnTemporaryTaxes
                                        .Where(x =>
                                            !x.StartTime.HasValue && !x.StartTime.HasValue
                                            && x.StartDate <= DateOnly.FromDateTime(_pickupDate)
                                            && x.EndDate >= DateOnly.FromDateTime(_pickupDate)
                                            ).ToList();
            foreach (var temporaryTax in temporaryTaxes)
            {
                if (result.ContainsKey(temporaryTax.Id))
                {
                    var existing = result[temporaryTax.Id];
                    result[temporaryTax.Id] = new Tuple<int, decimal>(existing.Item1 + 1, existing.Item2);
                }
                else
                {
                    result.Add(temporaryTax.Id, new Tuple<int, decimal>(1, temporaryTax.Value));
                }
            }

            // Get return temporary taxes by date only
            temporaryTaxes = _context.PickupReturnTemporaryTaxes
                                        .Where(x =>
                                            !x.StartTime.HasValue && !x.StartTime.HasValue
                                            && x.StartDate <= DateOnly.FromDateTime(_returnDate)
                                            && x.EndDate >= DateOnly.FromDateTime(_returnDate)
                                            ).ToList();
            foreach (var temporaryTax in temporaryTaxes)
            {
                if (result.ContainsKey(temporaryTax.Id))
                {
                    var existing = result[temporaryTax.Id];
                    result[temporaryTax.Id] = new Tuple<int, decimal>(existing.Item1 + 1, existing.Item2);
                }
                else
                {
                    result.Add(temporaryTax.Id, new Tuple<int, decimal>(1, temporaryTax.Value));
                }
            }

            // Get pickup temporary taxes by date and time
            temporaryTaxes = _context.PickupReturnTemporaryTaxes
                                        .Where(x =>
                                            x.StartDate.HasValue && x.EndDate.HasValue && x.StartTime.HasValue && x.EndTime.HasValue
                                            && x.StartDate.Value <= DateOnly.FromDateTime(_pickupDate)
                                            && x.StartTime.Value <= TimeOnly.FromDateTime(_pickupDate)
                                            && x.EndDate.Value >= DateOnly.FromDateTime(_pickupDate)
                                            && x.EndTime.Value >= TimeOnly.FromDateTime(_pickupDate)
                                            ).ToList();
            foreach (var temporaryTax in temporaryTaxes)
            {
                if (result.ContainsKey(temporaryTax.Id))
                {
                    var existing = result[temporaryTax.Id];
                    result[temporaryTax.Id] = new Tuple<int, decimal>(existing.Item1 + 1, existing.Item2);
                }
                else
                {
                    result.Add(temporaryTax.Id, new Tuple<int, decimal>(1, temporaryTax.Value));
                }
            }

            // Get return temporary taxes by date and time
            temporaryTaxes = _context.PickupReturnTemporaryTaxes
                                        .Where(x =>
                                            x.StartDate.HasValue && x.EndDate.HasValue && x.StartTime.HasValue && x.EndTime.HasValue
                                            && x.StartDate.Value <= DateOnly.FromDateTime(_returnDate)
                                            && x.StartTime.Value <= TimeOnly.FromDateTime(_returnDate)
                                            && x.EndDate.Value >= DateOnly.FromDateTime(_returnDate)
                                            && x.EndTime.Value >= TimeOnly.FromDateTime(_returnDate)
                                            ).ToList();
            foreach (var temporaryTax in temporaryTaxes)
            {
                if (result.ContainsKey(temporaryTax.Id))
                {
                    var existing = result[temporaryTax.Id];
                    result[temporaryTax.Id] = new Tuple<int, decimal>(existing.Item1 + 1, existing.Item2);
                }
                else
                {
                    result.Add(temporaryTax.Id, new Tuple<int, decimal>(1, temporaryTax.Value));
                }
            }

            return result;
        }

        private decimal GetCampaingVoucherDiscountValue(decimal carCost, decimal pickupCost, decimal returnCost, int? voucherId, int? campaignId, Dictionary<int, Tuple<int, decimal>> extras)
        {
            decimal discount = 0;

            Data.Models.Database.Voucher? appliedVoucher = null;
            if (voucherId.HasValue)
            {
                appliedVoucher = _context.Vouchers.Include(x => x.Extras).FirstOrDefault(x => x.Id == voucherId);
            }

            Data.Models.Database.Campaign? appliedCampaign = null;
            if (campaignId.HasValue)
            {
                appliedCampaign = _context.Campaigns.Include(x => x.Extras).FirstOrDefault(x => x.Id == campaignId);
            }

            if (appliedVoucher != null)
            {
                switch (appliedVoucher.DiscountType)
                {
                    case Data.Enums.DiscountTypes.Percentage:
                        discount = Math.Round((carCost + pickupCost + returnCost) * ((decimal)(appliedVoucher.Value) / 100), 2);
                        break;
                    case Data.Enums.DiscountTypes.Euro:
                        discount = Math.Round((decimal)appliedVoucher.Value, 2);
                        break;
                }
                foreach (var extra in appliedVoucher.Extras)
                {
                    if (extras.ContainsKey(extra.Id))
                    {
                        var extraCost = extras[extra.Id].Item1 * extras[extra.Id].Item2;
                        discount += extraCost;
                    }
                }
            }
            else if (appliedCampaign != null)
            {
                switch (appliedCampaign.DiscountType)
                {
                    case Data.Enums.DiscountTypes.Percentage:
                        discount = Math.Round((carCost + pickupCost + returnCost) * ((decimal)(appliedCampaign.Value) / 100), 2);
                        break;
                    case Data.Enums.DiscountTypes.Euro:
                        discount = Math.Round((decimal)appliedCampaign.Value, 2);
                        break;
                }
                foreach (var extra in appliedCampaign.Extras)
                {
                    if (extras.ContainsKey(extra.Id))
                    {
                        var extraCost = extras[extra.Id].Item1 * extras[extra.Id].Item2;
                        discount += extraCost;
                    }
                }
            }

            return discount;
        }

        public decimal GetTotalCost(Data.Models.Database.Reservation reservation)
        {
            Dictionary<int, Tuple<int, decimal>> extras = new();
            foreach (var extra in reservation.Extras)
            {
                extras.Add(extra.ExtraId, new Tuple<int, decimal>(extra.Quantity, extra.UnitValue));
            }
            return GetTotalCost(reservation.CarSegmentCost,
                                reservation.PickupCost,
                                reservation.ReturnCost,
                                reservation.InsuranceCost,
                                (reservation.Extras?.Sum(x => x.UnitValue * x.Quantity) ?? 0),
                                (reservation.PickupReturnTemporaryTaxes?.Sum(x => x.UnitValue * x.Quantity) ?? 0),
                                (reservation.Services?.Sum(x => x.Value) ?? 0),
                                reservation.VoucherId,
                                reservation.CampaignId,
                                extras);
        }

        public decimal GetTotalCost(decimal carSegmentCost, decimal pickupCost, decimal returnCost, decimal insuranceCost, decimal extrasCost, decimal pickupReturnTemporaryTaxesCost, decimal servicesCost, int? voucherId, int? campaignId, Dictionary<int, Tuple<int, decimal>> extras)
        {
            decimal discount = GetCampaingVoucherDiscountValue(carSegmentCost, pickupCost, returnCost, voucherId, campaignId, extras);
            return carSegmentCost
                    + pickupCost
                    + returnCost
                    + insuranceCost
                    + extrasCost
                    + pickupReturnTemporaryTaxesCost
                    + servicesCost
                    - discount;
        }

        public decimal GetTotalCost(Data.Models.Database.QuotationItem quotationItem)
        {
            Dictionary<int, Tuple<int, decimal>> extras = new();
            foreach (var extra in quotationItem.Extras)
            {
                extras.Add(extra.ExtraId, new Tuple<int, decimal>(extra.Quantity, extra.UnitValue));
            }
            decimal discount = GetCampaingVoucherDiscountValue(quotationItem.CarSegmentCost, quotationItem.PickupCost, quotationItem.ReturnCost, quotationItem.VoucherId, quotationItem.CampaignId, extras);
            return quotationItem.CarSegmentCost
                    + quotationItem.PickupCost
                    + quotationItem.ReturnCost
                    + quotationItem.InsuranceCost
                    + (quotationItem.Extras?.Sum(x => x.UnitValue * x.Quantity) ?? 0)
                    + (quotationItem.PickupReturnTemporaryTaxes?.Sum(x => x.UnitValue * x.Quantity) ?? 0)
                    + (quotationItem.Services?.Sum(x => x.Value) ?? 0)
                    - discount;
        }

        //public CostCalculator(Data.Contexts.FullDatabaseContext context)
        //{
        //    _context = context;
        //}


        //public int GetDaysDifference(DateTime start, DateTime end)
        //{
        //    TimeSpan difference = end - start;
        //    int daysDifference = difference.Days;

        //    if (difference.Hours > 0 || difference.Minutes > 0 || difference.Seconds > 0)
        //    {
        //        daysDifference++;
        //    }

        //    return daysDifference;
        //}

        //public CostResult? GetCosts(DateTime pickupDate,
        //                            DateTime returnDate,
        //                            int segmentId,
        //                            int pickupLocationId,
        //                            int returnLocationId,
        //                            Dictionary<int, int>? extras, // ExtraId, Quantity
        //                            int insuranceLevelId,
        //                            string? voucherCode = null)
        //{
        //    try
        //    {

        //        CostResult result = new();

        //        int totalDays = GetDaysDifference(pickupDate, returnDate);

        //        Data.Models.Database.Voucher? appliedVoucher = null;
        //        Data.Models.Database.Campaign? appliedCampaign = null;

        //        // Car costs

        //        for (int i = 0; i < totalDays; i++)
        //        {
        //            DateTime currentDay = new DateTime(pickupDate.AddDays(i).Year, pickupDate.AddDays(i).Month, pickupDate.AddDays(i).Day);
        //            Data.Models.Database.Price currentDatePrice = _context.Prices.OrderByDescending(x => x.Days).FirstOrDefault(x =>
        //                x.Season.StartDateUtc <= currentDay && x.Season.EndDateUtc >= currentDay &&
        //                x.CarSegmentId == segmentId && x.Days <= totalDays);

        //            if (currentDatePrice == null)
        //                return null;

        //            // Check for vouchers
        //            if (!string.IsNullOrEmpty(voucherCode) && appliedVoucher == null)
        //            {
        //                appliedVoucher = _context.Vouchers.FirstOrDefault(x =>
        //                x.AppliesToBookingsMadeFromUtc <= DateTime.UtcNow && x.AppliesToBookingsMadeUntilUtc >= DateTime.UtcNow &&
        //                x.ValidFromUtc <= pickupDate && x.ValidUntilUtc >= returnDate &&
        //                x.Code == voucherCode);
        //            }

        //            // Check for campaigns
        //            if (appliedVoucher == null && appliedCampaign == null)
        //            {
        //                appliedCampaign = _context.Campaigns.FirstOrDefault(x =>
        //                x.AppliesToBookingsMadeFromUtc <= DateTime.UtcNow && x.AppliesToBookingsMadeUntilUtc >= DateTime.UtcNow &&
        //                x.ValidFromUtc <= pickupDate && x.ValidUntilUtc >= returnDate &&
        //                x.CarSegments.Any(x => x.Id == segmentId));
        //            }

        //            result.CarCost += currentDatePrice.Value;
        //        }

        //        // Pickup and Return costs

        //        Data.Models.Database.PickupReturnLocationTax pickupLocationTax = _context.PickupReturnLocationTaxes
        //                    .OrderByDescending(x => x.Days).FirstOrDefault(x =>
        //            x.PickupReturnLocationId == pickupLocationId && x.Days <= totalDays);

        //        result.PickupCost = pickupLocationTax?.Value ?? 0;

        //        Data.Models.Database.PickupReturnLocationTax returnLocationTax = _context.PickupReturnLocationTaxes
        //                    .OrderByDescending(x => x.Days).FirstOrDefault(x =>
        //            x.PickupReturnLocationId == returnLocationId && x.Days <= totalDays);

        //        result.ReturnCost = returnLocationTax?.Value ?? 0;

        //        // Extra costs

        //        if (extras != null)
        //        {
        //            foreach (int extraId in extras.Keys)
        //            {
        //                Data.Models.Database.Extra extra = _context.Extras
        //                    .Include(x => x.Translations)
        //                    .FirstOrDefault(x => x.Id == extraId);

        //                decimal extraTotal = extra.Value * totalDays;
        //                if (extra.MaximumValue.HasValue && extraTotal > extra.MaximumValue)
        //                {
        //                    extraTotal = extra.MaximumValue.Value;
        //                }

        //                result.ExtraCost.Add(extra.Id, extraTotal);
        //            }
        //        }

        //        // Insurance costs

        //        Data.Models.Database.Insurance insurance = _context.CarSegments.AsNoTracking().Include(x => x.Insurances).FirstOrDefault(x => x.Id == segmentId).Insurances.FirstOrDefault(x => x.InsuranceLevelId == insuranceLevelId);

        //        decimal insuranceTotal = insurance.Value * totalDays;
        //        if (insurance.MaximumValue.HasValue && insuranceTotal > insurance.MaximumValue)
        //        {
        //            insuranceTotal = insurance.MaximumValue.Value;
        //        }
        //        else if (insurance.MinimumValue.HasValue && insuranceTotal < insurance.MinimumValue)
        //        {
        //            insuranceTotal = insurance.MinimumValue.Value;
        //        }

        //        result.InsuranceCost = insuranceTotal;
        //        result.InsuranceExcess = insurance.Excess;

        //        // Discount (Campaign or Voucher)

        //        if (appliedVoucher != null)
        //        {
        //            result.VoucherId = appliedVoucher.Id;
        //            switch (appliedVoucher.ValueUnit)
        //            {
        //                case Data.Enums.DiscountValueUnits.Percentage:
        //                    result.Discount = Math.Round((result.CarCost + result.PickupCost + result.ReturnCost) * ((decimal)(appliedVoucher.Value) / 100), 2);
        //                    break;
        //                case Data.Enums.DiscountValueUnits.Euro:
        //                    result.Discount = Math.Round((decimal)appliedVoucher.Value, 2);
        //                    break;
        //            }
        //        }
        //        else if (appliedCampaign != null)
        //        {
        //            result.CampaignId = appliedCampaign.Id;
        //            switch (appliedCampaign.ValueUnit)
        //            {
        //                case Data.Enums.DiscountValueUnits.Percentage:
        //                    result.Discount = Math.Round((result.CarCost + result.PickupCost + result.ReturnCost) * ((decimal)(appliedVoucher.Value) / 100), 2);
        //                    break;
        //                case Data.Enums.DiscountValueUnits.Euro:
        //                    result.Discount = Math.Round((decimal)appliedVoucher.Value, 2);
        //                    break;
        //            }
        //        }

        //        // Total

        //        result.Total = result.CarCost
        //                        + result.PickupCost
        //                        + result.ReturnCost
        //                        + result.InsuranceCost
        //                        + result.ExtraCost.Sum(x => extras.First(y => y.Key == x.Key).Value * x.Value)
        //                        - (result.Discount ?? 0);

        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

        //public RentCostResult? GetRentCost(DateTime pickupDate, DateTime returnDate, int segmentId, string? voucherCode = null)
        //{
        //    try
        //    {
        //        RentCostResult result = new();
        //        result.RentCost = 0;

        //        int totalDays = GetDaysDifference(pickupDate, returnDate);

        //        Data.Models.Database.Voucher? appliedVoucher = null;
        //        Data.Models.Database.Campaign? appliedCampaign = null;

        //        for (int i = 0; i < totalDays; i++)
        //        {
        //            DateTime currentDay = new DateTime(pickupDate.AddDays(i).Year, pickupDate.AddDays(i).Month, pickupDate.AddDays(i).Day);
        //            Data.Models.Database.Price currentDatePrice = _context.Prices.OrderByDescending(x => x.Days).FirstOrDefault(x =>
        //                x.Season.StartDateUtc <= currentDay && x.Season.EndDateUtc >= currentDay &&
        //                x.CarSegmentId == segmentId && x.Days <= totalDays);

        //            if (currentDatePrice == null)
        //                return null;

        //            // Check for vouchers
        //            if (!string.IsNullOrEmpty(voucherCode) && appliedVoucher == null)
        //            {
        //                appliedVoucher = _context.Vouchers.FirstOrDefault(x =>
        //                x.AppliesToBookingsMadeFromUtc <= DateTime.UtcNow && x.AppliesToBookingsMadeUntilUtc >= DateTime.UtcNow &&
        //                x.ValidFromUtc <= pickupDate && x.ValidUntilUtc >= returnDate &&
        //                x.Code == voucherCode);
        //            }

        //            // Check for campaigns
        //            if (appliedVoucher == null && appliedCampaign == null)
        //            {
        //                appliedCampaign = _context.Campaigns.FirstOrDefault(x =>
        //                x.AppliesToBookingsMadeFromUtc <= DateTime.UtcNow && x.AppliesToBookingsMadeUntilUtc >= DateTime.UtcNow &&
        //                x.ValidFromUtc <= pickupDate && x.ValidUntilUtc >= returnDate &&
        //                x.CarSegments.Any(x => x.Id == segmentId));
        //            }

        //            result.RentCost += currentDatePrice.Value;
        //        }

        //        if (appliedVoucher != null)
        //        {
        //            result.VoucherId = appliedVoucher.Id;
        //            //switch (appliedVoucher.ValueUnit)
        //            //{
        //            //    case Data.Enums.DiscountValueUnits.Percentage:
        //            //        result.RentCost = Math.Round(result.RentCost * ((decimal)(100 - appliedVoucher.Value) / 100), 2);
        //            //        break;
        //            //    case Data.Enums.DiscountValueUnits.Euro:
        //            //        result.RentCost = Math.Round(result.RentCost - appliedVoucher.Value, 2);
        //            //        break;
        //            //}
        //        }
        //        else if (appliedCampaign != null)
        //        {
        //            result.CampaignId = appliedCampaign.Id;
        //            //switch (appliedCampaign.ValueUnit)
        //            //{
        //            //    case Data.Enums.DiscountValueUnits.Percentage:
        //            //        result.RentCost = Math.Round(result.RentCost * ((decimal)(100 - appliedCampaign.Value) / 100), 2);
        //            //        break;
        //            //    case Data.Enums.DiscountValueUnits.Euro:
        //            //        result.RentCost = Math.Round(result.RentCost - appliedCampaign.Value, 2);
        //            //        break;
        //            //}
        //        }

        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

        //public decimal GetPickupReturnCost(DateTime pickupDate, DateTime ReturnDate, int locationId)
        //{
        //    try
        //    {
        //        decimal result = 0;

        //        int totalDays = GetDaysDifference(pickupDate, ReturnDate);

        //        Data.Models.Database.PickupReturnLocationTax locationTax = _context.PickupReturnLocationTaxes
        //                    .OrderByDescending(x => x.Days).FirstOrDefault(x =>
        //            x.PickupReturnLocationId == locationId && x.Days <= totalDays);

        //        result = locationTax?.Value ?? 0;

        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        return 0;
        //    }
        //}

        //public Dictionary<int, decimal> GetExtraCost(DateTime pickupDate, DateTime ReturnDate, List<int> extraIds, int? voucherId, int? campaignId)
        //{
        //    Dictionary<int, decimal> result = new();
        //    try
        //    {
        //        int totalDays = GetDaysDifference(pickupDate, ReturnDate);

        //        foreach (int extraId in extraIds)
        //        {
        //            Data.Models.Database.Extra extra = _context.Extras
        //                .Include(x => x.Translations)
        //                .FirstOrDefault(x => x.Id == extraId);

        //            decimal total = extra.Value * totalDays;
        //            if (extra.MaximumValue.HasValue && total > extra.MaximumValue)
        //            {
        //                total = extra.MaximumValue.Value;
        //            }

        //            //if (voucherId.HasValue)
        //            //{
        //            //    Data.Models.Database.Voucher appliedVoucher = _context.Vouchers.FirstOrDefault(x => x.Id == voucherId);
        //            //    switch (appliedVoucher.ValueUnit)
        //            //    {
        //            //        case Data.Enums.DiscountValueUnits.Percentage:
        //            //            total = Math.Round(total * ((decimal)(100 - appliedVoucher.Value) / 100), 2);
        //            //            break;
        //            //    }
        //            //}
        //            //else if (campaignId.HasValue)
        //            //{
        //            //    Data.Models.Database.Campaign appliedCampaign = _context.Campaigns.FirstOrDefault(x => x.Id == campaignId);
        //            //    switch (appliedCampaign.ValueUnit)
        //            //    {
        //            //        case Data.Enums.DiscountValueUnits.Percentage:
        //            //            total = Math.Round(total * ((decimal)(100 - appliedCampaign.Value) / 100), 2);
        //            //            break;
        //            //    }
        //            //}

        //            result.Add(extra.Id, total);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }

        //    return result;
        //}

        //public List<Data.Models.Database.Insurance> GetInsuranceCost(DateTime pickupDate, DateTime ReturnDate, List<Data.Models.Database.Insurance> segmentInsurances)
        //{
        //    try
        //    {
        //        int totalDays = GetDaysDifference(pickupDate, ReturnDate);

        //        foreach (Data.Models.Database.Insurance insurance in segmentInsurances)
        //        {
        //            decimal total = insurance.Value * totalDays;
        //            if (insurance.MaximumValue.HasValue && total > insurance.MaximumValue)
        //            {
        //                total = insurance.MaximumValue.Value;
        //            }
        //            else if (insurance.MinimumValue.HasValue && total < insurance.MinimumValue)
        //            {
        //                total = insurance.MinimumValue.Value;
        //            }

        //            segmentInsurances.FirstOrDefault(x => x.InsuranceLevelId == insurance.InsuranceLevelId).Value = total;
        //        }

        //        return segmentInsurances;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}
    }
}
