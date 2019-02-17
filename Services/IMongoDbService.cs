using System.Threading.Tasks;
using MongoDB.Driver;

namespace authService.Services
{
    public interface IMongoDbService
    {
        IMongoDatabase Database { get; }
        
        Task Init();
    }
}
