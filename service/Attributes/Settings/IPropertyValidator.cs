
using System.Reflection;

namespace authService.Attributes.Settings
{
    public interface IPropertyValidator
    {
        void Validate(string parentName, PropertyInfo propInfo, object obj);
    }
}
