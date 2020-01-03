namespace authService.Settings
{
    public class TokenGeneration : Settings
    {
        public string SecurityKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}