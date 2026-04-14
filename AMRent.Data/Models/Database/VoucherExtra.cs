namespace AMRent.Data.Models.Database
{
    public class VoucherExtra
    {
        public int VoucherId { get; set; }
        public int ExtraId { get; set; }

        public Voucher Voucher { get; set; }
        public Extra Extra { get; set; }
    }
}
