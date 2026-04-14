namespace AMRent.Data.Models.Database
{
    public class InsuranceChange : Base.EntityChange
    {
        public int InsuranceLevelId { get; set; }
        public int CarSegmentId { get; set; }
        public int InsuranceId { get; set; }
        public Insurance Insurance { get; set; }
    }
}
