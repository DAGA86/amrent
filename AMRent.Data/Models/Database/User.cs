namespace AMRent.Data.Models.Database
{
    public class User : dCore.Identity.Models.User
    {
        public List<Role>? Roles { get; set; } = new();
        public List<Reservation>? AssignedReservations { get; set; } = new();
        public List<Reservation>? Reservations { get; set; } = new();
        public List<Quotation>? Quotations { get; set; } = new();
        public List<ReservationChange>? ReservationChanges { get; set; } = new();
        public List<QuotationChange>? QuotationChanges { get; set; } = new();
        public List<DataProtectionConsentUser>? DataProtectionConsents { get; set; } = new();

        public string? Name { get; set; }
        public DateTime? BirthDate { get; set; }
        public int? TelephonePrefixCountryId { get; set; }
        public string? Telephone { get; set; }
        public int? IdentityCountryId { get; set; }
        public string? IdentityNumber { get; set; }
        public int? LicenseCountryId { get; set; }
        public string? LicenseNumber { get; set; }
        public DateTime? LicenseDate { get; set; }
        public DateTime? LicenseExpireDate { get; set; }
        public string? Address { get; set; }
        public string? PostalCode { get; set; }
        public string? PostalLocation { get; set; }
        public int? CountryId { get; set; }
        public string? VatNumber { get; set; }
    }
}
