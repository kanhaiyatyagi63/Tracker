using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracker.Business.Managers.Abstractions;
using Tracker.Business.Models.Project;
using Tracker.Core.Services.Abstractions;

namespace Tracker.Web.Controllers
{
    [Authorize]
    public class ProjectController : Controller
    {
        private readonly IProjectManager _projectManager;
        private readonly ILogger _logger;
        private readonly IEnumerationManager _enumerationManager;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;

        public ProjectController(IProjectManager projectManager,
            ILogger<ProjectController> logger,
            IEnumerationManager enumerationManager,
            IMapper mapper,
            IUserContextService userContextService)
        {
            _projectManager = projectManager;
            _logger = logger;
            _enumerationManager = enumerationManager;
            _mapper = mapper;
            _userContextService = userContextService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            var user = User.IsInRole("User");
            var admin = User.IsInRole("Admin");

            return View();
        }

        [HttpGet, Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.ProjectType = _enumerationManager.GetProjectType();
            ViewBag.ContactType = _enumerationManager.GetContractTypes();
            ViewBag.LifeCycleModelType = _enumerationManager.GetLifeCycleModelType();

            return View();
        }
        [HttpPost, Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProjectAddModel model)
        {
            ViewBag.ProjectType = _enumerationManager.GetProjectType();
            ViewBag.ContactType = _enumerationManager.GetContractTypes();
            ViewBag.LifeCycleModelType = _enumerationManager.GetLifeCycleModelType();

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var isCreated = await _projectManager.CreateAsync(model);
            if (!isCreated)
            {
                ViewBag.message = "Something went wrong!";
                return View(model);
            }
            TempData["success"] = "Project created successfully!";
            return RedirectToAction(nameof(Index));

        }
        [HttpGet, Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id)
        {
            ViewBag.ProjectType = _enumerationManager.GetProjectType();
            ViewBag.ContactType = _enumerationManager.GetContractTypes();
            ViewBag.LifeCycleModelType = _enumerationManager.GetLifeCycleModelType();
            if (id <= 0)
            {
                return NotFound();
            }
            var projectViewModel = await _projectManager.GetAsync(id);
            if (projectViewModel is null)
            {
                return NotFound();
            }

            return View(_mapper.Map<ProjectEditModel>(projectViewModel));
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(ProjectEditModel model)
        {
            ViewBag.ProjectType = _enumerationManager.GetProjectType();
            ViewBag.ContactType = _enumerationManager.GetContractTypes();
            ViewBag.LifeCycleModelType = _enumerationManager.GetLifeCycleModelType();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var isUpdated = await _projectManager.UpdateAsync(model);
            if (!isUpdated)
            {
                ViewBag.error = "Something went wrong!";
                return View(model);
            }
            TempData["success"] = "Project updated Successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet, Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            ViewBag.ProjectType = _enumerationManager.GetProjectType();
            ViewBag.ContactType = _enumerationManager.GetContractTypes();
            ViewBag.LifeCycleModelType = _enumerationManager.GetLifeCycleModelType();
            if (id <= 0)
            {
                return NotFound();
            }
            var projectViewModel = await _projectManager.GetAsync(id);
            if (projectViewModel is null)
            {
                return NotFound();
            }

            return View(projectViewModel);
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(ProjectViewModel model)
        {
            var isDeleted = await _projectManager.DeleteAsync(model.Id);
            if (!isDeleted)
            {
                ViewBag.error = "Something went wrong!";
                return View(model);
            }
            TempData["success"] = "Project deleted Successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            ViewBag.ProjectType = _enumerationManager.GetProjectType();
            ViewBag.ContactType = _enumerationManager.GetContractTypes();
            ViewBag.LifeCycleModelType = _enumerationManager.GetLifeCycleModelType();
            if (id <= 0)
            {
                return NotFound();
            }
            var projectViewModel = await _projectManager.GetAsync(id);
            if (projectViewModel is null)
            {
                return NotFound();
            }

            return View(projectViewModel);
        }
    }
}
