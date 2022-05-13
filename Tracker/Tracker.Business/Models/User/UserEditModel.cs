using System.ComponentModel.DataAnnotations;
using Tracker.Core.Models.Constants;
using Tracker.DataLayer.Enumerations;

namespace Tracker.Business.Models.User
{
    public class UserEditModel
    {
        [Required]
        [StringLength(StringLengthConstants.ApplicationUserId)]
        public string Id { get; set; }
        //[Required]
        //public long UserId { get; set; }
        [Required]
        [StringLength(StringLengthConstants.Name)]
        public string Name { get; set; }
        [Required]
        [StringLength(StringLengthConstants.Email)]
        public string Email { get; set; }
        [Required]
        [StringLength(StringLengthConstants.PhoneCode)]
        public string? PhoneCode { get; set; }
        [Required]
        [StringLength(StringLengthConstants.PhoneNumber)]
        public string? PhoneNumber { get; set; }
        [Required]
        [MinLength(1)]
        public string[] Roles { get; set; }
        public ApplicationRoleType ApplicationRoleType { get; set; }
    }
}
