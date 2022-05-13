using Microsoft.AspNetCore.Identity;
using Tracker.DataLayer.Enumerations;

namespace Tracker.DataLayer.Entities
{
    public class ApplicationRole : IdentityRole
    {
        public string? Description { get; set; }
        public bool IsSystemGenerated { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public ApplicationRoleType ApplicationRoleType { get; set; }
    }
}
