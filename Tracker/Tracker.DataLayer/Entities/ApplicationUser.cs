using Microsoft.AspNetCore.Identity;
using Tracker.DataLayer.Enumerations;

namespace Tracker.DataLayer.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string? PhoneCode { get; set; }
        public string? ProfileImage { get; set; }
        public DateTime? LastLoggedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsActive { get; set; }
        public ApplicationRoleType ApplicationRoleType { get; set; }
        public bool AccountActivated { get; set; }
    }
}
