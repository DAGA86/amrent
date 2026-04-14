using System.ComponentModel.DataAnnotations.Schema;

namespace AMRent.Data.Models.Database
{
    public class QuotationItem
    {
        public int Id { get; set; }

        public int QuotationId { get; set; }

        // Associations
        public int CarSegmentId { get; set; }
        public int? CampaignId { get; set; }
        public int? VoucherId { get; set; }
        public int InsuranceLevelId { get; set; }

        // Payment
        [Column(TypeName = "decimal(7, 2)")]
        public decimal CarSegmentCost { get; set; }
        [Column(TypeName = "decimal(7, 2)")]
        public decimal PickupCost { get; set; }
        [Column(TypeName = "decimal(7, 2)")]
        public decimal ReturnCost { get; set; }
        [Column(TypeName = "decimal(7, 2)")]
        public decimal InsuranceCost { get; set; }
        [Column(TypeName = "decimal(7, 2)")]
        public decimal InsuranceExcess { get; set; }
        [Column(TypeName = "decimal(7, 2)")]
        public decimal TotalCost { get; set; }
        [Column(TypeName = "decimal(7, 2)")]
        public decimal? TotalCostOverride { get; set; }

        // Navigation Properties
        public CarSegment? CarSegment { get; set; }
        public InsuranceLevel? InsuranceLevel { get; set; }
        public Campaign? Campaign { get; set; }
        public Voucher? Voucher { get; set; }
        public List<QuotationItemExtra>? Extras { get; set; } = new List<QuotationItemExtra>();
        public List<QuotationItemPickupReturnTemporaryTax>? PickupReturnTemporaryTaxes { get; set; } = new();
        public List<QuotationItemService>? Services { get; set; } = new List<QuotationItemService>();
    }
}
