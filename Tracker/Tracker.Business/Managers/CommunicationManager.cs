using Microsoft.Extensions.Logging;
using Tracker.Business.Managers.Abstractions;
using Tracker.DataLayer.Entities;

namespace Tracker.Business.Managers
{
    public class CommunicationManager : ICommunicationManager
    {
        private readonly ILogger<CommunicationManager> _logger;

        public CommunicationManager(ILogger<CommunicationManager> logger)
        {
            _logger = logger;
        }

        public async Task SendEmailConfirmationNotificationAsync(ApplicationUser user, string secret)
        {

        }

        public async Task SendForgotPasswordEmailNotificationsAsync(ApplicationUser user, string temporaryPassword, string secret)
        {
            //throw new NotImplementedException();
        }

        public async Task SendWelcomeEmailNotificationsAsync(ApplicationUser user, string password, string secret)
        {
            //throw new NotImplementedException();
        }
    }
}
