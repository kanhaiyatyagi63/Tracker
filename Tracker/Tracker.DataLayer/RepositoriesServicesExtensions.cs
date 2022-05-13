using Microsoft.Extensions.DependencyInjection;
using Tracker.DataLayer.Repositories;
using Tracker.DataLayer.Repositories.Abstractions;

namespace Tracker.DataLayer
{
    public static class RepositoriesServicesExtension
    {
        public static IServiceCollection ConfigureRepositoriesServices(this IServiceCollection services)
        {
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ITimeEntryRepository, TimeEntryRepository>();
            return services;
        }
    }
}
