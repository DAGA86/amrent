namespace AMRent.Data.Models.Database
{
    public class QuotationChange : Base.EntityChange
    {
        public int QuotationId { get; set; }
        public Quotation Quotation { get; set; }
    }
}
