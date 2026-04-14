namespace AMRent.Data.Models.Database
{
    public class CampaignCarSegment
    {
        public int CampaignId { get; set; }
        public int CarSegmentId { get; set; }

        public Campaign Campaign { get; set; }
        public CarSegment CarSegment { get; set; }
    }
}
