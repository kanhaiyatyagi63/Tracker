namespace Tracker.Core.Services.Abstractions
{
    public interface IUserContextService
    {
        string? GetUserId();

        string? GetUserName();

        bool IsAdmin();
    }
}
