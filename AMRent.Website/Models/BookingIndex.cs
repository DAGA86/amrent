using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AMRent.Data.Enums;

namespace AMRent.Website.Models
{
    public class BookingIndex
    {
        // Filters
        public int PickupLocationId { get; set; }
        public string PickupLocationName { get; set; }
        public DateTime PickupDateTime { get; set; }
        public int ReturnLocationId { get; set; }
        public string ReturnLocationName { get; set; }
        public DateTime ReturnDateTime { get; set; }

        // Data
        public int SegmentId { get; set; }
        public Guid SegmentImageId { get; set; }
        public string? SegmentName { get; set; }
        public decimal RentValue { get; set; }
        public decimal? TotalValue { get; set; }
        public decimal PickupValue { get; set; }
        public decimal ReturnValue { get; set; }
        public int Days { get; set; }
        public int SelectedInsuranceLevelId { get; set; }
        //public List<int> SelectedExtraIds { get; set; } = new();
        public Data.Enums.PaymentTypes PaymentType { get; set; } = PaymentTypes.BankTransfer;
        public string? MbWayNumber { get; set; }
        public PaymentAmountType PaymentAmountType { get; set; } = AMRent.Website.Models.PaymentAmountType.Total;

        // Driver
        public string DriverName { get; set; }
        public DateTime DriverBirthDate { get; set; }
        public string DriverEmail { get; set; }
        public int DriverTelephoneCountryId { get; set; }
        public string DriverTelephone { get; set; }
        public Data.Enums.IdentityType DriverIdentityType { get; set; }
        public int DriverIdentityCardCountryId { get; set; }
        public string DriverIdentityCardNumber { get; set; }
        public string DriverVatNumber { get; set; }
        public int DriverLicenseCountryId { get; set; }
        public string DriverLicenseNumber { get; set; }
        public DateTime DriverLicenseDate { get; set; }
        public DateTime DriverLicenseExpireDate { get; set; }

        // Bill
        public string BillName { get; set; }
        public string BillEmail { get; set; }
        public int BillTelephoneCountryId { get; set; }
        public string BillTelephone { get; set; }
        public string BillAddress { get; set; }
        public string BillPostalCode { get; set; }
        public string BillPostalLocation { get; set; }
        public int BillCountryId { get; set; }
        public string BillVatNumber { get; set; }

        // Other
        public string FlightNumber { get; set; }
        public string Comments { get; set; }
        public string? VoucherCode { get; set; }
        public int? CampaignId { get; set; }
        public int? VoucherId { get; set; }
        public string? CampaignVoucherName { get; set; }

        public List<Data.Models.Database.Insurance>? Insurances { get; set; } = new();
        public List<AMRent.Website.Models.SelectedExtra>? Extras { get; set; } = new();
        public List<AMRent.Website.Models.PickupReturnTemporaryTax>? PickupReturnTemporaryTaxes { get; set; } = new();
        public List<AMRent.Website.Models.ExtraDriver>? ExtraDrivers { get; set; } = new();
        public List<AMRent.Website.Models.DataProtectionConsent>? DataProtectionConsents { get; set; } = new();
    }

    public class SelectedExtra
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Cost { get; set; }
        public Data.Enums.ExtraTypes Type { get; set; } = Data.Enums.ExtraTypes.Other;
        [Column(TypeName = "decimal(7, 2)")]
        public decimal DailyValue { get; set; }
        [Column(TypeName = "decimal(7, 2)")]
        public decimal? MaximumValue { get; set; }
        public int Quantity { get; set; } = 0;
    }

    public class PickupReturnTemporaryTax
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Cost { get; set; }
        [Column(TypeName = "decimal(7, 2)")]
        public decimal UnitValue { get; set; }
        public int Quantity { get; set; } = 0;
    }

    public class ExtraDriver
    {
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public int LicenseCountryId { get; set; }
        public string LicenseNumber { get; set; }
        public DateTime LicenseDate { get; set; }
        public DateTime LicenseExpireDate { get; set; }
    }

    public class DataProtectionConsent
    {
        public int Id { get; set; }
        public bool HasConsent { get; set; }
        public bool IsRequired { get; set; }
        public string Text { get; set; }
    }

    public enum PaymentAmountType
    {
        Total = 1,
        Deposit = 2
    }
}
