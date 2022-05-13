using System.ComponentModel.DataAnnotations;
using Tracker.Core.Models.Constants;

namespace Tracker.Business.Models.User
{
    public class UserEditProfileModel
    {
        [Required]
        [StringLength(StringLengthConstants.ApplicationUserId)]
        public string Id { get; set; }
        [Required]
        public long UserId { get; set; }
        [Required]
        [StringLength(StringLengthConstants.Name)]
        public string Name { get; set; }
        [Required]
        [StringLength(StringLengthConstants.Email)]
        public string Email { get; set; }
        [Required]
        [StringLength(StringLengthConstants.PhoneNumber)]
        public string? PhoneNumber { get; set; }
    }
}
