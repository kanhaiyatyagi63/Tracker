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
            await SeedAdminUserAsync();
            await SeedSuperAdminAsync();
        }

        private async Task SeedSuperAdminAsync()
        {
            try
            {
                if (!_applicationRoleManager.Roles.Any())
                {
                    _logger.LogError("Admin user seed failed, no role found");
                }

                var user = await _applicationUserManager.FindByNameAsync("superadmin");
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        Name = "superadmin",
                        UserName = "superadmin",
                        Email = "kanhaya.tyagi+1@successive.tech",
                        EmailConfirmed = true,
                        IsActive = true,
                        PhoneCode = "+91",
                        ApplicationRoleType = ApplicationRoleType.SuperAdmin
                    };

                    var result = await _applicationUserManager.CreateAsync(user, "Password@123");
                    if (!result.Succeeded)
                    {
                        _logger.LogError("Super admin seed failed");
                        _logger.LogError(string.Join(",", result.Errors.Select(x => x.Description)));
                        return;
                    }

                    //user = await _applicationUserManager.FindByNameAsync("admin");
                }

                var isInRole = await _applicationUserManager.IsInRoleAsync(user, "SuperAdmin");
                if (!isInRole)
                {
                    var result = await _applicationUserManager.AddToRoleAsync(user, "SuperAdmin");
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("Super admin seed completed");
                        return;
                    }

                    _logger.LogError("Super Admin seed failed");
                    _logger.LogError(string.Join(",", result.Errors.Select(x => x.Description)));

                    await _applicationUserManager.DeleteAsync(user);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in super admin seeding {ex}");
            }
        }

        private async Task SeedAdminUserAsync()
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
                        Email = "kanhaya.tyagi+1@successive.tech",
                        EmailConfirmed = true,
                        IsActive = true,
                        PhoneCode = "+91",
                        ApplicationRoleType = ApplicationRoleType.Admin
                    };

                    var result = await _applicationUserManager.CreateAsync(user, "Password@123");
                    if (!result.Succeeded)
                    {
                        _logger.LogError("Admin user seed failed");
                        _logger.LogError(string.Join(",", result.Errors.Select(x => x.Description)));
                        return;
                    }

                    user = await _applicationUserManager.FindByNameAsync("admin");
                }

                var isInRole = await _applicationUserManager.IsInRoleAsync(user, AdminRole);
                if (!isInRole)
                {
                    var result = await _applicationUserManager.AddToRoleAsync(user, AdminRole);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("Admin user seed completed");
                        return;
                    }

                    _logger.LogError("Admin user seed failed");
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
                    Name = "SuperAdmin",
                    Description = "Super administrator of System",
                    NormalizedName = "superadmin",
                    ApplicationRoleType = ApplicationRoleType.SuperAdmin,
                },
                new SeedApplicationRole
                {
                    Name = "User",
                    Description = "User of the system",
                    NormalizedName = "user",
                    ApplicationRoleType = ApplicationRoleType.User,

                },
                 new SeedApplicationRole
                {
                    Name = "SystemUser",
                    Description = "System user of the system",
                    NormalizedName = "systemuser",
                    ApplicationRoleType = ApplicationRoleType.System,

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
