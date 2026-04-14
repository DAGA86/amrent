using System.ComponentModel.DataAnnotations;

namespace AMRent.Data.Models.Database
{
    public class TeamMemberTranslation : dCore.MultiLanguage.Models.Translation
    {
        public int TeamMemberId { get; set; }
        [StringLength(64)]
        public string Job { get; set; }

        public TeamMember? TeamMember { get; set; }
    }
}
