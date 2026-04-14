using System.ComponentModel.DataAnnotations.Schema;
using AMRent.Data.Enums;

namespace AMRent.Data.Models.Database
{
    public class ReservationExtraDriver
    {
        public int Id { get; set; }
        public int ReservationId { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public int? LicenseCountryId { get; set; }
        public string? LicenseNumber { get; set; }
        public DateTime? LicenseDate { get; set; }
        public DateTime? LicenseExpireDate { get; set; }

        public Reservation? Reservation { get; set; }
        public Country? LicenseCountry { get; set; }
    }
}
