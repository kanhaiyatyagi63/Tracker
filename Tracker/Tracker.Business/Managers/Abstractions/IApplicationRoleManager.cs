using Microsoft.AspNetCore.Identity;
using Tracker.Business.Models.Role;
using Tracker.DataLayer.Entities;
using Tracker.DataLayer.Enumerations;

namespace Tracker.Business.Managers.Abstractions
{
    public interface IApplicationRoleManager
    {
        IEnumerable<ApplicationRole> Roles { get; }
        Task<ApplicationRole> FindByNameAsync(string roleName);
        Task<IdentityResult> CreateAsync(ApplicationRole role);
        Task<IdentityResult> UpdateAsync(ApplicationRole role);
        Task<IList<ApplicationRole>> GetRoles(IEnumerable<string> roleNames);
        Task<IEnumerable<RoleModel>> GetRoles();
        Task<IEnumerable<RoleModel>> GetRoles(ApplicationRoleType applicationRoleType);
        Task<RoleDetailModel> AddRoleAsync(RoleAddModel model);
        Task<RoleDetailModel> EditRoleAsync(RoleAddModel model);
        Task DeleteAsync(string id);
        Task ToggleStatusAsync(string id);
    }
}
