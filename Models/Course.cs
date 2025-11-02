using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DevAtlasBackend.Models
{
    public class Course
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string Level { get; set; } = null!;
        public string? ThumbnailUrl { get; set; }
    }
}