using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMRent.Data.Models.Database
{
    public class CampaignTranslation : dCore.MultiLanguage.Models.Translation
    {
        public int CampaignId { get; set; }
        [StringLength(64)]
        public string Name { get; set; }
        public string Description { get; set; }
        [NotMapped]
        public string? ImagePath { get; set; }
        [NotMapped]
        public string? PopupImagePath { get; set; }

        public Campaign? Campaign { get; set; }
    }
}
