using System.ComponentModel.DataAnnotations;

namespace AMRent.Data.Models.Database
{
    public class TestimonialTranslation : dCore.MultiLanguage.Models.Translation
    {
        public int TestimonialId { get; set; }
        [StringLength(512)]
        public string Text { get; set; }

        public Testimonial? Testimonial { get; set; }
    }
}
