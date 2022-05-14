using System.ComponentModel.DataAnnotations;
using Tracker.Core.Utilities;

namespace Tracker.DataLayer.Enumerations
{
    public enum ApplicationRoleType
    {
        [Ignore]
        None = 0,
        [Display(Name = "Admin")]
        Admin = 1,
        [Display(Name = "User")]
        User = 2,
        [Display(Name = "System")]
        System = 2
    }
}
