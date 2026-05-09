using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TrackYourTasksAPI.Models
{
    public class DailyTask
    {
        // Id is optional for incoming requests; the service will generate a GUID if missing.
        [BsonId]
        public string? Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Persist selection flag so API returns the client-sent value
        public bool IsSelected { get; set; }

        // Daily recurring task time stored as a DateTime (ISO format in JSON)
        public DateTime? RecurrenceTime { get; set; }

        // UI-only computed property (not stored)
        [BsonIgnore]
        public DateTime? NextOccurrence => RecurrenceTime;
    }
}