using System.ComponentModel.DataAnnotations;

namespace AMRent.Data.Models.Database
{
    public class EmailContent
    {
        public int Id { get; set; }
        public Enums.EmailContentTypes Type { get; set; }
        public bool SendQuotationReservationSummaryPdf { get; set; } = false;

        public ICollection<EmailContentTranslation>? Translations { get; set; } = new List<EmailContentTranslation>();
    }
}
