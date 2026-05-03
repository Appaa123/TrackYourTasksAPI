using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TrackYourTasksAPI.Models
{
    public class TrackTask
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } // Primary Key
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsSkipped { get; set; }
        public bool IsPartiallyCompleted { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}