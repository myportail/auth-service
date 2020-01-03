using authService.Attributes.Settings;

namespace authService.Settings
{
    public class MongoDb : Settings
    {
        public string ConnectionString { get; set; }
        public string Server { get; set; }
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        
        [Required]
        public Service Service { get; set; }
    }
}
