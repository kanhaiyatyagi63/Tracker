using System.ComponentModel.DataAnnotations;

namespace Tracker.DataLayer.Enumerations
{
    public enum ProjectType
    {

        [Display(Name = "Web")]
        Web = 1,
        [Display(Name = "Mobile")]
        Mobile = 2,
        [Display(Name = "Desktop")]
        Desktop = 3,
        [Display(Name = "Web and Mobile")]
        WebAndMobile = 1,
    }
}
