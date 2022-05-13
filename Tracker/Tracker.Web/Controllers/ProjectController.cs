using Microsoft.AspNetCore.Mvc;
using Tracker.Business.Managers.Abstractions;
using Tracker.Business.Models.Project;

namespace Tracker.Web.Controllers
{
    public class ProjectController : Controller
    {
        private readonly IProjectManager _projectManager;
        private readonly ILogger _logger;
        private readonly IEnumerationManager _enumerationManager;

        public ProjectController(IProjectManager projectManager,
            ILogger<ProjectController> logger,
            IEnumerationManager enumerationManager)
        {
            _projectManager = projectManager;
            _logger = logger;
            _enumerationManager = enumerationManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var tt = _enumerationManager.GetContractTypes();
            return View(await _projectManager.GetAllAsync());
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.ProjectType = _enumerationManager.GetProjectType();
            ViewBag.ContactType = _enumerationManager.GetContractTypes();
            ViewBag.LifeCycleModelType = _enumerationManager.GetLifeCycleModelType();

            return View();
        }
        [HttpPost] 
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
            TempData["message"] = "Project Created Successfully!";
            return RedirectToAction(nameof(Index));

        }
        [HttpGet]
        public IActionResult Update()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Delete()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            return View();
        }
    }
}
