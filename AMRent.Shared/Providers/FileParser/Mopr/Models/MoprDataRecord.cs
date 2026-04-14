namespace AMRent.Shared.Providers.FileParser.Mopr.Models
{
    public class MoprDataRecord : IFileRecord
    {
        public string RecordType => "1";

        public string ManufacturerCode { get; set; }
        public string EquipmentNumber { get; set; }
        public string ContractLicencePlate { get; set; }
        public string TariffClass { get; set; }
        public string ExitTollCode { get; set; }
        public string ExitTollName { get; set; }
        public DateTime ExitDate { get; set; }
        public string EntryTollCode { get; set; }
        public string EntryTollName { get; set; }
        public DateTime? EntryDate { get; set; }
        public int TariffInCents { get; set; }
        public string TransactionCode { get; set; }
        public string VatCode { get; set; }
        public string ServiceCode { get; set; }
    }
}
