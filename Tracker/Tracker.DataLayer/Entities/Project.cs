using Tracker.Core.Data;
using Tracker.DataLayer.Enumerations;

namespace Tracker.DataLayer.Entities
{
    public class Project : Entity<int>
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? TechnologyStack { get; set; }
        public bool IsClientBillable { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double EstimatedHours { get; set; }
        public ProjectType ProjectType { get; set; }
        public LifeCycleModel LifeCycleModel { get; set; }
        public ContractType ContactType { get; set; }
    }
}
