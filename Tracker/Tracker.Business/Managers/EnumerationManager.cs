using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Tracker.Business.Managers.Abstractions;
using Tracker.Core.Utilities;
using Tracker.DataLayer.Enumerations;

namespace Tracker.Business.Managers
{
    public class EnumerationManager : IEnumerationManager
    {
        public List<SelectListItem<int>> GetContractTypes()
        {
            return GetEnumerationAsList(typeof(ContractType));
        }

        public List<SelectListItem<int>> GetLifeCycleModelType()
        {
            return GetEnumerationAsList(typeof(LifeCycleModel));
        }

        public List<SelectListItem<int>> GetProjectType()
        {
            return GetEnumerationAsList(typeof(ProjectType));
        }

        private List<SelectListItem<int>> GetEnumerationAsList(Type enumType)
        {
            var list = new List<SelectListItem<int>>();
            foreach (var name in Enum.GetNames(enumType))
            {
                var member = enumType.GetMember(name);
                //This allows you to obsolete a permission and it won't be shown as a
                //possible option, but is still there so you won't reuse the number
                var ignoreAttribute = member[0].GetCustomAttribute<IgnoreAttribute>();
                if (ignoreAttribute != null)
                    continue;
                //If there is no DisplayAttribute then the Enum is not used
                var displayAttribute = member[0].GetCustomAttribute<DisplayAttribute>();
                if (displayAttribute == null)
                    continue;
                int value = (int)Enum.Parse(enumType, name);

                list.Add(new SelectListItem<int>
                {
                    Text = displayAttribute.Name,
                    Value = value
                });
            }
            return list.OrderBy(x => x.Text).ToList();
        }
    }
}
