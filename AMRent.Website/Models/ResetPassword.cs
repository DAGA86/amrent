using System.ComponentModel.DataAnnotations;

namespace AMRent.Website.Models
{
    public class ResetPassword
    {
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public string NewPassword { get; set; }
        [Required]
        public string NewPasswordConfirm { get; set; }
    }
}
