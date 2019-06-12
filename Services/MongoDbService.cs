using System;
using System.Threading.Tasks;
using authService.Model;
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
        
        public MongoDbService(Settings.Application appSettings,
            IServiceResolver serviceResolver)
        {
            AppSettings = appSettings;
            
            try
            {
                var service = appSettings?.Connections?.Authdb?.Service;
                if (service == null)
                    throw new Exception("Failure to get service information for Authdb");
                
                var name = appSettings.Connections.Authdb.Service.Name;
//                var portName = appSettings.Connections.Authdb.Service.PortName;
                var portNumber = appSettings.Connections.Authdb.Service.PortNumber;
//                var serviceAddress = serviceResolver.Resolve(
//                    name, 
//                    portName);

//                var serviceAddress = new ServiceAddress("authdbproxy", 27017);
//                if (serviceAddress == null)
//                    throw new Exception($"Failure to resolve service address for {name} : {portNumber}");
                
                Console.WriteLine($"AuthDb connection: {name}:{portNumber.ToString()}");

                var credential = MongoCredential.CreateCredential(
                    AppSettings.Connections.Authdb.Database,
                    AppSettings.Connections.Authdb.Username,
                    AppSettings.Connections.Authdb.Password);
            
                var settings = new MongoClientSettings
                {
                    Server = new MongoServerAddress(name, portNumber),
                    Credential = credential
                };

                var mongoClient = new MongoClient(settings);

                Database = mongoClient.GetDatabase(AppSettings.Connections.Authdb.Database);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
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
                Console.Error.WriteLine(e);
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
                Console.Error.WriteLine(e);
            }
        }
    }
}
