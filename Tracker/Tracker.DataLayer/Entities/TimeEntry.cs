using Tracker.Core.Data;
using Tracker.DataLayer.Enumerations;

namespace Tracker.DataLayer.Entities
{
    public class TimeEntry : Entity<int>
    {
        public int ProjectId { get; set; }
        public string Comments { get; set; }
        public double Hours { get; set; }
        public ActivityType ActivityType { get; set; }
        public DateTime LogTime { get; set; }
        public bool IsApproved { get; set; }
        public virtual Project Project { get; set; }

    }
}
