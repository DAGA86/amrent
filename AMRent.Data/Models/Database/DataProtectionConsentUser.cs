namespace AMRent.Data.Models.Database
{
    public class DataProtectionConsentUser
    {
        public int Id { get; set; }
        public bool HasConsented { get; set; } = false;

        public int DataProtectionConsentId { get; set; }
        public DataProtectionConsent? DataProtectionConsent { get; set; }

        public Guid UserId { get; set; }
        public User? User { get; set; }
    }
}
