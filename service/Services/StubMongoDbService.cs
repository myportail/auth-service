using System.Threading.Tasks;
using authService.Model.MongoDb;
using MongoDB.Driver;

namespace authService.Services
{
    public class StubMongoDbService : IMongoDbService
    {
        public IMongoDatabase Database { get; }
        public IMongoCollection<User> UsersCollection { get; }
        public async Task Init()
        {
        }
    }
}