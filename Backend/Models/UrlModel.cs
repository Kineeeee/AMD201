using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace URLShortener.Models
{
    public class UrlModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("original_url")]
        public string OriginalUrl { get; set; } = string.Empty;

        [BsonElement("short_url")]
        public string ShortUrl { get; set; } = string.Empty;

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
