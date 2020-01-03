using authService.Attributes.Settings;

namespace authService.Settings
{
    public class Service
    {
        [Required]
        public string Name { get; set; }
        
        [Required]
        public int PortNumber { get; set; }
    }
}
