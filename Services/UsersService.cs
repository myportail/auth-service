using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using authService.Contexts;
using authService.Model.Api;
using authService.Settings;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Swashbuckle.AspNetCore.Swagger;

namespace authService.Services
{
    public class UsersService : IUsersService
    {
        private IPasswordHasher PasswordHasher { get; }
        
        private IMongoDbService MongoDbService { get; }
        
        private UserContext UserContext { get; }

        private IMongoCollection<Model.MongoDb.User> UsersCollection => MongoDbService.Database.GetCollection<Model.MongoDb.User>("authUsers");

        public UsersService(
            IMongoDbService mongoDbService,
            IPasswordHasher passwordHasher,
            UserContext userContext)
        {
            UserContext = userContext;
            PasswordHasher = passwordHasher;
            MongoDbService = mongoDbService;
        }

        public async Task<Model.Db.User> AddUser(Model.Api.User user)
        {
            try
            {
                var dbUser = new Model.Db.User()
                {
                    Name = user.Name,
                    Password = PasswordHasher.HashPassword(user.Password),
                    Id = Guid.NewGuid().ToString()
                };

                await UserContext.Users.AddAsync(dbUser);
                await UserContext.SaveChangesAsync();
//                await UsersCollection.InsertOneAsync(mongoDbUser);
                                
                return dbUser;
            }
            catch (Exception ex)
            {
                System.Console.Error.WriteLine(ex.ToString());
                throw;
            }
        }

        public async Task<Model.Db.User> GetUserByName(string name)
        {
            try
            {
                var res = UserContext.Users.Where(x => x.Name.Equals(name));
                if (res.Any())
                {
                    return res.First();
                }

                return null;
//                using (var result = await UsersCollection.FindAsync(x => x.Name == name))
//                {
//                    var users = result.ToList();
//
//                    if (users.Count > 0)
//                        return users.First();
//
//                    return null;
//                };
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
                using (var results = await UsersCollection.FindAsync(Builders<Model.MongoDb.User>.Filter.Empty))
                {
                    var dbUsers = results.ToList();
                
                    var users = new List<User>();
                
                    dbUsers.ForEach(dbUser =>
                    {
                        users.Add(new User()
                        {
                            Id = dbUser.Id,
                            Name = dbUser.Name
                        });
                    });
                
                    return users;
                }
            }
            catch (Exception ex)
            {
                System.Console.Error.WriteLine(ex.ToString());
                throw;
            }
        }
    }
}
