using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tracker.Business.Managers.Abstractions;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Tracker.Business.Models.Project;
using Tracker.Business.Models.Extentions;

namespace Tracker.Web.ApiController
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectManager _projectManager;
        private readonly ILogger _logger;
        private readonly IEnumerationManager _enumerationManager;
        private readonly IMapper _mapper;

        public ProjectController(IProjectManager projectManager,
            ILogger<ProjectController> logger,
            IEnumerationManager enumerationManager,
            IMapper mapper)
        {
            _projectManager = projectManager;
            _logger = logger;
            _enumerationManager = enumerationManager;
            _mapper = mapper;
        }
        [Route("GetAllProject")]
        public async  Task<IActionResult> GetAllAsync()
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
                var customerData =  _projectManager.GetAllQueryableAsync();
                //Sorting  
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                //Search  
                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(m => m.Name == searchValue);
                }

                //total number of rows counts   
                recordsTotal = customerData.Count();
                //Paging   
                var projects = customerData.Skip(skip).Take(pageSize).ToList();
                var data = projects.Select(x => new ProjectViewModel()
                {
                    Id = x.Id,
                    IsClientBillable = x.IsClientBillable,
                    ContactType = x.ContactType,
                    ContractTypeView = x.ContactType.GetDisplayName(),
                    ProjectType = x.ProjectType,
                    ProjectTypeView = x.ProjectType.GetDisplayName(),
                    CreatedBy = x.CreatedBy,
                    CreatedDate = x.CreatedDate,
                    UpdatedBy = x.UpdatedBy,
                    UpdatedDate = x.UpdatedDate,
                    Description = x.Description,
                    EndDate = x.EndDate,
                    StartDate = x.StartDate,
                    EstimatedHours = x.EstimatedHours,
                    LifeCycleModel = x.LifeCycleModel,
                    IsDeleted = x.IsDeleted,
                    LifeCycleTypeView = x.LifeCycleModel.GetDisplayName(),
                    Name = x.Name,
                    TechnologyStack = x.TechnologyStack
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
