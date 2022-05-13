using System.ComponentModel.DataAnnotations;

namespace Tracker.DataLayer.Enumerations
{
    public enum LifeCycleModel
    {
        [Display(Name = "Support")]
        Support = 1,
        [Display(Name = "Maintainance")]
        Maintainance = 2,
        [Display(Name = "Devlopment")]
        Devlopment = 3,
        [Display(Name = "Requirement Analysis")]
        RequirementAnalysis = 3,
    }
}
