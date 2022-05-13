using System.ComponentModel.DataAnnotations;

namespace Tracker.Web.Models
{
    public class AccountEmailVerificationModel
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string Secret { get; set; }
    }
}
