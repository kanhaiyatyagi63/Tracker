using System.ComponentModel.DataAnnotations;

namespace Tracker.Business.Models.Authentication
{
    public class AccountResetModel
    {
        [Required]
        public string Key { get; set; }
        [Required]
        public string Secret { get; set; }
        [Required]
        public string TemporaryPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
        [Required]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
    }
}
