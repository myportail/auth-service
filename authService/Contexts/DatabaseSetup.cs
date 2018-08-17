using System;
using authService.Services;
using Microsoft.Extensions.DependencyInjection;

namespace authService.Contexts
{
    public class DatabaseSetup
    {
        public static void InitialiseUsers(IServiceProvider services)
        {
            var usersDbContext = services.GetService<UsersDbContext>();
            var usersService = services.GetService<IUsersService>();

            if (usersDbContext.Database.EnsureCreated())
            {
                var adminUser = new Model.Api.User()
                {
                    Name = "Admin",
                    Password = "Admin@123"
                };

                usersService.AddUser(adminUser);

                // first time the database is created : add default admin user   
            }
        }
    }
}
