using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Tracker.Business.Managers.Abstractions;
using Tracker.Business.Models.TimeEnties;

namespace Tracker.Web.Controllers
{
    public class TimeEntryController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ITimeEntryManager _timeEntryManager;
        private readonly IProjectManager _projectManager;
        private readonly IEnumerationManager _enumerationManager;

        public TimeEntryController(IMapper mapper,
                                   ITimeEntryManager timeEntryManager,
                                   IProjectManager projectManager,
                                   IEnumerationManager enumerationManager)
        {
            _mapper = mapper;
            _timeEntryManager = timeEntryManager;
            _projectManager = projectManager;
            _enumerationManager = enumerationManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Create(int projectId)
        {
            ViewBag.ActivityType = _enumerationManager.GetActivityType();
            ViewBag.ProjectId = projectId;
            if (projectId <= 0)
            {
                return NotFound();
            }
            var project = await _projectManager.GetAsync(projectId);
            if (project is null)
            {
                return NotFound();
            }
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TimeEntryAddModel model)
        {
            ViewBag.ActivityType = _enumerationManager.GetActivityType();
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var isCreated = await _timeEntryManager.CreateAsync(model);
            if (!isCreated)
            {
                ViewBag.message = "Something went wrong!";
                return View(model);
            }
            TempData["success"] = "Time entries created successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
