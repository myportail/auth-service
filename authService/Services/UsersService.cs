using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using authService.Model.Api;

namespace authService.Services
{
    public class UsersService : IUsersService
    {
        private Contexts.UsersDbContext UsersContext { get; }
        private IPasswordHasher PasswordHasher { get; }
        
        public UsersService(
            Contexts.UsersDbContext usersContext,
            IPasswordHasher passwordHasher)
        {
            UsersContext = usersContext;
            PasswordHasher = passwordHasher;
        }

        public async Task<Model.Db.User> AddUser(Model.Api.User user)
        {
            try
            {
                var dbUser = new Model.Db.User
                {
                    Name = user.Name,
                    Password = PasswordHasher.HashPassword(user.Password),
                    Id = Guid.NewGuid().ToString()
                };

                using (var ctx = UsersContext)
                {
                    ctx.Database.EnsureCreated();
                    ctx.Users.Add(dbUser);
                    await ctx.SaveChangesAsync();
                }
                
                return dbUser;
            }
            catch (Exception ex)
            {
                System.Console.Error.WriteLine(ex.ToString());
                throw;
            }
        }

        public async Task<List<User>> listUsers()
        {
            try
            {
                List<Model.Api.User> users = new List<User>();
                
                using (var ctx = UsersContext)
                {
                    foreach (var dbUser in ctx.Users)
                    {
                        users.Add(new User()
                        {
                            Id = dbUser.Id,
                            Name = dbUser.Name
                        });
                    }
                }

                return users;
            }
            catch (Exception ex)
            {
                System.Console.Error.WriteLine(ex.ToString());
                throw;
            }
        }
    }
}
