using AutoMapper;
using Microsoft.Extensions.Logging;
using Tracker.Business.Managers.Abstractions;
using Tracker.Business.Models.Project;
using Tracker.DataLayer;
using Tracker.DataLayer.Entities;

namespace Tracker.Business.Managers
{
    public class ProjectManager : IProjectManager
    {
        private readonly ILogger<ProjectManager> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ProjectManager(ILogger<ProjectManager> logger,
            IMapper mapper, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> CreateAsync(ProjectAddModel model)
        {
            try
            {
                var project = _mapper.Map<Project>(model);
                if (project == null)
                    return false;
                var isExist = await _unitOfWork.ProjectRepository.AnyAsync(x => x.Name == project.Name);
                if (isExist)
                {
                    _logger.LogError($"Project with name {project.Name} is already exist!, Unable to add project");
                    return false;
                }
                await _unitOfWork.ProjectRepository.AddAsync(project);
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
                var isExist = await _unitOfWork.ProjectRepository.AnyAsync(x => x.Id == id);
                if (!isExist)
                {
                    _logger.LogError($"Unable to find project with project id: {id}");
                    return false;
                }
                await _unitOfWork.ProjectRepository.DeleteAsync(id);
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in DeleteAsync() : {ex.ToString()}");
                return false;
            }
        }

        public async Task<IEnumerable<ProjectViewModel>> GetAllAsync()
        {
            var projects = await _unitOfWork.ProjectRepository.GetListAsync(x => !x.IsDeleted);
            return _mapper.Map<IEnumerable<ProjectViewModel>>(projects);
        }

        public IQueryable<Project> GetAllQueryableAsync()
        {
            var projects = _unitOfWork.ProjectRepository.GetQueryable(x => !x.IsDeleted);
            return projects;
        }

        public async Task<ProjectViewModel> GetAsync(int id)
        {
            var project = await _unitOfWork.ProjectRepository.GetAsync(x=>x.Id == x.Id && !x.IsDeleted);
            return _mapper.Map<ProjectViewModel>(project);
        }

        public async Task<bool> UpdateAsync(ProjectEditModel model)
        {
            try
            {
                var project = _mapper.Map<Project>(model);
                if (project == null)
                    return false;
                var isExist = await _unitOfWork.ProjectRepository.AnyAsync(x => x.Name == project.Name && !x.IsDeleted && x.Id != project.Id);
                if (isExist)
                {
                    _logger.LogError($"Project with name {project.Name} is already exist, Unable to update project!");
                    return false;
                }
                _unitOfWork.ProjectRepository.Update(project);
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
