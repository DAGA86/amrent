using AMRent.Data.Enums;

namespace AMRent.Website.Models
{
    public class BookingAskQuotationConfirmation
    {
        public string QuotationNumber { get; set; }
        public string PickupLocationName { get; set; }
        public DateTime PickupDateTime { get; set; }
        public string ReturnLocationName { get; set; }
        public DateTime ReturnDateTime { get; set; }
        public int SegmentId { get; set; }
        public string? SegmentName { get; set; }
        public decimal RentCost { get; set; }
        public decimal TotalValue { get; set; }
        public string? CampaignVoucherName { get; set; }
        public int Days { get; set; }

        public List<Data.Models.Database.Insurance>? Insurances { get; set; } = new();
        public List<AMRent.Website.Models.SelectedExtra>? SelectedExtras { get; set; } = new();
    }
}
