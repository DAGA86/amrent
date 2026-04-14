using System.ComponentModel.DataAnnotations;

namespace AMRent.Data.Models.Database
{
    public class ReasonToChooseUsTranslation : dCore.MultiLanguage.Models.Translation
    {
        public int ReasonToChooseUsId { get; set; }
        [StringLength(64)]
        public string Title { get; set; }
        [StringLength(256)]
        public string Text { get; set; }

        public ReasonToChooseUs? ReasonToChooseUs { get; set; }
    }
}
