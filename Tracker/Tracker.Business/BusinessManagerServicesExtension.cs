using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tracker.Business.Managers;
using Tracker.Business.Managers.Abstractions;
using Tracker.Business.Profile;

namespace Tracker.Business
{
    public static class BusinessManagerServicesExtension
    {
        public static IServiceCollection ConfigureBusinessManagerServices(this IServiceCollection services, IConfiguration configuration)
        {
            //Add Mapping Dependency
            services.AddAutoMapper(typeof(AutoMappingProfile));
            services.AddScoped<ISeedManager, SeedManager>();
            services.AddScoped<IApplicationRoleManager, ApplicationRoleManager>();
            services.AddScoped<IApplicationUserManager, ApplicationUserManager>();
            services.AddScoped<ICommunicationManager, CommunicationManager>();
            services.AddScoped<IProjectManager, ProjectManager>();
            services.AddScoped<IEnumerationManager, EnumerationManager>();
            services.AddScoped<ITimeEntryManager, TimeEntryManager>();
            return services;
        }
    }
}
