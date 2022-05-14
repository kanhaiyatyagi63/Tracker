using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Dynamic.Core;
using Tracker.Business.Managers.Abstractions;
using Tracker.Business.Models.Extentions;
using Tracker.Business.Models.Project;

namespace Tracker.Web.ApiController
{
    public class ProjectController : BaseApiController
    {
        private readonly IProjectManager _projectManager;
        private readonly ILogger _logger;

        public ProjectController(IProjectManager projectManager,
            ILogger<ProjectController> logger,
            IEnumerationManager enumerationManager,
            IMapper mapper)
        {
            _projectManager = projectManager;
            _logger = logger;
        }
        [Route("GetAllProject")]
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

                var result = _projectManager.GetDataTableRecordAsync(sortColumn, sortColumnDirection,searchValue, recordsTotal, skip, pageSize);
                var data = result.Item1;
                recordsTotal = result.Item2;
                //Returning Json Data  
                return new JsonResult(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });

            }
            catch (Exception ex)
            {
                _logger.LogError($"Get error in project => GetAllAsync {ex}");
                return new JsonResult(new { draw = 0, recordsFiltered = 0, recordsTotal = 0, data = new List<string>() });

            }

        }
    }
}
