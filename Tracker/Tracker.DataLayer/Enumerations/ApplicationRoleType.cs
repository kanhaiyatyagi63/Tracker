using System.ComponentModel.DataAnnotations;
using Tracker.Core.Utilities;

namespace Tracker.DataLayer.Enumerations
{
    public enum ApplicationRoleType
    {
        [Ignore]
        None = 0,
        [Display(Name = "Super Admin")]
        SuperAdmin = 1,
        [Display(Name = "Admin")]
        Admin = 2,
        [Display(Name = "User")]
        User = 3,
        [Display(Name = "System")]
        System = 4
    }
}
