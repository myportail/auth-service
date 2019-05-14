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
        
        public IMongoDatabase Database { get; }
        
        public IMongoCollection<Model.MongoDb.User> UsersCollection => Database.GetCollection<Model.MongoDb.User>("authUsers");
        
        public MongoDbService(Settings.Application appSettings)
        {
            AppSettings = appSettings;

            try
            {
                var credential = MongoCredential.CreateCredential(
                    AppSettings.Connections.Authdb.Database,
                    AppSettings.Connections.Authdb.Username,
                    AppSettings.Connections.Authdb.Password);
            
                var settings = new MongoClientSettings
                {
                    Server = new MongoServerAddress(AppSettings.Connections.Authdb.Server, 
                        AppSettings.Connections.Authdb.Port),
                    Credential = credential
                };

                var mongoClient = new MongoClient(settings);

                Database = mongoClient.GetDatabase(AppSettings.Connections.Authdb.Database);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task Init()
        {
            try
            {
                var collections = Database.ListCollections().ToList();
                if (collections.Count == 0)
                {
                    var usersCollection = await CreateAuthUsersCollection();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        async Task<IMongoCollection<User>> CreateAuthUsersCollection()
        {
            Database.CreateCollection("authUsers");
            var collection = Database.GetCollection<User>("authUsers");
            var options = new CreateIndexOptions() { Unique = true };
            var field = new StringFieldDefinition<User>("Name");
            var indexDefinition = new IndexKeysDefinitionBuilder<User>().Ascending(field);
            await collection.Indexes.CreateOneAsync(indexDefinition, options);

            return collection;
        }

        async Task CreateDefautUser(IMongoCollection<User> usersCollection)
        {
            try
            {
                await usersCollection.InsertOneAsync(new User()
                {
                    Name = "Admin",
                    Password = "Admin@123"
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
