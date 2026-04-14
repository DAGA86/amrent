using System.ComponentModel.DataAnnotations;

namespace AMRent.Data.Models.Database
{
    public class CarSegmentTranslation : dCore.MultiLanguage.Models.Translation
    {
        public int CarSegmentId { get; set; }
        [StringLength(128)]
        public string Name { get; set; }

        public CarSegment? CarSegment { get; set; }
    }
}
