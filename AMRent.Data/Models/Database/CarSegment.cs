using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMRent.Data.Models.Database
{
    public class CarSegment
    {
        public int Id { get; set; }
        [Range(1, 9)]
        public int Seats { get; set; }
        public int? LoadingSpaceLengthInMilimeters { get; set; }
        public int? LoadingSpaceWidthInMilimeters { get; set; }
        public int? LoadingSpaceHeightInMilimeters { get; set; }
        [Range(1, int.MaxValue)]
        public int CarGearboxId { get; set; }
        [Range(1, int.MaxValue)]
        public int CarFuelId { get; set; }
        public int CarCategoryId { get; set; }
        public int RentCountIncrement { get; set; }
        [StringLength(16)]
        public string Code { get; set; }
        public bool IsActive { get; set; } = true;
        [NotMapped]
        public string? ImagePath { get; set; }
        [NotMapped]
        public string? ListImagePath { get; set; }
        [NotMapped]
        public string? GridImagePath { get; set; }

        public CarGearbox? CarGearbox { get; set; }
        public CarFuel? CarFuel { get; set; }
        public CarCategory? CarCategory { get; set; }
        public ICollection<CarSegmentTranslation>? Translations { get; set; } = new List<CarSegmentTranslation>();
        public List<Price>? Prices { get; set; } = new();
        public List<Insurance>? Insurances { get; set; } = new List<Insurance>();
        public ICollection<Campaign>? Campaigns { get; set; } = new List<Campaign>();
        public ICollection<Reservation>? Reservations { get; set; } = new List<Reservation>();
        //internal ICollection<CampaignCarSegment>? CampaignSegments { get; set; } = new List<CampaignCarSegment>();
    }
}
