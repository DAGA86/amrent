namespace AMRent.Data.Models.Database
{
    public class DataProtectionConsentReservation
    {
        public int Id { get; set; }
        public bool HasConsented { get; set; } = false;

        public int DataProtectionConsentId { get; set; }
        public DataProtectionConsent? DataProtectionConsent { get; set; }

        public int ReservationId { get; set; }
        public Reservation? Reservation { get; set; }

        public List<DataProtectionConsentReservationChange>? DataProtectionConsentReservationChanges { get; set; } = new();
    }
}
