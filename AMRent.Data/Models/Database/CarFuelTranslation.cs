using System.ComponentModel.DataAnnotations;

namespace AMRent.Data.Models.Database
{
    public class CarFuelTranslation : dCore.MultiLanguage.Models.Translation
    {
        public int CarFuelId { get; set; }
        [StringLength(128)]
        public string Name { get; set; }

        public CarFuel? CarFuel { get; set; }
    }
}
