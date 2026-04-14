namespace AMRent.Data.Models.Database
{
    public class DataProtectionConsentReservationChange : Base.EntityChange
    {
        public int DataProtectionConsentReservationId { get; set; }
        public DataProtectionConsentReservation DataProtectionConsentReservation { get; set; }
    }
}
