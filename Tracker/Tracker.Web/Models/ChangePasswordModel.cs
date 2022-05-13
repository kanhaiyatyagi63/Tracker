using System.ComponentModel.DataAnnotations;

namespace Tracker.Web.Models
{
    public class ChangePasswordModel
    {
        [Required]
        [MinLength(6)]
        public string CurrentPassword { get; set; }

        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword", ErrorMessage = "New password and confirm password doesn't matched")]
        public string ConfirmPassword { get; set; }
    }
}
