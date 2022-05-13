using Microsoft.AspNetCore.Identity;
using Tracker.Business.Models.Authentication;
using Tracker.Business.Models.User;
using Tracker.DataLayer.Entities;

namespace Tracker.Business.Managers.Abstractions
{
    public interface IApplicationUserManager
    {
        Task<ApplicationUser> FindByIdAsync(string userId);
        Task<ApplicationUser> FindByNameAsync(string userName);
        Task<ApplicationUser> FindByUserNameOrEmail(string userNameOrEmail);
        Task<ApplicationUser> FindByEmail(string email);
        Task<ApplicationUser> FindByEmailExcludingId(string email, string excludeId);
        Task<SignInResult> SignInAsync(ApplicationUser user, string password, bool isPersistant);
        Task LogoutAsync();
        Task<IdentityResult> CreateAsync(ApplicationUser user, string password);
        Task<UserDetailModel> AddUserAsync(UserAddModel model);
        Task<bool> IsInRoleAsync(ApplicationUser user, string role);
        Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role);
        Task<IdentityResult> DeleteAsync(ApplicationUser user);
        Task<(bool IsValid, string ErrorMessage, ApplicationUser User, IEnumerable<string> Roles)> ValidateUser(string userName, string password);
        Task<(bool IsValid, string ErrorMessage, ApplicationUser User, IEnumerable<string> Roles)> ValidateUser(string userId);
        Task<UserDetailModel> GetActiveUserDetail(string userId);
        Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword);
        Task ResetPasswordAsync(ApplicationUser user, string newPassword);
        //Task<IEnumerable<UserModel>> GetAccountManagers();
        Task<(bool Succeeded, string ErrorMessage)> RecoverAccount(string email);
        Task<(bool Succeeded, IEnumerable<string> ErrorMessage)> ActivateAccount(AccountActivateModel model);
        Task SendWelcomeEmailNotificationAsync(string applicationUserId);
        Task<UserDetailModel> GetUserDetail(string userId);
        Task<(bool Succeeded, IEnumerable<string> ErrorMessage)> ResetAccount(AccountResetModel model);
        Task<(bool Succeeded, IEnumerable<string> ErrorMessages)> UpdateUserProfileAsync(UserEditProfileModel model);
        Task DeleteAsync(string id);
        Task ToggleStatus(string id);
        Task<(bool Succeeded, IEnumerable<string> ErrorMessages)> EditUserAsync(UserEditModel model);
        Task<(bool Succeeded, string ErrorMessage)> VerifyEmail(string userId, string token);
        Task<IList<ApplicationUser>> GetSystemUsers();
    }
}
