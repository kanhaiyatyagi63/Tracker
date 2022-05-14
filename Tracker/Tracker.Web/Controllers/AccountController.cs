using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Tracker.Business.Managers.Abstractions;
using Tracker.DataLayer.Entities;
using Tracker.DataLayer.Enumerations;
using Tracker.Web.Models;

namespace Tracker.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IApplicationUserManager _userManager;
        private readonly IApplicationUserManager _applicationUserManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IMapper _mapper;

        public AccountController(ILogger<AccountController> logger, IMapper mapper,
            IApplicationUserManager userManager, IApplicationUserManager applicationUserManager)
        {
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
            _applicationUserManager = applicationUserManager;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AuthenticateRequestModel userModel, string? returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(userModel);
            }
            var user = await _userManager.FindByUserNameOrEmail(userModel.UserName);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid UserName or Password");
                return View();
            }

            var result = await _userManager.SignInAsync(user, userModel.Password, userModel.RememberMe);
            if (result.Succeeded)
            {
                return RedirectToLocal(returnUrl);
            }
            else
            {
                ModelState.AddModelError("", "Invalid UserName or Password");
                return View();
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterRequestModel userModel)
        {
            if (!ModelState.IsValid)
            {
                return View(userModel);
            }

            var user = _mapper.Map<ApplicationUser>(userModel);
            user.UserName = userModel.Name;
            user.NormalizedUserName = userModel.Name;
            user.PhoneCode = "+91";
            user.ApplicationRoleType = ApplicationRoleType.User;
            var result = await _userManager.CreateAsync(user, userModel.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }

                return View(userModel);
            }

            var isInRole = await _applicationUserManager.IsInRoleAsync(user, "User");
            if (!isInRole)
            {
                var resultRole = await _applicationUserManager.AddToRoleAsync(user, "User");
                if (!resultRole.Succeeded)
                {
                    _logger.LogError("user registration failed");
                    _logger.LogError(string.Join(",", result.Errors.Select(x => x.Description)));

                    await _applicationUserManager.DeleteAsync(user);
                    return View();
                }
            }
            _logger.LogInformation("user registration completed");
            TempData["success"] = "User registered successfully!";
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _userManager.LogoutAsync();

            return RedirectToAction(nameof(HomeController.Index), "Project");
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction(nameof(HomeController.Index), "Project");
        }
    }
}
