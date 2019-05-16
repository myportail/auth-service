using System;
namespace authService.Settings
{
    public class Application : Settings
    {
        public Connections Connections { get; set; }
        public TokenGeneration TokenGeneration { get; set; }
    }
}
