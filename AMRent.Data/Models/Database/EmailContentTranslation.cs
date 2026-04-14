using System.ComponentModel.DataAnnotations;

namespace AMRent.Data.Models.Database
{
    public class EmailContentTranslation : dCore.MultiLanguage.Models.Translation
    {
        public int EmailContentId { get; set; }
        [StringLength(64)]
        public string Subject { get; set; }
        public string Text { get; set; }

        public EmailContent? EmailContent { get; set; }
    }
}
