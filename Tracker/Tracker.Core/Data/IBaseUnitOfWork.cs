namespace Tracker.Core.Data
{
    public interface IBaseUnitOfWork : IDisposable
    {
        Task CommitAsync();
    }
}
