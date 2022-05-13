using System.ComponentModel.DataAnnotations;

namespace Tracker.DataLayer.Enumerations
{
    public enum ActivityType
    {
        [Display(Name = "Design")]
        Design = 1,
        [Display(Name = "Development")]
        Development = 2,
        [Display(Name = "Testing")]
        Testing = 3,
        [Display(Name = "Management")]
        Management = 4,
    }
}
