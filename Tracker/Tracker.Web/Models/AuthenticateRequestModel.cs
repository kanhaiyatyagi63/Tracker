using System.ComponentModel.DataAnnotations;

namespace Tracker.Web.Models
{
    public class AuthenticateRequestModel
    {
        [Required, Display(Name = "Username", Prompt = "Enter username")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password), Display(Name = "Password", Prompt = "Enter password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
