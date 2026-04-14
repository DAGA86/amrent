using System.ComponentModel.DataAnnotations;

namespace AMRent.Data.Models.Database
{
    public class PickupReturnTemporaryTaxTranslation : dCore.MultiLanguage.Models.Translation
    {
        public int PickupReturnTemporaryTaxId { get; set; }
        [StringLength(64)]
        public string Name { get; set; }

        public PickupReturnTemporaryTax? PickupReturnTemporaryTax { get; set; }
    }
}
