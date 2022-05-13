using System.ComponentModel.DataAnnotations;
using Tracker.Core.Data;
using Tracker.Core.Models.Constants;
using Tracker.DataLayer.Enumerations;

namespace Tracker.Business.Models.TimeEnties
{
    public class TimeEntryModel : Entity<int>
    {
        [Required, Display(Name = "Project")]
        public int ProjectId { get; set; }
        [Required, Display(Name = "Comments"), MaxLength(StringLengthConstants.Description)]
        public string Comments { get; set; }
        [Required, Display(Name = "Hours")]
        public double Hours { get; set; }
        [Required, Display(Name = "Activity")]
        public ActivityType ActivityType { get; set; }
        [Required, Display(Name = "Date")]
        public DateTime LogTime { get; set; }
        [Required, Display(Name = "Approved")]
        public bool IsApproved { get; set; }
    }
}
