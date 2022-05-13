using AutoMapper;
using Microsoft.Extensions.Logging;
using Tracker.Business.Managers.Abstractions;
using Tracker.Business.Models.TimeEnties;
using Tracker.DataLayer;
using Tracker.DataLayer.Entities;

namespace Tracker.Business.Managers
{
    public class TimeEntryManager : ITimeEntryManager
    {
        private readonly ILogger<ProjectManager> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public TimeEntryManager(ILogger<ProjectManager> logger,
            IMapper mapper, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
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

        public IQueryable<TimeEntry> GetAllQueryableAsync()
        {
            var timeEntry = _unitOfWork.TimeEntryRepository.GetQueryable(x => !x.IsDeleted);
            return timeEntry;
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

    }
}
