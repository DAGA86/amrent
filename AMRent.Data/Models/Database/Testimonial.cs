using System.ComponentModel.DataAnnotations;

namespace AMRent.Data.Models.Database
{
    public class Testimonial
    {
        public int Id { get; set; }
        [StringLength(64)]
        public string AuthorName { get; set; }
        [Range(0, 5)]
        public int Score { get; set; }
        
        public ICollection<TestimonialTranslation>? Translations { get; set; } = new List<TestimonialTranslation>();
    }
}
