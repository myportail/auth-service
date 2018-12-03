using System;
namespace authService.Settings
{
    public class Connections
    {
        public string AuthConnString { get; set; }
        public MongoDb MongoDb { get; set; }
    }
}
