using System.ComponentModel.DataAnnotations.Schema;

namespace AMRent.Data.Models.Database
{
    public class About
    {
        public int Id { get; set; }
        [NotMapped]
        public string? ImagePath { get; set; }

        public ICollection<AboutTranslation>? Translations { get; set; } = new List<AboutTranslation>();
    }
}
