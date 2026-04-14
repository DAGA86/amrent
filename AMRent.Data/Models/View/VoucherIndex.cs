namespace AMRent.Data.Models.View
{
    public class VoucherIndex
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string? DiscountType { get; set; }
        public int? Value { get; set; }
        public string? Extras { get; set; }
        public DateTime? ValidFromUtc { get; set; }
        public DateTime? ValidUntilUtc { get; set; }
    }
}
