using System.ComponentModel.DataAnnotations;

namespace AMRent.Data.Models.Database
{
    public class Country
    {
        public int Id { get; set; }
        [StringLength(2)]
        public string Alpha2Code { get; set; }
        [StringLength(5)]
        public string TelephoneCode { get; set; }
        [StringLength(3)]
        public string Iso3166NumberCode { get; set; }

        public ICollection<CountryTranslation>? Translations { get; set; } = new List<CountryTranslation>();
    }
}
