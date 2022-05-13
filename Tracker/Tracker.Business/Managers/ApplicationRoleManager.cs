using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tracker.Business.Managers.Abstractions;
using Tracker.Business.Models.Role;
using Tracker.Core.Services.Abstractions;
using Tracker.DataLayer;
using Tracker.DataLayer.Entities;
using Tracker.DataLayer.Enumerations;

namespace Tracker.Business.Managers
{
    public class ApplicationRoleManager : IApplicationRoleManager
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<ApplicationRoleManager> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;

        public ApplicationRoleManager(ILogger<ApplicationRoleManager> logger,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            RoleManager<ApplicationRole> roleManager,
            IUserContextService userContextService)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _roleManager = roleManager;
            _userContextService = userContextService;
        }

        public IEnumerable<ApplicationRole> Roles => _roleManager.Roles;

        public async Task<IdentityResult> CreateAsync(ApplicationRole role)
        {
            return await _roleManager.CreateAsync(role);
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationRole role)
        {
            return await _roleManager.UpdateAsync(role);
        }

        public async Task<ApplicationRole> FindByNameAsync(string roleName)
        {
            return await _roleManager.FindByNameAsync(roleName);
        }

        public async Task<ApplicationRole> FindByIdAsync(string id)
        {
            return await _roleManager.FindByIdAsync(id);
        }

        public async Task<IList<ApplicationRole>> GetRoles(IEnumerable<string> roleNames)
        {
            var roles = await _roleManager.Roles.Where(x => x.IsActive && !x.IsDeleted && roleNames.Contains(x.Name))
                .ToListAsync();

            return roles;
        }

        public async Task<IEnumerable<RoleModel>> GetRoles()
        {
            var roles = await _roleManager.Roles.Where(x => x.IsActive && x.IsDeleted == false)
                .ToListAsync();

            return _mapper.Map<IEnumerable<RoleModel>>(roles).OrderBy(x => x.Name);
        }

        public async Task<IEnumerable<RoleModel>> GetRoles(ApplicationRoleType applicationRoleType)
        {
            var roles = await _roleManager.Roles.Where(x => x.IsActive && x.IsDeleted == false && x.ApplicationRoleType == applicationRoleType)
                .ToListAsync();
            return _mapper.Map<IEnumerable<RoleModel>>(roles);
        }


        public async Task<RoleDetailModel> GetRole(string id)
        {
            try
            {
                var role = await _roleManager.Roles
                    .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);

                if (role == null)
                {
                    throw new Exception("Role has been deleted.");
                }

                return new RoleDetailModel()
                {
                    Id = role.Id,
                    Name = role.Name,
                    ApplicationRoleType = role.ApplicationRoleType
                };
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error occurred in GetRoleAndPermission");
                throw;
            }
        }

        public async Task<RoleDetailModel> AddRoleAsync(RoleAddModel model)
        {
            try
            {
                var applicationRole = await FindByNameAsync(model.Name);
                if (applicationRole != null)
                {
                    throw new Exception("Role with name already exists.");
                }

                IdentityResult identityResult = await CreateAsync(new ApplicationRole()
                {
                    Name = model.Name,
                    NormalizedName = model.Name.Replace(' ', '_'),
                    ApplicationRoleType = model.ApplicationRoleType,
                    IsSystemGenerated = false,
                    IsActive = true
                });

                if (!identityResult.Succeeded)
                {
                    throw new Exception("Role creation failed.");
                }

                applicationRole = await FindByNameAsync(model.Name);
                if (applicationRole == null)
                {
                    throw new ArgumentNullException(nameof(applicationRole));
                }

                await _unitOfWork.CommitAsync();

                return new RoleDetailModel()
                {
                    Id = applicationRole.Id,
                    Name = applicationRole.Name,
                    ApplicationRoleType = applicationRole.ApplicationRoleType
                };
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error occurred during AddRole");
                throw;
            }
        }

        public async Task<RoleDetailModel> EditRoleAsync(RoleAddModel model)
        {
            try
            {
                //IdentityResult identityResult;
                var applicationRole = await FindByIdAsync(model.Id);
                if (applicationRole == null)
                {
                    throw new Exception("Role has been deleted.");
                }

                applicationRole.ApplicationRoleType = model.ApplicationRoleType;
                IdentityResult identityResult = await UpdateAsync(applicationRole);

                //Role Update failed take appropriate action
                if (!identityResult.Succeeded)
                {
                    throw new Exception("Role Update failed.");
                }

                await _unitOfWork.CommitAsync();

                return new RoleDetailModel()
                {
                    Id = applicationRole.Id,
                    Name = applicationRole.Name,
                    ApplicationRoleType = applicationRole.ApplicationRoleType,
                };
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error occurred during EditRole");
                throw;
            }
        }

        public async Task DeleteAsync(string id)
        {
            var role = await FindByIdAsync(id);
            if (role == null)
                throw new Exception("Role doesn't exist.");

            try
            {
                role.IsDeleted = true;
                await _roleManager.UpdateAsync(role);

                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting role");
            }
        }

        public async Task ToggleStatusAsync(string id)
        {
            try
            {
                var role = await FindByIdAsync(id);
                if (role == null)
                    throw new Exception("Role doesn't exist.");

                role.IsActive = !role.IsActive;

                await UpdateAsync(role);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error occurred in ApplicationRole- ToggleStatus");
                throw;
            }
        }
    }
}
