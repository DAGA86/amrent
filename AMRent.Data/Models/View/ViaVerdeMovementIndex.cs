namespace AMRent.Data.Models.View
{
    public class ViaVerdeMovementIndex
    {
        public Guid Id { get; set; }
        public string? TransactionCode { get; set; }
        public string? LicencePlate { get; set; }
        public string? EquipmentNumber { get; set; }
        public DateTime? EntryDate { get; set; }
        public DateTime? ExitDate { get; set; }
        public string Status { get; set; }
    }
}
