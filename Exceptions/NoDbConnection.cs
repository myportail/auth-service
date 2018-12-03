using System;

namespace authService.Exceptions
{
    public class NoDbConnection : Exception
    {
        public NoDbConnection(string message = null) : base(message)
        {
            
        }
    }
}
