using AMRent.Data.Enums;

namespace AMRent.Website.Models
{
    public class BookingConfirmation
    {
        public string BookingNumber { get; set; }
        public string PickupLocationName { get; set; }
        public DateTime PickupDateTime { get; set; }
        public string ReturnLocationName { get; set; }
        public DateTime ReturnDateTime { get; set; }
        public int SegmentId { get; set; }
        public string? SegmentName { get; set; }
        public PaymentTypes PaymentType { get; set; }
        public decimal RentCost { get; set; }
        public decimal PickupCost { get; set; }
        public decimal ReturnCost { get; set; }
        public decimal TotalValue { get; set; }
        public string? CampaignVoucherName { get; set; }
        public string BillName { get; set; }
        public string BillEmail { get; set; }
        public string BillTelephone { get; set; }
        public string BillVatNumber { get; set; }
        public int Days { get; set; }
        public string? MultibancoEntity { get; set; }
        public string? MultibancoReference { get; set; }

        public List<Data.Models.Database.Insurance>? Insurances { get; set; } = new();
        public List<AMRent.Website.Models.SelectedExtra>? SelectedExtras { get; set; } = new();
        public List<AMRent.Website.Models.PickupReturnTemporaryTax>? PickupReturnTemporaryTaxes { get; set; } = new();
    }
}
