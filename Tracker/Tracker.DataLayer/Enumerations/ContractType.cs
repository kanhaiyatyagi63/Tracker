using System.ComponentModel.DataAnnotations;

namespace Tracker.DataLayer.Enumerations
{
    public enum ContractType
    {
        [Display(Name = "Fixed Price")]
        FixedPrice = 1,

        [Display(Name = "Cost Plus Fixed Fee")]
        CostPlusFixedFee = 2,

        [Display(Name = "Cost Plus Incentive Fee")]
        CostPlusIncentiveFee = 3,

        [Display(Name = "Time & Material Contacts")]
        TimeAndMaterialContract = 4
    }
}
