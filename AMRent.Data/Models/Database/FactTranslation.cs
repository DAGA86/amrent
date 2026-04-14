using System.ComponentModel.DataAnnotations;

namespace AMRent.Data.Models.Database
{
    public class FactTranslation : dCore.MultiLanguage.Models.Translation
    {
        public int FactId { get; set; }
        [StringLength(64)]
        public string Title { get; set; }

        public Fact? Fact { get; set; }
    }
}
