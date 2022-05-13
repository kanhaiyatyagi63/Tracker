using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Dynamic.Core;
using Tracker.Business.Managers.Abstractions;
using Tracker.Business.Models.Extentions;
using Tracker.Business.Models.TimeEnties;

namespace Tracker.Web.ApiController
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeEntryController : Controller
    {
        private readonly ITimeEntryManager _timeEntryManager;
        private readonly ILogger _logger;
        private readonly IEnumerationManager _enumerationManager;
        private readonly IMapper _mapper;

        public TimeEntryController(ITimeEntryManager timeEntryManager,
            ILogger<TimeEntryController> logger,
            IEnumerationManager enumerationManager,
            IMapper mapper)
        {
            _timeEntryManager = timeEntryManager;
            _logger = logger;
            _enumerationManager = enumerationManager;
            _mapper = mapper;
        }
        [Route("GetAllTimEntries")]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();

                // Skip number of Rows count  
                var start = Request.Form["start"].FirstOrDefault();

                // Paging Length 10,20  
                var length = Request.Form["length"].FirstOrDefault();

                // Sort Column Name  
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();

                // Sort Column Direction (asc, desc)  
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();

                // Search Value from (Search box)  
                var searchValue = Request.Form["search[value]"].FirstOrDefault();

                //Paging Size (10, 20, 50,100)  
                int pageSize = length != null ? Convert.ToInt32(length) : 0;

                int skip = start != null ? Convert.ToInt32(start) : 0;

                int recordsTotal = 0;

                // getting all Customer data  
                var customerData = _timeEntryManager.GetAllQueryableAsync();

                //Sorting  
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                //Search  
                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(m => m.Comments.Contains(searchValue) ||
                    m.Project.Name.Contains(searchValue) || m.Hours.ToString().Contains(searchValue));
                }

                //total number of rows counts   
                recordsTotal = customerData.Count();
                //Paging   
                var projects = customerData.Skip(skip).Take(pageSize).ToList();
                var data = projects.Select(x => new TimeEntryViewModel()
                {
                    Id = x.Id,
                    IsApproved = x.IsApproved,
                    Comments = x.Comments,
                    ActivityTypeView = x.ActivityType.GetDisplayName(),
                    CreatedBy = x.CreatedBy,
                    CreatedDate = x.CreatedDate,
                    UpdatedBy = x.UpdatedBy,
                    UpdatedDate = x.UpdatedDate,
                    Hours = x.Hours,
                    LogTime = x.LogTime,
                    ProjectName = x.Project?.Name,
                    ProjectId = x.ProjectId,
                });
                //Returning Json Data  
                return new JsonResult(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });

            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}
