using System.ComponentModel.DataAnnotations;

namespace AMRent.Data.Models.Database
{
    public class ReasonToChooseUs
    {
        public int Id { get; set; }
        [StringLength(64)]
        public string FontAwesomeIconCode { get; set; }

        public ICollection<ReasonToChooseUsTranslation>? Translations { get; set; } = new List<ReasonToChooseUsTranslation>();
    }
}
