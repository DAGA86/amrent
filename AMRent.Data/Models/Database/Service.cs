namespace AMRent.Data.Models.Database
{
    public class Service
    {
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<ServiceTranslation>? Translations { get; set; } = new List<ServiceTranslation>();
        public ICollection<ReservationService>? Reservations { get; set; } = new List<ReservationService>();
        public ICollection<QuotationItemService>? QuotationItems { get; set; } = new List<QuotationItemService>();
    }
}
