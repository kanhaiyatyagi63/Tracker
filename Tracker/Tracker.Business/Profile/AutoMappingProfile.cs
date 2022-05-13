using Tracker.Business.Models.Project;
using Tracker.Business.Models.Role;
using Tracker.Business.Models.User;
using Tracker.DataLayer.Entities;

namespace Tracker.Business.Profile
{
    public class AutoMappingProfile : AutoMapper.Profile
    {
        public AutoMappingProfile()
        {
            CreateMap<ApplicationUser, UserModel>();
            CreateMap<ApplicationRole, RoleModel>();

            CreateMap<Project, ProjectAddModel>().ReverseMap();
            CreateMap<Project, ProjectEditModel>().ReverseMap();
            CreateMap<Project, ProjectViewModel>().ReverseMap();

        }
    }
}
