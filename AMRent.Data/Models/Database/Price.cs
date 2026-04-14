using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AMRent.Data.Models.Database
{
    public class Price
    {
        public int Id { get; set; }
        public int SeasonId { get; set; }
        public int CarSegmentId { get; set; }
        [Range(0, int.MaxValue)]
        public int Days { get; set; }
        [Column(TypeName = "decimal(7, 2)")]
        public decimal Value { get; set; }

        public CarSegment? CarSegment { get; set; }
        public Season? Season { get; set; }

        public List<PriceChange>? Changes { get; set; } = new();
    }
}
