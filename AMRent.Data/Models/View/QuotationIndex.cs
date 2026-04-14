namespace AMRent.Data.Models.View
{
    public class QuotationIndex
    {
        public int Id { get; set; }

        public string? Number { get; set; }
        public string? CollaboratorName { get; set; }
        public string? Status { get; set; }
        public string? CustomerName { get; set; }
        public string? PickupLocation { get; set; }
        public DateTime? PickupDateTime { get; set; }
        public DateTime? CreateDateTime { get; set; }
        public DateTime? ExpireDateTime { get; set; }
        public string? BackgroundColor { get; set; }
        public int Priority { get; set; }
    }
}
