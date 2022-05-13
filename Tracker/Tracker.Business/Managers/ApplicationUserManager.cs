using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Tracker.Business.Constants;
using Tracker.Business.Managers.Abstractions;
using Tracker.Business.Models.Authentication;
using Tracker.Business.Models.User;
using Tracker.Core.Services.Abstractions;
using Tracker.DataLayer;
using Tracker.DataLayer.Entities;
using Tracker.DataLayer.Enumerations;

namespace Tracker.Business.Managers
{
    public class ApplicationUserManager : IApplicationUserManager
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IOptions<IdentityOptions> _identityOptions;
        private readonly IPasswordGeneratorService _passwordGenerator;
        private readonly ICommunicationManager _communicationManager;
        private readonly ILogger<ApplicationUserManager> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly string _tokenDeliminator = "||";
        private readonly IUserContextService _userContextService;
        private const string GenericMessage = "Something went wrong";

        public ApplicationUserManager(ILogger<ApplicationUserManager> logger,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IOptions<IdentityOptions> identityOptions,
            IPasswordGeneratorService passwordGenerator,
            ICommunicationManager communicationManager,
            IUserContextService userContextService,
            SignInManager<ApplicationUser> signInManager
            )
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
            _identityOptions = identityOptions;
            _passwordGenerator = passwordGenerator;
            _communicationManager = communicationManager;
            _userContextService = userContextService;
            _signInManager = signInManager;
        }

        public async Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role)
        {
            return await _userManager.AddToRoleAsync(user, role);
        }

        public async Task<SignInResult> SignInAsync(ApplicationUser user, string password, bool isPersistant)
        {
            var result = await _signInManager.PasswordSignInAsync(user, password, isPersistant, false);
            return result;
        }
        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
        /// <summary>
        /// Used in Seed Manager to create default users. Should not be used in other places
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<IdentityResult> DeleteAsync(ApplicationUser user)
        {
            return await _userManager.DeleteAsync(user);
        }

        public async Task<ApplicationUser> FindByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<ApplicationUser> FindByNameAsync(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }

        public async Task<bool> IsInRoleAsync(ApplicationUser user, string role)
        {
            return await _userManager.IsInRoleAsync(user, role);
        }

        public async Task<ApplicationUser> FindByEmail(string email)
        {
            return await _userManager.Users.FirstOrDefaultAsync(x => !x.IsDeleted && x.Email.Equals(email));
        }
        public async Task<ApplicationUser> FindByEmailExcludingId(string email, string excludeId)
        {
            return await _userManager.Users.FirstOrDefaultAsync(x => x.Id != excludeId && !x.IsDeleted && x.Email.Equals(email));
        }

        public async Task<ApplicationUser> FindByUserNameOrEmail(string userNameOrEmail)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(userNameOrEmail);
                if (user != null)
                {
                    return user;
                }

                return await FindByEmail(userNameOrEmail);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error occurred in FindByUserNameOrEmail");
                throw;
            }
        }



        public async Task<(bool IsValid, string ErrorMessage, ApplicationUser User, IEnumerable<string> Roles)> ValidateUser(string userName, string password)
        {
            try
            {
                var user = await FindByUserNameOrEmail(userName);

                if (user == null || user.IsDeleted || !await _userManager.CheckPasswordAsync(user, password))
                {
                    return (false, "The username and password do not match, please try again or reset the password.", null, null);
                }

                if (!user.IsActive)
                {
                    return (false, "Your account is inactive. Please contact to administrator.", null, null);
                }
                //Get User Role Name
                var roleNames = await _userManager.GetRolesAsync(user);

                return (true, null, user, roleNames);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error occurred in ValidateUser");
                throw;
            }
        }

        public async Task<(bool IsValid, string ErrorMessage, ApplicationUser User, IEnumerable<string> Roles)> ValidateUser(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null || user.IsDeleted)
                {
                    return (false, "The username and password do not match, please try again or reset the password.", null, null);
                }

                if (!user.AccountActivated)
                {
                    return (false, "Please check your mail to activate account or please contact Administrator.", null, null);
                }

                if (!user.IsActive)
                {
                    return (false, "Your account is disabled. Please contact Administrator.", null, null);
                }

                //Get User Role Name
                var roleNames = await _userManager.GetRolesAsync(user);

                return (true, null, user, roleNames);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error occurred in ValidateUser");
                throw;
            }
        }

        public async Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        }

        public async Task ResetPasswordAsync(ApplicationUser user, string newPassword)
        {
            try
            {
                var result = await _userManager.RemovePasswordAsync(user);
                if (result.Succeeded)
                {
                    _logger.LogInformation($"Password for {user.Id} removed");

                    var addPasswordResult = await _userManager.AddPasswordAsync(user, newPassword);

                    if (addPasswordResult.Succeeded)
                    {
                        _logger.LogInformation($"Password for {user.Id} changed");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while changing password for  {user.Id} ");
            }
        }

        public async Task<(bool Succeeded, string ErrorMessage)> RecoverAccount(string userNameOrEmail)
        {
            try
            {
                var user = await FindByUserNameOrEmail(userNameOrEmail);
                if (user == null)
                {
                    _logger.LogError("User not found. Email/User Id: " + userNameOrEmail);
                    return (true, null);
                }

                if (!user.AccountActivated)
                {
                    return (false, "Account verification pending. Please check the email and verify the account.");
                }

                if (!user.EmailConfirmed)
                {
                    return (false, "Email verification pending. Please verify the email.");
                }

                var secret = Guid.NewGuid().ToString("N");
                var temporaryPassword = GeneratePassword();
                var passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

                //Validate old claims and remove old password claim if exists 
                var userClaims = await _userManager.GetClaimsAsync(user);
                var passwordResetTokenClaim = userClaims.SingleOrDefault(x => x.Type.Equals(CustomClaimTypesConstants.TemporaryPasswordToken));
                if (passwordResetTokenClaim != null)
                {
                    await _userManager.RemoveClaimAsync(user, passwordResetTokenClaim);
                }

                //Generate New Claims
                var token = string.Join(_tokenDeliminator, temporaryPassword, DateTime.UtcNow.AddHours(12).Ticks, secret, passwordResetToken);
                passwordResetTokenClaim = new Claim(CustomClaimTypesConstants.TemporaryPasswordToken, token);
                await _userManager.AddClaimAsync(user, passwordResetTokenClaim);

                await _communicationManager.SendForgotPasswordEmailNotificationsAsync(user, temporaryPassword, secret);

                await _unitOfWork.CommitAsync();

                return (true, null);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error occurred in RecoverAccount");
                throw;
            }
        }

        public async Task<(bool Succeeded, IEnumerable<string> ErrorMessage)> ActivateAccount(AccountActivateModel model)
        {
            try
            {
                _logger.LogDebug($"Activate Account: Check user exist for user id {model.Key}");
                var user = await _userManager.FindByIdAsync(model.Key);
                if (user == null)
                {
                    return (false, new List<string>() { GenericMessage });
                }

                _logger.LogDebug($"Activate Account: Get claims for user with id {model.Key}");
                var userClaims = await _userManager.GetClaimsAsync(user);
                var welcomeEmailConfirmationTokenClaim = userClaims.FirstOrDefault(x => x.Type.Equals(CustomClaimTypesConstants.WelcomeEmailConfirmationToken));
                if (welcomeEmailConfirmationTokenClaim == null)
                {
                    return (false, new List<string>() { GenericMessage });
                }

                _logger.LogDebug($"Activate Account: Validate Secret {model.Key}");
                var claimValues = welcomeEmailConfirmationTokenClaim.Value.Split(_tokenDeliminator);
                var secret = claimValues[0];
                var temporaryPassword = claimValues[1];
                var token = claimValues[2];
                if (!model.Secret.Equals(secret, StringComparison.CurrentCultureIgnoreCase))
                {
                    return (false, new List<string>() { GenericMessage });
                }

                _logger.LogDebug($"Activate Account: Validate Temporary password for user {model.Key}");
                if (!temporaryPassword.Equals(model.TemporaryPassword, StringComparison.CurrentCultureIgnoreCase))
                {
                    return (false, new List<string>() { GenericMessage });
                }

                IdentityResult result;
                if (!user.AccountActivated)
                {
                    //Set Account as activated
                    user.AccountActivated = true;
                    result = await _userManager.UpdateAsync(user);
                    if (!result.Succeeded)
                    {
                        return (false, result.Errors.Select(x => x.Description));
                    }

                    _logger.LogDebug($"Activate Account: Confirm Email is  user with id {model.Key}");
                    result = await _userManager.ConfirmEmailAsync(user, token);
                    if (!result.Succeeded)
                    {
                        return (false, result.Errors.Select(x => x.Description));
                    }

                    _logger.LogDebug("Activate Account: Email confirmed hence remove claim");
                    await _userManager.RemoveClaimAsync(user, welcomeEmailConfirmationTokenClaim);
                }

                _logger.LogDebug($"Activate Account: Add Password for user {model.Key}");
                result = await _userManager.AddPasswordAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    return (false, result.Errors.Select(x => x.Description));
                }

                return (true, null);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error occurred in ActivateAccount");
                throw;
            }
        }

        public async Task SendWelcomeEmailNotificationAsync(string applicationUserId)
        {
            try
            {
                var user = await FindByIdAsync(applicationUserId);
                if (user == null)
                {
                    _logger.LogDebug("User with id doesn't exists");
                    return;
                }

                //Email is confirmed
                if (user.AccountActivated)
                {
                    return;
                }

                await SendWelcomeEmailNotificationAsync(user);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error occurred during SendWelcomeNotification");
                throw;
            }
        }

        private async Task SendWelcomeEmailNotificationAsync(ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            //Email is confirmed
            if (user.EmailConfirmed)
            {
                return;
            }

            try
            {
                _logger.LogDebug($"SendWelcomeNotificationAsync: Remove previous activation email claim");
                //Validate old claims and remove activation claim 
                var userClaims = await _userManager.GetClaimsAsync(user);
                foreach (var uc in userClaims.Where(x => x.Type.Equals(CustomClaimTypesConstants.WelcomeEmailConfirmationToken)))
                {
                    await _userManager.RemoveClaimAsync(user, uc);
                }
                _logger.LogDebug($"SendWelcomeNotificationAsync: Remove previous activation email claim Completed");


                _logger.LogDebug($"SendWelcomeNotificationAsync: Create Activation Claim and Url");
                var password = GeneratePassword();
                var secret = Guid.NewGuid().ToString("N");
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var claim = new Claim(CustomClaimTypesConstants.WelcomeEmailConfirmationToken, string.Join(_tokenDeliminator, secret, password, token));
                await _userManager.AddClaimAsync(user, claim);
                _logger.LogDebug("SendWelcomeNotificationAsync: Create Activation Claim and Url completed");

                _logger.LogDebug("SendWelcomeNotificationAsync: Send Welcome email");
                await _communicationManager.SendWelcomeEmailNotificationsAsync(user, password, secret);

                await _unitOfWork.CommitAsync();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error occurred during SendWelcomeNotification");
                throw;
            }
        }

        private async Task SendEmailConfirmationNotificationAsync(ApplicationUser user)
        {
            if (!user.AccountActivated)
            {
                _logger.LogError($"Account not activated. Hence, should not send Email change for {user.UserName}");
                return;
            }
            try
            {
                _logger.LogDebug("SendEmailConfirmationNotificationAsync: Remove previous email verification claim");
                //Validate old claims and remove activation claim 
                var userClaims = await _userManager.GetClaimsAsync(user);
                foreach (var uc in userClaims.Where(x => x.Type.Equals(CustomClaimTypesConstants.EmailConfirmationToken)))
                {
                    await _userManager.RemoveClaimAsync(user, uc);
                }
                _logger.LogDebug("SendEmailConfirmationNotificationAsync: Remove previous email verification claim Completed");


                _logger.LogDebug("SendEmailConfirmationNotificationAsync: Create email verification claim");
                var secret = Guid.NewGuid().ToString("N");
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var claim = new Claim(CustomClaimTypesConstants.EmailConfirmationToken, string.Join(_tokenDeliminator, secret, token));
                await _userManager.AddClaimAsync(user, claim);
                _logger.LogDebug("SendEmailConfirmationNotificationAsync: Create email verification claim completed");

                _logger.LogDebug("SendEmailConfirmationNotificationAsync: Send email verification email");
                await _communicationManager.SendEmailConfirmationNotificationAsync(user, secret);
                _logger.LogDebug("SendEmailConfirmationNotificationAsync: Send email verification email completed");

                await _unitOfWork.CommitAsync();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error occurred during SendEmailConfirmationNotificationAsync");
                throw;
            }
        }

        public async Task<(bool Succeeded, IEnumerable<string> ErrorMessage)> ResetAccount(AccountResetModel model)
        {
            try
            {
                _logger.LogDebug($"Reset Account: Check user exist for user id {model.Key}");
                var user = await _userManager.FindByIdAsync(model.Key);
                if (user == null)
                {
                    return (false, new List<string>() { GenericMessage });
                }

                _logger.LogDebug($"Reset Account: Get claims for user with id {model.Key}");
                var userClaims = await _userManager.GetClaimsAsync(user);
                var resetPasswordTokenClaim = userClaims.FirstOrDefault(x => x.Type.Equals(CustomClaimTypesConstants.TemporaryPasswordToken));
                if (resetPasswordTokenClaim == null)
                {
                    return (false, new List<string>() { "Either key or secret is invalid or link is already used, please regenerate reset link once again" });
                }

                _logger.LogDebug($"Reset Account: Validate Secret {model.Key}");
                var claimValues = resetPasswordTokenClaim.Value.Split(_tokenDeliminator);
                var temporaryPassword = claimValues[0];
                var expiryTicks = Convert.ToInt64(claimValues[1]);
                var secret = claimValues[2];
                var token = claimValues[3];

                _logger.LogDebug($"Reset Account: Validate Secret for user {model.Key}");
                if (!secret.Equals(model.Secret, StringComparison.CurrentCultureIgnoreCase))
                {
                    return (false, new List<string>() { GenericMessage });
                }

                _logger.LogDebug($"Reset Account: Validate Temporary password for user {model.Key}");
                if (!temporaryPassword.Equals(model.TemporaryPassword, StringComparison.CurrentCultureIgnoreCase))
                {
                    return (false, new List<string>() { GenericMessage });
                }

                _logger.LogDebug($"Reset Account: Validate Expiry duration for user {model.Key}");
                if (expiryTicks < DateTime.UtcNow.Ticks)
                {
                    await _userManager.RemoveClaimAsync(user, resetPasswordTokenClaim);
                    return (false, new List<string>() { "Expired temporary password, please regenerate reset link once again" });
                }

                _logger.LogDebug($"Reset Account: Reset password expiry for user {model.Key}");
                var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
                if (!result.Succeeded)
                {
                    return (false, result.Errors.Select(x => x.Description));
                }

                await _userManager.RemoveClaimAsync(user, resetPasswordTokenClaim);
                return (true, null);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error occurred in ResetAccount");
                throw;
            }
        }

        public async Task<(bool Succeeded, string ErrorMessage)> VerifyEmail(string userId, string inputSecret)
        {
            try
            {
                _logger.LogDebug($"Verify Email: Check user exist for user id: {userId}");
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return (false, GenericMessage);
                }

                _logger.LogDebug($"Verify Email: Get claims for user with id {userId}");
                var userClaims = await _userManager.GetClaimsAsync(user);
                var tokenClaim = userClaims.FirstOrDefault(x => x.Type.Equals(CustomClaimTypesConstants.EmailConfirmationToken));
                if (tokenClaim == null)
                {
                    return (false, "Either key or secret is invalid or link is already used, please regenerate verification link once again");
                }

                _logger.LogDebug($"Verify Email: Validate Secret {userId}");
                var claimValues = tokenClaim.Value.Split(_tokenDeliminator);
                var secret = claimValues[0];
                var token = claimValues[1];

                if (!secret.Equals(inputSecret, StringComparison.CurrentCultureIgnoreCase))
                {
                    return (false, GenericMessage);
                }

                _logger.LogDebug($"Reset Account: Reset password expiry for user {userId}");
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (!result.Succeeded)
                {
                    return (false, result.Errors.FirstOrDefault()?.Description);
                }

                await _userManager.RemoveClaimAsync(user, tokenClaim);
                return (true, null);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error occurred in VerifyEmail");
                throw;
            }
        }

        public async Task<UserDetailModel> AddUserAsync(UserAddModel model)
        {
            try
            {
                _logger.LogDebug($"CreateUserAsync: Account creation for user with email {model.Email} and phone {model.PhoneNumber} started");

                _logger.LogDebug("CreateUserAsync: Validation Starts");
                if (model == null)
                {
                    throw new ArgumentNullException(nameof(model));
                }
                if (string.IsNullOrEmpty(model.Name))
                {
                    throw new ArgumentNullException(nameof(model.Name));
                }
                if (string.IsNullOrEmpty(model.Email))
                {
                    throw new ArgumentNullException(nameof(model.Email));
                }
                if (string.IsNullOrEmpty(model.PhoneNumber))
                {
                    throw new ArgumentNullException(nameof(model.PhoneNumber));
                }
                if (model.Roles == null || model.Roles.Length == 0)
                {
                    throw new ArgumentNullException(nameof(model.Roles));
                }


                var existingUserWithEmail = await FindByEmail(model.Email);
                if (existingUserWithEmail != null)
                {
                    throw new ArgumentException("Email is already in use, Please specify other email id");
                }

                _logger.LogDebug("CreateUserAsync: Validation Completed");

                _logger.LogDebug("CreateUserAsync: Generate Temporary Password");
                var password = GeneratePassword();
                _logger.LogDebug("CreateUserAsync: Generate Temporary Password Completed");

                _logger.LogDebug("CreateUserAsync: Generate Unique User Name Started");
                //Generate Unique User Name
                var userName = await GetUniqueUserName(model.Name);
                _logger.LogDebug($"CreateUserAsync: Generate Unique User Name Completed with user name {userName}");

                _logger.LogDebug("CreateUserAsync: Create User");
                var user = new ApplicationUser
                {
                    Name = model.Name,
                    UserName = userName,
                    Email = model.Email,
                    EmailConfirmed = false,
                    PhoneCode = model.PhoneCode,
                    PhoneNumber = model.PhoneNumber,
                    PhoneNumberConfirmed = true, //Assume confirmed
                    IsActive = true,
                    ApplicationRoleType = model.ApplicationRoleType,
                    AccountActivated = false
                };

                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    _logger.LogError("CreateUserAsync: User Creation failed");
                    _logger.LogError(string.Join(",", result.Errors.Select(x => x.Description)));

                    throw new Exception($"User Creation failed, {result.Errors.Select(x => x.Description)}");
                }
                _logger.LogDebug("CreateUserAsync: Create User Succeeded");

                if (model.Roles.Any())
                {
                    _logger.LogDebug("CreateUserAsync: Create Roles Started");
                    var roleResult = await _userManager.AddToRolesAsync(user, model.Roles);
                    if (!roleResult.Succeeded)
                    {
                        _logger.LogError($"CreateUserAsync: Creating roles for user {user.Id} failed.");
                        _logger.LogError(string.Join(",", result.Errors.Select(x => x.Description)));

                        await _userManager.DeleteAsync(user);
                        throw new Exception($"Creating roles for user {user.Id} failed. Error {result.Errors.Select(x => x.Description)}");
                    }
                    _logger.LogDebug("CreateUserAsync: Create Roles Succeeded");
                }

                _logger.LogDebug("CreateUserAsync: Create Associations Start");

                await _unitOfWork.CommitAsync();

                _logger.LogDebug("CreateUserAsync: Creating Associations Succeeded");

                _logger.LogDebug("CreateUserAsync: Create Activation Claim and Url");
                var secret = Guid.NewGuid().ToString("N");
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var claim = new Claim(CustomClaimTypesConstants.WelcomeEmailConfirmationToken, string.Join(_tokenDeliminator, secret, password, token));
                await _userManager.AddClaimAsync(user, claim);
                _logger.LogDebug("CreateUserAsync: Create Activation Claim");

                _logger.LogDebug("CreateUserAsync: Send Welcome email and SMS");
                await _communicationManager.SendWelcomeEmailNotificationsAsync(user, password, secret);
                _logger.LogDebug("CreateUserAsync: Send Welcome email and SMS Complete");
                return _mapper.Map<UserDetailModel>(user);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error occurred in Add User");
                throw;
            }
        }

        /// <summary>
        /// The method generates user names by concatenating counter with short name of user
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private async Task<string> GetUniqueUserName(string name)
        {
            try
            {
                _logger.LogDebug("GetUnique User Name Started");

                long counter = 0;
                var generatedUserName = GenerateUserName(name.Trim());
                var originalGeneratedUserName = generatedUserName.Clone();

                var userName = generatedUserName;
                var userNames = await _userManager.Users.Where(x => x.UserName.StartsWith(userName))
                    .Select(x => x.UserName)
                    .OrderBy(x => x)
                    .ToListAsync();

                while (userNames.Any(x => x.Equals(generatedUserName)))
                {
                    generatedUserName = string.Concat(originalGeneratedUserName, ++counter);
                }
                _logger.LogDebug("GetUnique User Name Completed");
                return generatedUserName;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error occurred in GetUniqueUserName");
                throw;
            }
        }

        private string GenerateUserName(string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentNullException(nameof(name));
                }

                //Break string into words
                var words = name.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                var userName = words.Length == 1 ? name : string.Concat(words.First()[0], words.Last());

                return userName.ToLowerInvariant();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error occurred in GenerateUserName");
                throw;
            }
        }

        private string GeneratePassword()
        {
            return _passwordGenerator.WithLength(_identityOptions.Value.Password.RequiredLength)
                .WithLowerCase(_identityOptions.Value.Password.RequireLowercase)
                .WithUpperCase(_identityOptions.Value.Password.RequireUppercase)
                .WithNumbers(_identityOptions.Value.Password.RequireDigit)
                .WithSpecials(_identityOptions.Value.Password.RequireNonAlphanumeric)
                .Generate();
        }

        public async Task<UserDetailModel> GetActiveUserDetail(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null || user.IsDeleted || !user.IsActive)
                {
                    return null;
                }

                return await GetDetail(user);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error occurred in GetActiveUserDetails");
                throw;
            }
        }

        public async Task<UserDetailModel> GetUserDetail(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null || user.IsDeleted)
                {
                    return null;
                }

                return await GetDetail(user);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error occurred in GetUserDetail");
                throw;
            }
        }

        public async Task<UserDetailModel> GetDetail(ApplicationUser user)
        {
            try
            {
                var model = _mapper.Map<UserDetailModel>(user);

                //Get User Role Name
                model.Roles = await _userManager.GetRolesAsync(user);
                return model;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error occurred in GetDetails");
                throw;
            }
        }

        public async Task<(bool Succeeded, IEnumerable<string> ErrorMessages)> UpdateUserProfileAsync(UserEditProfileModel model)
        {
            try
            {
                if (model == null)
                {
                    throw new ArgumentNullException(nameof(model));
                }

                var existingUserWithEmail = await FindByEmailExcludingId(model.Email, model.Id);
                if (existingUserWithEmail != null)
                {
                    throw new ArgumentException("Email is already in use, Please specify other email id");
                }

                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null || user.IsDeleted)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                var emailChanged = !model.Email.Equals(user.Email, StringComparison.InvariantCultureIgnoreCase);
                user.Name = model.Name;
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return (false, result.Errors.Select(x => x.Description));
                }

                if (emailChanged || !user.EmailConfirmed)
                {
                    if (user.AccountActivated)
                    {
                        await SendEmailConfirmationNotificationAsync(user);
                    }
                    else
                    {
                        await SendWelcomeEmailNotificationAsync(user);
                    }
                }

                return (true, null);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error occurred during UpdateUserProfile");
                throw;
            }
        }

        public async Task<(bool Succeeded, IEnumerable<string> ErrorMessages)> EditUserAsync(UserEditModel model)
        {
            try
            {
                if (model == null)
                {
                    throw new ArgumentNullException(nameof(model));
                }

                var existingUserWithEmail = await FindByEmailExcludingId(model.Email, model.Id);
                if (existingUserWithEmail != null)
                {
                    throw new ArgumentException("Email is already in use, Please specify other email id");
                }

                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null || user.IsDeleted)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                var emailChanged = !model.Email.Equals(user.Email, StringComparison.InvariantCultureIgnoreCase);

                user.Name = model.Name;
                user.Email = model.Email;
                user.PhoneCode = model.PhoneCode;
                user.PhoneNumber = model.PhoneNumber;
                user.ApplicationRoleType = model.ApplicationRoleType;

                //Email Has Changed and as email is confirmed, Hence mark it unconfirmed
                if (emailChanged && user.EmailConfirmed)
                {
                    user.EmailConfirmed = false;
                }

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return (false, result.Errors.Select(x => x.Description));
                }

                //Update Roles
                var roles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, roles);
                result = await _userManager.AddToRolesAsync(user, model.Roles);
                if (!result.Succeeded)
                {
                    return (false, result.Errors.Select(x => x.Description));
                }

                _logger.LogDebug("EditUserAsync: Update Associations Start");


                await _unitOfWork.CommitAsync();

                _logger.LogDebug("EditUserAsync: Update Associations succeeded");

                if (emailChanged || !user.EmailConfirmed)
                {
                    await SendEmailConfirmationNotificationAsync(user);
                }

                return (true, null);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error occurred during EditUser");
                throw;
            }
        }

        public async Task DeleteAsync(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null || user.IsDeleted)
                {
                    return;
                }

                //Soft Delete
                user.IsDeleted = true;

                await _userManager.UpdateAsync(user);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error occurred during DeleteUser");
                throw;
            }
        }

        public async Task ToggleStatus(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null || user.IsDeleted)
            {
                throw new ArgumentException("User doesn't exists");
            }

            user.IsActive = !user.IsActive;

            await _userManager.UpdateAsync(user);
        }

        public async Task<IList<ApplicationUser>> GetSystemUsers()
        {
            try
            {
                var query = _userManager.Users.Where(user =>
                            !user.IsDeleted
                            && user.IsActive
                            && user.AccountActivated
                            && user.ApplicationRoleType == ApplicationRoleType.System
                     );

                return await query.ToListAsync();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error occurred in GetSystemUsers");
                throw;
            }
        }


        private async Task<bool> UserHavingAnyRole(ApplicationUser applicationUser, IEnumerable<string> roleNames)
        {
            foreach (var role in roleNames)
            {
                if (await _userManager.IsInRoleAsync(applicationUser, role))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
