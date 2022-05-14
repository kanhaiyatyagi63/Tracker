using System.ComponentModel.DataAnnotations;
using Tracker.Business.Models.Project;

namespace Tracker.Business.Attribute
{
    public class DateValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var pam = validationContext.ObjectInstance as ProjectModel;
            if (pam.StartDate > pam.EndDate)
            {
                return new ValidationResult(GetErrorMessage());
            }
            return ValidationResult.Success;
        }
        public string GetErrorMessage()
        {
            return $"Start Date must be less than equal to End Date!";
        }
    }
}
