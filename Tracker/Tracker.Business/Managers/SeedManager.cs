using Microsoft.Extensions.Logging;
using Tracker.Business.Managers.Abstractions;
using Tracker.DataLayer;
using Tracker.DataLayer.Entities;
using Tracker.DataLayer.Enumerations;

namespace Tracker.Business.Managers
{
    public class SeedManager : ISeedManager
    {
        private const string AdminRole = "Admin";
        private readonly ILogger _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IApplicationRoleManager _applicationRoleManager;
        private readonly IApplicationUserManager _applicationUserManager;

        public SeedManager(
            ILogger<SeedManager> logger,
            IUnitOfWork unitOfWork,
            IApplicationUserManager applicationUserManager,
            IApplicationRoleManager applicationRoleManager)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _applicationRoleManager = applicationRoleManager;
            _applicationUserManager = applicationUserManager;
        }

        public async Task SeedAsync()
        {
            await SeedRolesAsync();
            //await SeedUser1Async();
            //await SeedUser2Async();

            await SeedAdminAsync();
        }

        private async Task SeedAdminAsync()
        {
            try
            {
                if (!_applicationRoleManager.Roles.Any())
                {
                    _logger.LogError("Admin user seed failed, no role found");
                }

                var user = await _applicationUserManager.FindByNameAsync("admin");
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        Name = "admin",
                        UserName = "admin",
                        Email = "admin@tt.com",
                        EmailConfirmed = true,
                        IsActive = true,
                        PhoneCode = "+91",
                        ApplicationRoleType = ApplicationRoleType.Admin
                    };

                    var result = await _applicationUserManager.CreateAsync(user, "admin@123");
                    if (!result.Succeeded)
                    {
                        _logger.LogError("admin seed failed");
                        _logger.LogError(string.Join(",", result.Errors.Select(x => x.Description)));
                        return;
                    }

                }

                var isInRole = await _applicationUserManager.IsInRoleAsync(user, "Admin");
                if (!isInRole)
                {
                    var result = await _applicationUserManager.AddToRoleAsync(user, "Admin");
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("admin seed completed");
                        return;
                    }

                    _logger.LogError("admin seed failed");
                    _logger.LogError(string.Join(",", result.Errors.Select(x => x.Description)));

                    await _applicationUserManager.DeleteAsync(user);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in admin seeding {ex}");
            }
        }

        private async Task SeedUser1Async()
        {
            try
            {
                if (!_applicationRoleManager.Roles.Any())
                {
                    _logger.LogError("Admin user seed failed, no role found");
                }

                var user = await _applicationUserManager.FindByNameAsync("admin");
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        Name = "user1",
                        UserName = "user1",
                        Email = "user1@tt.com",
                        EmailConfirmed = true,
                        IsActive = true,
                        PhoneCode = "+91",
                        ApplicationRoleType = ApplicationRoleType.User
                    };

                    var result = await _applicationUserManager.CreateAsync(user, "user@123");
                    if (!result.Succeeded)
                    {
                        _logger.LogError("user seed failed");
                        _logger.LogError(string.Join(",", result.Errors.Select(x => x.Description)));
                        return;
                    }

                    user = await _applicationUserManager.FindByNameAsync("user1");
                }

                var isInRole = await _applicationUserManager.IsInRoleAsync(user, "User");
                if (!isInRole)
                {
                    var result = await _applicationUserManager.AddToRoleAsync(user, "User");
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("user seed completed");
                        return;
                    }

                    _logger.LogError("user seed failed");
                    _logger.LogError(string.Join(",", result.Errors.Select(x => x.Description)));

                    await _applicationUserManager.DeleteAsync(user);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in system user seeding {ex}");
            }
        }

        private async Task SeedUser2Async()
        {
            try
            {
                if (!_applicationRoleManager.Roles.Any())
                {
                    _logger.LogError("Admin user seed failed, no role found");
                }

                var user = await _applicationUserManager.FindByNameAsync("admin");
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        Name = "user2",
                        UserName = "user2",
                        Email = "user2@tt.com",
                        EmailConfirmed = true,
                        IsActive = true,
                        PhoneCode = "+91",
                        ApplicationRoleType = ApplicationRoleType.User
                    };

                    var result = await _applicationUserManager.CreateAsync(user, "user@123");
                    if (!result.Succeeded)
                    {
                        _logger.LogError("user seed failed");
                        _logger.LogError(string.Join(",", result.Errors.Select(x => x.Description)));
                        return;
                    }

                    user = await _applicationUserManager.FindByNameAsync("user2");
                }

                var isInRole = await _applicationUserManager.IsInRoleAsync(user, "User");
                if (!isInRole)
                {
                    var result = await _applicationUserManager.AddToRoleAsync(user, "User");
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("user seed completed");
                        return;
                    }

                    _logger.LogError("user seed failed");
                    _logger.LogError(string.Join(",", result.Errors.Select(x => x.Description)));

                    await _applicationUserManager.DeleteAsync(user);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in system user seeding {ex}");
            }
        }

        private async Task SeedRolesAsync()
        {
            var roles = new List<SeedApplicationRole>
            {
                new SeedApplicationRole
                {
                    Name = AdminRole,
                    Description = "Administrator of System",
                    NormalizedName = "admin",
                    ApplicationRoleType = ApplicationRoleType.Admin,
                },
                new SeedApplicationRole
                {
                    Name = "User",
                    Description = "User of the system",
                    NormalizedName = "user",
                    ApplicationRoleType = ApplicationRoleType.User,

                }
            };
            foreach (var role in roles)
            {
                try
                {
                    var applicationRole = await _applicationRoleManager.FindByNameAsync(role.Name);
                    if (applicationRole == null)
                    {
                        var identityResult = await _applicationRoleManager.CreateAsync(new ApplicationRole()
                        {
                            Name = role.Name,
                            NormalizedName = role.NormalizedName,
                            IsSystemGenerated = true,
                            IsActive = true,
                            Description = role.Description,
                            ApplicationRoleType = role.ApplicationRoleType
                        });

                        _logger.LogInformation($"Role ({role.Name}) seed result: {identityResult}");

                        if (identityResult.Succeeded)
                        {
                            applicationRole = await _applicationRoleManager.FindByNameAsync(role.Name);
                        }
                    }


                    await _unitOfWork.CommitAsync();
                    _logger.LogInformation($"Permissions for role ({role.Name}) added");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error in Seeding Role {role.Name}");
                }
            }
        }
        private class SeedApplicationRole
        {
            public string Name { get; set; }
            public string? Description { get; set; }
            public string NormalizedName { get; internal set; }
            public ApplicationRoleType ApplicationRoleType { get; internal set; }
        }
    }
}
