using System;
using System.Threading.Tasks;
using authService.Model.MongoDb;
using MongoDB.Bson;
using MongoDB.Driver;

namespace authService.Services
{
    public class MongoDbService : IMongoDbService
    {
        private Settings.Application AppSettings { get; }
        
        public MongoDbService(Settings.Application appSettings)
        {
            AppSettings = appSettings;
        }

        public async Task Init()
        {
            try
            {
                var credential = MongoCredential.CreateCredential(
                    AppSettings.Connections.MongoDb.Database,
                    AppSettings.Connections.MongoDb.Username,
                    AppSettings.Connections.MongoDb.Password);
            
                var settings = new MongoClientSettings
                {
                    Server = new MongoServerAddress(AppSettings.Connections.MongoDb.Server, 27017),
                    Credential = credential
                };

                var mongoClient = new MongoClient(settings);

                var db = mongoClient.GetDatabase(AppSettings.Connections.MongoDb.Database);
                var collection = db.GetCollection<User>("authUsers");

                // var doc = new BsonDocument();
                // doc.Add("Name", "Admin");
                // doc.Add("Password", "Admin@123");
                
                await collection.InsertOneAsync(new User()
                {
                    Name = "Admin",
                    Password = "Admin@123"
                });

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
