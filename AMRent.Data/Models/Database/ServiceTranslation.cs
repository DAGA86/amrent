using System.ComponentModel.DataAnnotations;

namespace AMRent.Data.Models.Database
{
    public class ServiceTranslation : dCore.MultiLanguage.Models.Translation
    {
        public int ServiceId { get; set; }
        [StringLength(64)]
        public string Name { get; set; }

        public Service? Service { get; set; }
    }
}
