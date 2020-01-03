using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace authService.Model.MongoDb
{
    public class User
    {
        [BsonId]
        public ObjectId InternalId { get; set; }
        
        public string Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}