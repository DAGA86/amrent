using System.ComponentModel.DataAnnotations;

namespace AMRent.Website.Models
{
    public class ForgotPassword
    {
        [Required]
        public string EmailAddress { get; set; }
    }
}
