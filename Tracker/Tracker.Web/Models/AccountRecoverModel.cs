using System.ComponentModel.DataAnnotations;

namespace Tracker.Web.Models
{
    public class AccountRecoverModel
    {
        [Required]
        public string Email { get; set; }
    }
}
