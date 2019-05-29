using authService.Model;

namespace authService.Services
{
    public interface IServiceResolver
    {
        ServiceAddress Resolve(string serviceName, string portName);
    }
}

