using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DevAtlasBackend.Models
{
    public class CourseOutline
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        public string CourseId { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string? FileUrl { get; set; }
        public string? VideoUrl { get; set; }
        public int Order { get; set; }
    }
}