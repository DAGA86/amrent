namespace AMRent.Data.Models.Database
{
    public class CampaignExtra
    {
        public int CampaignId { get; set; }
        public int ExtraId { get; set; }

        public Campaign Campaign { get; set; }
        public Extra Extra { get; set; }
    }
}
