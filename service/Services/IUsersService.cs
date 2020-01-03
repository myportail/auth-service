using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace authService.Services
{
    public interface IUsersService
    {
        Task<Model.Db.User> AddUser(Model.Api.User user);
        Task<List<Model.Api.User>> listUsers();
        Task<Model.Db.User> GetUserByName(string name);
    }
}
