using System.Threading.Tasks;

namespace authService.Services
{
    public interface IMongoDbService
    {
        Task Init();
    }
}