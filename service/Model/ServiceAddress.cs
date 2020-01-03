namespace authService.Model
{
    public class ServiceAddress
    {
        public string Address { get; }
        public int Port { get; }

        public ServiceAddress(string address, int port)
        {
            Address = address;
            Port = port;
        }
    }
}
