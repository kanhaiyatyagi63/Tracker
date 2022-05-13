using Tracker.DataLayer.Enumerations;

namespace Tracker.Business.Models.User
{
    public class UserDetailModel
    {
        public string Id { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string? PhoneCode { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public bool AccountActivated { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public ApplicationRoleType ApplicationRoleType { get; set; }
        public bool EmailConfirmed { get; internal set; }
    }
}
