using System;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;
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
        
        public MongoDbService(Settings.Application appSettings)
        {
            AppSettings = appSettings;
            
            try
            {
                var service = appSettings?.Connections?.Authdb?.Service;
                if (service == null)
                    throw new Exception("Failure to get service information for Authdb");
                
                var name = appSettings.Connections.Authdb.Service.Name;
                var portNumber = appSettings.Connections.Authdb.Service.PortNumber;
                
                Console.WriteLine($"AuthDb connection: {name}:{portNumber.ToString()}");

                MongoInternalIdentity identity = new MongoInternalIdentity(
                    AppSettings.Connections.Authdb.Database,
                    AppSettings.Connections.Authdb.Username);

                var encodedPwd = WebUtility.UrlEncode(AppSettings.Connections.Authdb.Password);
//                var encodedPwdBytes = HttpUtility.UrlEncodeToBytes(AppSettings.Connections.Authdb.Password, Encoding.ASCII);
//                var encodedPwd = System.Text.Encoding.UTF8.GetString(encodedPwdBytes);
//                var encodedPwd = Uri.EscapeUriString(AppSettings.Connections.Authdb.Password);
                PasswordEvidence pwdEvidence = new PasswordEvidence(AppSettings.Connections.Authdb.Password);

                MongoCredential credential = new MongoCredential(
                    "SCRAM-SHA-1",
                    identity,
                    pwdEvidence);
                
//                var credential = MongoCredential.CreateCredential(
//                    AppSettings.Connections.Authdb.Database,
//                    AppSettings.Connections.Authdb.Username,
//                    AppSettings.Connections.Authdb.Password);
            
                var settings = new MongoClientSettings
                {
                    Server = new MongoServerAddress(name, portNumber),
                    Credential = credential
                };

//                encodedPwd = "~%5Dub%3A5jc%3C8!%3A-(z";
                var connectionString = $"mongodb://{AppSettings.Connections.Authdb.Username}:{encodedPwd}@138.197.130.157:27017/?authSource=auth";

//                var mongoClient = new MongoClient(settings);
                var mongoClient = new MongoClient(connectionString);

//                Database = mongoClient.GetDatabase(AppSettings.Connections.Authdb.Database);
                Database = mongoClient.GetDatabase("auth");
                Database.CreateCollection("Test");
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
