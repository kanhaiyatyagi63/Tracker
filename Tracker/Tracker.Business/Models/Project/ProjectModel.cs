using System.ComponentModel.DataAnnotations;
using Tracker.Core.Data;
using Tracker.Core.Models.Constants;
using Tracker.DataLayer.Enumerations;

namespace Tracker.Business.Models.Project
{
    public class ProjectModel : Entity<int>
    {
        [Required, Display(Name = "Name", Prompt = "Enter project name"),
         MaxLength(StringLengthConstants.Name)]
        public string Name { get; set; }
        [Display(Name = "Description", Prompt = "Enter description"),
         MaxLength(StringLengthConstants.Description)]
        public string? Description { get; set; }
        [Display(Name = "Technology Stack",
         Prompt = "Enter technology stack like angular, dot net etc."),
         MaxLength(StringLengthConstants.Default)]
        public string? TechnologyStack { get; set; }
        [Required, Display(Name = "Client Billable")]
        public bool IsClientBillable { get; set; }
        [Required, Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }
        [Required, Display(Name = "End Date")]
        public DateTime EndDate { get; set; }
        [Required, Display(Name = "Estimated Hours", Prompt = "Enter project estimated hours")]
        public double EstimatedHours { get; set; }
        [Required, Display(Name = "Project Type")]
        public ProjectType ProjectType { get; set; }
        [Required, Display(Name = "LifeCycle Model")]
        public LifeCycleModel LifeCycleModel { get; set; }
        [Required, Display(Name = "Contract Type")]
        public ContractType ContactType { get; set; }
    }
}
