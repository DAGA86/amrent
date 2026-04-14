using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMRent.Data.Models.Database
{
    public class TeamMember
    {
        public int Id { get; set; }
        [StringLength(64)]
        public string Name { get; set; }
        [StringLength(256)]
        public string Email { get; set; }
        [StringLength(16)]
        public string Telephone { get; set; }
        [NotMapped]
        public string? ImagePath { get; set; }
        public short SortNumber { get; set; }

        public ICollection<TeamMemberTranslation>? Translations { get; set; } = new List<TeamMemberTranslation>();
    }
}
