using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tracker.Business.Managers.Abstractions;
using Tracker.Business.Models.TimeEnties;
using Tracker.DataLayer;
using Tracker.DataLayer.Entities;
using System.Linq.Dynamic.Core;
using Tracker.Core.Services.Abstractions;
using Tracker.Business.Models.Extentions;

namespace Tracker.Business.Managers
{
    public class TimeEntryManager : ITimeEntryManager
    {
        private readonly ILogger<ProjectManager> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContextService;
        private readonly IApplicationUserManager _applicationUserManager;

        public TimeEntryManager(ILogger<ProjectManager> logger,
            IMapper mapper, IUnitOfWork unitOfWork, IUserContextService userContextService,
            IApplicationUserManager applicationUserManager)
        {
            _logger = logger;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userContextService = userContextService;
            _applicationUserManager = applicationUserManager;
        }

        public async Task<bool> CreateAsync(TimeEntryAddModel model)
        {
            try
            {
                var timeEntry = _mapper.Map<TimeEntry>(model);
                if (timeEntry == null)
                    return false;
                await _unitOfWork.TimeEntryRepository.AddAsync(timeEntry);
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in CreateAsync() : {ex.ToString()}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var isExist = await _unitOfWork.TimeEntryRepository.AnyAsync(x => x.Id == id);
                if (!isExist)
                {
                    _logger.LogError($"Unable to find timeEntry with timeEntry id: {id}");
                    return false;
                }
                await _unitOfWork.TimeEntryRepository.DeleteAsync(id);
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in DeleteAsync() : {ex.ToString()}");
                return false;
            }
        }

        public async Task<IEnumerable<TimeEntryViewModel>> GetAllAsync()
        {
            var timeEntry = await _unitOfWork.TimeEntryRepository.GetListAsync(x => !x.IsDeleted);
            return _mapper.Map<IEnumerable<TimeEntryViewModel>>(timeEntry);
        }

        public async Task<TimeEntryViewModel> GetAsync(int id)
        {
            var timeEntry = await _unitOfWork.TimeEntryRepository.GetAsync(x => x.Id == x.Id && !x.IsDeleted);
            return _mapper.Map<TimeEntryViewModel>(timeEntry);
        }

        public async Task<bool> UpdateAsync(TimeEntryEditModel model)
        {
            try
            {
                var timeEntry = _mapper.Map<TimeEntry>(model);
                if (timeEntry == null)
                    return false;
                _unitOfWork.TimeEntryRepository.Update(timeEntry);
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in UpdateAsync() : {ex.ToString()}");
                return false;
            }
        }

        public (IEnumerable<TimeEntryViewModel>, int) GetDataTableRecordAsync(string sortColumn, string sortColumnDirection,
          string searchValue, int recordsTotal, int skip, int pageSize, string projectId)
        {
            // getting all time entry data  

            var customerData = _unitOfWork.TimeEntryRepository
                                          .GetQueryable(x => !x.IsDeleted)
                                          .Include(x => x.Project)
                                          .AsQueryable();


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
            if (!string.IsNullOrEmpty(projectId))
            {

                customerData = customerData.Where(x => x.ProjectId == Convert.ToInt32(projectId));
            }

            // user is admin then return all timeenteries otherwise
            // return specific user time enteries
            if (!_userContextService.IsAdmin())
            {
                customerData = customerData.Where(x => x.CreatedBy == _userContextService.GetUserId());
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
                CreatedByName = _applicationUserManager.GetUserDetail(x.CreatedBy).Result?.Name,
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
            return (data, recordsTotal);
        }
    }
}
