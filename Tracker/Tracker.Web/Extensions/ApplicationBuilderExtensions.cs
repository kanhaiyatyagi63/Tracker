using Microsoft.EntityFrameworkCore;
using Tracker.Business.Managers.Abstractions;
using Tracker.DataLayer;

namespace Tracker.Web.Extensions
{
    public static class ApplicationBuilderExtensions
    {

        public static IApplicationBuilder Migrate(this IApplicationBuilder app, ILogger logger)
        {
            logger.LogInformation("Inside Migrate");
            try
            {
                var serviceFactory = app.ApplicationServices.GetService<IServiceScopeFactory>();
                if (serviceFactory != null)
                {
                    using var scope = serviceFactory.CreateScope();
                    var services = scope.ServiceProvider;

                    //Temporary: This should be removed
                    var dbContext = services.GetRequiredService<DataContext>();
                    if (dbContext.Database.GetPendingMigrations().Any())
                    {
                        logger.LogInformation("Migrate: Migration Pending executing migration.");
                        dbContext.Database.Migrate();
                    }
                }

                logger.LogInformation("Migrate: Completed.");
            }
            catch (Exception exception)
            {
                logger.LogCritical(exception, "Critical: Database Migration failed.");
            }
            logger.LogInformation("Exiting Migrate");
            return app;

        }

        public static IApplicationBuilder InitializeDatabase(this IApplicationBuilder app, ILogger logger)
        {
            logger.LogInformation("Inside InitializeDatabase");
            try
            {
                var serviceFactory = app.ApplicationServices.GetService<IServiceScopeFactory>();
                if (serviceFactory != null)
                {
                    using var scope = serviceFactory.CreateScope();
                    var services = scope.ServiceProvider;

                    logger.LogInformation("InitializeDatabase: Getting Seed Manager.");
                    var seedManager = services.GetRequiredService<ISeedManager>();

                    logger.LogInformation("InitializeDatabase: Wait for Seed to Complete.");
                    seedManager.SeedAsync().Wait();
                }
            }
            catch (Exception exception)
            {
                logger.LogCritical(exception, "Critical: Database Seed failed.");
            }

            logger.LogInformation("Exiting InitializeDatabase");
            return app;
        }
    }
}
