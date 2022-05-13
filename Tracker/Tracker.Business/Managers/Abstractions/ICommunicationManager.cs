using Tracker.DataLayer.Entities;

namespace Tracker.Business.Managers.Abstractions
{
    public interface ICommunicationManager
    {
        Task SendWelcomeEmailNotificationsAsync(ApplicationUser user, string password, string secret);
        Task SendEmailConfirmationNotificationAsync(ApplicationUser user, string secret);
        Task SendForgotPasswordEmailNotificationsAsync(ApplicationUser user, string temporaryPassword, string secret);
    }
}
