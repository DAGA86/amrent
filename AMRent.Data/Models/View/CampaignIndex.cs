namespace AMRent.Data.Models.View
{
    public class CampaignIndex
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? DiscountType { get; set; }
        public int? Value { get; set; }
        public string? Extras { get; set; }
        public DateTime? ValidFromUtc { get; set; }
        public DateTime? ValidUntilUtc { get; set; }
        public bool IsActive { get; set; }
        public bool HasReservations { get; set; }
    }
}
