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

                var collections = db.ListCollections().ToList();
                if (collections.Count == 0)
                {
                    var usersCollection = await CreateAuthUsersCollection(db);
                    await CreateDefautUser(usersCollection);
                }
                
//                var users = db.GetCollection<User>("authUsers");
//
//                var result = collection.Find(x => x.Name == "Admin").ToList();
                
                // var doc = new BsonDocument();
                // doc.Add("Name", "Admin");
                // doc.Add("Password", "Admin@123");
                

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        async Task<IMongoCollection<User>> CreateAuthUsersCollection(IMongoDatabase db)
        {
            db.CreateCollection("authUsers");
            var collection = db.GetCollection<User>("authUsers");
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
