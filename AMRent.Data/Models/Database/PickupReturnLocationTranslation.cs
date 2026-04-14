using System.ComponentModel.DataAnnotations;

namespace AMRent.Data.Models.Database
{
    public class PickupReturnLocationTranslation : dCore.MultiLanguage.Models.Translation
    {
        public int PickupReturnLocationId { get; set; }
        [StringLength(32)]
        public string Name { get; set; }

        public PickupReturnLocation? PickupReturnLocation { get; set; }
    }
}
