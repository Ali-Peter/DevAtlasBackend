using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DevAtlasBackend.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        [BsonElement("firstname")]
        public string FirstName { get; set; } = null!;

        [BsonElement("lastname")]
        public string LastName { get; set; } = null!;

        [BsonElement("email")]
        public string Email { get; set; } = null!;

        [BsonElement("passwordHash")]
        public string PasswordHash { get; set; } = null!;
    }
}