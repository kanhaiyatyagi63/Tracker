using Tracker.Core.Data;
using Tracker.DataLayer.Entities;
using Tracker.DataLayer.Repositories.Abstractions;

namespace Tracker.DataLayer.Repositories
{
    public class TimeEntryRepository : Repository<TimeEntry, int>, ITimeEntryRepository
    {
        public TimeEntryRepository(DataContext dbContext) : base(dbContext)
        {
        }
    }
}
