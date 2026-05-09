using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TrackYourTasksAPI.Models
{
    public class DailyTask
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // UI-only selection flag for bulk actions — ignored by BSON/DB
        [BsonIgnore]
        public bool IsSelected { get; set; }

        // Daily recurring task time, if any
        [BsonIgnore]
        public TimeSpan? RecurrenceTime { get; set; }

        // Calculates the next occurrence of this task based on the stored time
        [BsonIgnore]
        public DateTime? NextOccurrence => RecurrenceTime.HasValue
            ? DateTime.Today.Add(RecurrenceTime.Value)
            : (DateTime?)null;
    }
}