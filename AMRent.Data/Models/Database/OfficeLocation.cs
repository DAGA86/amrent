using System.ComponentModel.DataAnnotations;

namespace AMRent.Data.Models.Database
{
    public class OfficeLocation
    {
        public int Id { get; set; }
        [StringLength(512)]
        public string Address { get; set; }
        [StringLength(16)]
        public string Telephone { get; set; }
        [StringLength(256)]
        public string Email { get; set; }
        [StringLength(512)]
        public string GoogleMapsURL { get; set; }
        [StringLength(32)]
        public string MondayToFridayTimetable { get; set; }
        [StringLength(32)]
        public string SaturdayTimetable { get; set; }
        [StringLength(32)]
        public string SundayTimetable { get; set; }

        public ICollection<OfficeLocationTranslation>? Translations { get; set; } = new List<OfficeLocationTranslation>();
    }
}
