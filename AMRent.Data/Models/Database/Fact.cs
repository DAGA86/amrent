using System.ComponentModel.DataAnnotations;

namespace AMRent.Data.Models.Database
{
    public class Fact
    {
        public int Id { get; set; }
        public int Number { get; set; }
        [StringLength(64)]
        public string FontAwesomeIconCode { get; set; }

        public ICollection<FactTranslation>? Translations { get; set; } = new List<FactTranslation>();
    }
}
