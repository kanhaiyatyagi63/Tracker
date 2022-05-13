using Tracker.Business.Models.TimeEnties;
using Tracker.DataLayer.Entities;

namespace Tracker.Business.Managers.Abstractions
{
    public interface ITimeEntryManager
    {
        Task<bool> CreateAsync(TimeEntryAddModel model);
        Task<bool> UpdateAsync(TimeEntryEditModel model);
        Task<bool> DeleteAsync(int id);
        Task<TimeEntryViewModel> GetAsync(int id);
        Task<IEnumerable<TimeEntryViewModel>> GetAllAsync();
        IQueryable<TimeEntry> GetAllQueryableAsync();
    }
}
