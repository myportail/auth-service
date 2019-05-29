using System;
using authService.Model;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http2.HPack;

namespace authService.Services
{
    public class ServiceResolver : IServiceResolver
    {
        public ServiceAddress Resolve(string serviceName, string portName)
        {
            var addressEnvName = $"{serviceName.ToUpper()}_SERVICE_HOST";
            var portEnvName = $"{serviceName.ToUpper()}_SERVICE_PORT_{portName.ToUpper()}";
            
            var vars = Environment.GetEnvironmentVariables();
            var addressEntry = vars[addressEnvName] as string;
            var portEntry = vars[portEnvName] as string;

            if (!string.IsNullOrEmpty(addressEntry) &&
                !string.IsNullOrEmpty(portEntry) &&
                int.TryParse(portEntry, out var portValue))
            {
                return new ServiceAddress(addressEntry, portValue);
            }

            return null;
        }
    }
}
