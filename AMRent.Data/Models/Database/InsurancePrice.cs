using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMRent.Data.Models.Database
{
    public class InsurancePrice
    {
        public Guid Id { get; set; }

        [Range(0, int.MaxValue)]
        public int Days { get; set; }
        [Column(TypeName = "decimal(7, 2)")]
		public decimal Value { get; set; }

        public int InsuranceId { get; set; }

        public Insurance? Insurance { get; set; }
    }
}
