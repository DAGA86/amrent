using System.ComponentModel.DataAnnotations;

namespace AMRent.Data.Models.Database
{
    public class CarGearboxTranslation : dCore.MultiLanguage.Models.Translation
    {
        public int CarGearboxId { get; set; }
        [StringLength(128)]
        public string Name { get; set; }

        public CarGearbox? CarGearbox { get; set; }
    }
}
