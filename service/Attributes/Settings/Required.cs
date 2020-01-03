using System;
using System.Reflection;

namespace authService.Attributes.Settings
{
    public class Required : Attribute, IPropertyValidator
    {
        public void Validate(string parentName, PropertyInfo propInfo, object obj)
        {
            if (propInfo.GetValue(obj) == null)
            {
                Console.Error.WriteLine($"ERROR : Required settings missing --> {parentName}.{propInfo.Name}");
            }
        }
    }
}
