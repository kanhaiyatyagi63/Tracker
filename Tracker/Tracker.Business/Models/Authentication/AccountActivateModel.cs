using System.ComponentModel.DataAnnotations;

namespace Tracker.Business.Models.Authentication
{
    public class AccountActivateModel
    {
        [Required]
        public string Key { get; set; }
        [Required]
        public string Secret { get; set; }

        [Required]
        public string TemporaryPassword { get; set; }

        [Required]
        public string Password { get; set; }
        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}
