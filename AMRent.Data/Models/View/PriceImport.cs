using System.ComponentModel.DataAnnotations;

namespace AMRent.Data.Models.View
{
    public class PriceImport
    {
        [Required]
        public Enums.PriceImportMethods Method { get; set; }
    }
}
