using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracker.Business.Managers.Abstractions;
using Tracker.Business.Models.TimeEnties;
using Tracker.Core.Utilities;

namespace Tracker.Web.Controllers
{
    [Authorize]
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
            var projects = await _projectManager.GetAllAsync();
            ViewBag.Projects = projects.Select(x => new SelectListItem<int>
            {
                IsSelected = x.Id == projectId,
                Value = x.Id,
                Text = x.Name
            });

            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TimeEntryAddModel model)
        {
            ViewBag.ActivityType = _enumerationManager.GetActivityType();
            var projects = await _projectManager.GetAllAsync();
            ViewBag.Projects = projects.Select(x => new SelectListItem<int>
            {
                Value = x.Id,
                Text = x.Name
            });
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

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            ViewBag.ActivityType = _enumerationManager.GetActivityType();
            var projects = await _projectManager.GetAllAsync();
            ViewBag.Projects = projects.Select(x => new SelectListItem<int>
            {
                Value = x.Id,
                Text = x.Name
            });
            if (id <= 0)
            {
                return NotFound();
            }
            var timeentryViewModel = await _timeEntryManager.GetAsync(id);
            if (timeentryViewModel is null)
            {
                return NotFound();
            }

            return View(_mapper.Map<TimeEntryEditModel>(timeentryViewModel));
        }
        [HttpPost]
        public async Task<IActionResult> Update(TimeEntryEditModel model)
        {
            ViewBag.ActivityType = _enumerationManager.GetActivityType();
            var projects = await _projectManager.GetAllAsync();
            ViewBag.Projects = projects.Select(x => new SelectListItem<int>
            {
                Value = x.Id,
                Text = x.Name
            });

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var isUpdated = await _timeEntryManager.UpdateAsync(model);
            if (!isUpdated)
            {
                ViewBag.error = "Something went wrong!";
                return View(model);
            }
            TempData["success"] = "Time entry updated Successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            ViewBag.ActivityType = _enumerationManager.GetActivityType();
            var projects = await _projectManager.GetAllAsync();
            ViewBag.Projects = projects.Select(x => new SelectListItem<int>
            {
                Value = x.Id,
                Text = x.Name
            });

            var timeEntryViewModel = await _timeEntryManager.GetAsync(id);
            if (timeEntryViewModel is null)
            {
                return NotFound();
            }

            return View(timeEntryViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(TimeEntryViewModel model)
        {
            var isDeleted = await _timeEntryManager.DeleteAsync(model.Id);
            if (!isDeleted)
            {
                ViewBag.error = "Something went wrong!";
                return View(model);
            }
            TempData["success"] = "Time entry deleted Successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
