using System;
using System.Linq;
using System.Reflection;
using authService.Attributes.Settings;

namespace authService.Settings
{
    public class Settings : ISettings
    {
        public void Validate(string parentName = "")
        {
            var Name = $"{parentName}{GetType().Name}";
            
            Console.WriteLine($"Validating type : {Name}");
            
            var properties = GetType().GetProperties();
            foreach (var prop in properties)
            {
                ValidateAttributes(Name, prop);
                
                if (prop.PropertyType.IsSubclassOf(typeof(Settings)))
                {
                    var settingsProp = prop.GetValue(this) as Settings;
                    settingsProp?.Validate($"{Name}.");
                }
            }
        }
        
        void ValidateAttributes(string parentName, PropertyInfo prop)
        {
            var attributes = prop.GetCustomAttributes(true);
            foreach (var attr in attributes)
            {
                if (attr.GetType().GetInterfaces().Contains(typeof(IPropertyValidator)))
                {
                    var validator = attr as IPropertyValidator;
                    validator?.Validate(parentName, prop, this);
                }
            }
        }
    }
    
}
