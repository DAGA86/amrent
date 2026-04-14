using System.ComponentModel.DataAnnotations;

namespace AMRent.Data.Models.Database
{
    public class Process
    {
        public int Id { get; set; }
        [StringLength(64)]
        public string FontAwesomeIconCode { get; set; }

        public ICollection<ProcessTranslation>? Translations { get; set; } = new List<ProcessTranslation>();
    }
}
