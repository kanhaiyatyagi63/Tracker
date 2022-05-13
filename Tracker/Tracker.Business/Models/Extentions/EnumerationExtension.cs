using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Tracker.Business.Models.Extentions
{
    public static class EnumerationExtension
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            return enumValue.GetType()
              .GetMember(enumValue.ToString())
              .First()
              .GetCustomAttribute<DisplayAttribute>()
              ?.GetName();
        }
    }
}
