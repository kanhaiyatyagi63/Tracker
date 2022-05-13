using AutoMapper;
using Tracker.DataLayer.Entities;
using Tracker.Web.Models;

namespace Tracker.Web
{
    public class AutoMappingProfile : Profile
    {
        public AutoMappingProfile()
        {
            CreateMap<RegisterRequestModel, ApplicationUser>()
                .ForMember(u => u.UserName, opt => opt.MapFrom(x => x.Email));
        }
    }
}
