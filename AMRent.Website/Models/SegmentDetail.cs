using System.ComponentModel.DataAnnotations.Schema;

namespace AMRent.Website.Models
{
    public class SegmentDetail
    {
        // Filters
        public int PickupLocationId { get; set; }
        public string? CustomPickupLocationName { get; set; }
        public DateTime PickupDateTime { get; set; }
        public int ReturnLocationId { get; set; }
        public string? CustomReturnLocationName { get; set; }
        public DateTime ReturnDateTime { get; set; }

        // Data
        public int SegmentId { get; set; }
        public string? Name { get; set; }
        public string? Gearbox { get; set; }
        public string? Fuel { get; set; }
        public int? Seats { get; set; }
        public int? LoadingSpaceLengthInMilimeters { get; set; }
        public int? LoadingSpaceWidthInMilimeters { get; set; }
        public int? LoadingSpaceHeightInMilimeters { get; set; }
        public decimal? RentValue { get; set; }
        public decimal? PickupValue { get; set; }
        public decimal? ReturnValue { get; set; }
        public decimal? TotalValue { get; set; }
        public int SelectedInsuranceLevelId { get; set; } = 1;
        public bool IsCommercial { get; set; }
        public int Days { get; set; }
        public string? VoucherCode { get; set; }
        public int? CampaignId { get; set; }
        public int? VoucherId { get; set; }
        public string? CampaignVoucherName { get; set; }

        //public List<int> SelectedExtraIds { get; set; } = new();
        public List<Extra> Extras { get; set; } = new();
        public List<Data.Models.Database.Insurance>? Insurances { get; set; } = new();
        public List<AMRent.Website.Models.PickupReturnTemporaryTax>? PickupReturnTemporaryTaxes { get; set; } = new();

        // Other
        public string? Anchor { get; set; }
        public bool PickupReturnValuesAdjusted { get; set; } = false;
    }

    public class Extra
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Column(TypeName = "decimal(7, 2)")]
        public decimal DailyValue { get; set; }
        [Column(TypeName = "decimal(7, 2)")]
        public decimal? MaximumValue { get; set; }
        public decimal? Cost { get; set; }
        public int Quantity { get; set; } = 0;
        public bool AllowMultiple { get; set; } = false;
    }
}
