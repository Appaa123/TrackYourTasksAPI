namespace TrackYourTasksAPI.Models
{
    // Renamed envelope used by API requests/responses:
    // { "task": { ... DailyTask ... } }
    public class DailyTasks
    {
        public DailyTask Task { get; set; } = new();
    }
}