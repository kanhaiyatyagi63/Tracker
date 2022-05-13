using Tracker.Core.Data;
using Tracker.DataLayer.Repositories.Abstractions;

namespace Tracker.DataLayer
{
    public interface IUnitOfWork : IBaseUnitOfWork
    {
        public IProjectRepository ProjectRepository { get; }
        public ITimeEntryRepository TimeEntryRepository { get; }
    }
}
