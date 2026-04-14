using System.ComponentModel.DataAnnotations;

namespace AMRent.Data.Models.Database
{
    public class ViaVerdeMovement
    {
        public Guid Id { get; set; }
        public Enums.ViaVerdeMovementStatus Status { get; set; } = Enums.ViaVerdeMovementStatus.Registered;
        public bool SendToViaVerde { get; set; }

        // MOPR

        [StringLength(5)]
        public string ManufacturerCode { get; set; }
        [StringLength(13)]
        public string EquipmentNumber { get; set; }
        [StringLength(15)]
        public string ContractLicencePlate { get; set; }
        [StringLength(1)]
        public string TariffClass { get; set; }
        [StringLength(4)]
        public string ExitTollCode { get; set; }
        [StringLength(18)]
        public string ExitTollName { get; set; }
        public DateTime ExitDate { get; set; }
        [StringLength(4)]
        public string EntryTollCode { get; set; }
        [StringLength(18)]
        public string EntryTollName { get; set; }
        public DateTime? EntryDate { get; set; }
        public int TariffInCents { get; set; }
        [StringLength(12)]
        public string TransactionCode { get; set; }
        [StringLength(2)]
        public string VatCode { get; set; }
        [StringLength(2)]
        public string ServiceCode { get; set; }

        // MERF

        [StringLength(3)]
        public string? ResultCode { get; set; }
        [StringLength(40)]
        public string? NameOrDeliverySlipNumber { get; set; }
        [StringLength(60)]
        public string? Address { get; set; }
        [StringLength(30)]
        public string? Town { get; set; }
        [StringLength(8)]
        public string? PostalCode { get; set; }
        [StringLength(30)]
        public string? PostalLocation { get; set; }
        public int? CountryId { get; set; }
        public Country? Country { get; set; }
        [StringLength(18)]
        public string? IdentificationNumber { get; set; }
        [StringLength(4)]
        public string? CreditCardLast4Digits { get; set; }
        public DateTime? CollectionAttemptedDate { get; set; }
        [StringLength(15)]
        public string? AcquirerResultCode { get; set; }
        [StringLength(128)]
        public string? AcquirerResultDescription { get; set; }
    }
}
