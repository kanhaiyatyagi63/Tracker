using Tracker.Core.Data;
using Tracker.DataLayer.Entities;
using Tracker.DataLayer.Repositories.Abstractions;

namespace Tracker.DataLayer.Repositories
{
    public class ProjectRepository : Repository<Project, int>, IProjectRepository
    {
        public ProjectRepository(DataContext dbContext) : base(dbContext)
        {
        }
    }
}
