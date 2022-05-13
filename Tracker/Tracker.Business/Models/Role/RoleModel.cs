using System.ComponentModel.DataAnnotations;
using Tracker.Core.Models.Constants;
using Tracker.DataLayer.Enumerations;

namespace Tracker.Business.Models.Role
{
    public class RoleModel
    {
        public string Id { get; set; }
        [Required]
        [MaxLength(StringLengthConstants.Name)]
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public ApplicationRoleType ApplicationRoleType { get; set; }
    }
}
