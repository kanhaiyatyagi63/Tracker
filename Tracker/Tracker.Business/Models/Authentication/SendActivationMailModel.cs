using System.ComponentModel.DataAnnotations;

namespace Tracker.Business.Models.Authentication
{
    public class SendActivationMailModel
    {
        [Required]
        public string ApplicationUserId { get; set; }
    }
}
