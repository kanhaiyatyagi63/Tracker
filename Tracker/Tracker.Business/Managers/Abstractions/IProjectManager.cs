using Tracker.Business.Models.Project;
using Tracker.DataLayer.Entities;

namespace Tracker.Business.Managers.Abstractions
{
    public interface IProjectManager
    {
        Task<bool> CreateAsync(ProjectAddModel model);
        Task<bool> UpdateAsync(ProjectEditModel model);
        Task<bool> DeleteAsync(int id);
        Task<ProjectViewModel> GetAsync(int id);
        Task<IEnumerable<ProjectViewModel>> GetAllAsync();
        (IEnumerable<ProjectViewModel>, int) GetDataTableRecordAsync(string sortColumn, string sortColumnDirection,
            string searchValue, int recordsTotal, int skip, int pageSize);
    }
}
