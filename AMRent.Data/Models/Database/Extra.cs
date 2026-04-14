using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AMRent.Data.Enums;

namespace AMRent.Data.Models.Database
{
    public class Extra
    {
        public int Id { get; set; }
        public Enums.ExtraTypes ExtraType { get; set; } = ExtraTypes.Other;
        public bool AllowMultiple { get; set; } = false;
        public bool IsActive { get; set; } = true;

        public ICollection<Campaign>? Campaigns { get; set; } = new List<Campaign>();
        public ICollection<Voucher>? Vouchers { get; set; } = new List<Voucher>();
        public ICollection<ExtraTranslation>? Translations { get; set; } = new List<ExtraTranslation>();
        public ICollection<ReservationExtra>? Reservations { get; set; } = new List<ReservationExtra>();
        public ICollection<QuotationItemExtra>? QuotationItems { get; set; } = new List<QuotationItemExtra>();
        public ICollection<ExtraPriceByInsuranceLevel>? ExtraPricesByInsuranceLevel { get; set; } = new List<ExtraPriceByInsuranceLevel>();
    }
}
