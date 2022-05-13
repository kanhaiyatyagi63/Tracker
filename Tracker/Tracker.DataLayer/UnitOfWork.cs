using Microsoft.Extensions.Logging;
using Tracker.DataLayer.Repositories.Abstractions;

namespace Tracker.DataLayer
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _dbContext;
        private readonly ILogger _logger;

        public UnitOfWork(
            ILogger<UnitOfWork> logger,
            DataContext dbContext,
            IProjectRepository projectRepository,
            ITimeEntryRepository timeEntryRepository)
        {
            _logger = logger;
            _dbContext = dbContext;
            ProjectRepository = projectRepository;
            TimeEntryRepository = timeEntryRepository;
        }
        public IProjectRepository ProjectRepository { get; }
        public ITimeEntryRepository TimeEntryRepository { get; }

        public async Task CommitAsync()
        {
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred on SaveChanges.");
                throw;
            }
        }

        void IDisposable.Dispose()
        {
            if (_dbContext != null)
            {
                _dbContext.Dispose();
            }
        }
    }
}
