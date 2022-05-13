using Tracker.Core.Utilities;

namespace Tracker.Business.Managers.Abstractions
{
    public interface IEnumerationManager
    {
        List<SelectListItem<int>> GetContractTypes();
        List<SelectListItem<int>> GetProjectType();
        List<SelectListItem<int>> GetLifeCycleModelType();
        List<SelectListItem<int>> GetActivityType();
    }
}
