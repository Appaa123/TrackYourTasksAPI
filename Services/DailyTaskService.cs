using MongoDB.Driver;
using TrackYourTasksAPI.Models;

namespace TrackYourTasksAPI.Services
{
    public class DailyTaskService
    {
        private readonly IMongoCollection<DailyTask> _dailyTasks;

        public DailyTaskService(IConfiguration config)
        {
            var client = new MongoClient(config["MongoDb:ConnectionString"]);
            var database = client.GetDatabase(config["MongoDb:DatabaseName"]);

            _dailyTasks = database.GetCollection<DailyTask>("DailyTasks");
        }

        // Get all daily tasks
        public async Task<List<DailyTask>> GetAsync() =>
            await _dailyTasks.Find(_ => true).ToListAsync();

        // Get single task by id
        public async Task<DailyTask?> GetAsync(string id) =>
            await _dailyTasks.Find(x => x.Id == id).FirstOrDefaultAsync();

        // Create a new daily task. Ensure Id exists (generate GUID if not provided).
        public async Task CreateAsync(DailyTask task)
        {
            if (string.IsNullOrWhiteSpace(task.Id))
            {
                task.Id = Guid.NewGuid().ToString();
            }

            await _dailyTasks.InsertOneAsync(task);
        }

        // Update an existing daily task. Returns true if a document was matched (exists).
        public async Task<bool> UpdateAsync(string id, DailyTask task)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));

            // Ensure task.Id matches the route id so the persisted document keeps the same id.
            task.Id = id;

            var result = await _dailyTasks.ReplaceOneAsync(x => x.Id == id, task);
            // If no document was matched, treat as not found.
            return result.IsAcknowledged && result.MatchedCount > 0;
        }

        // Delete a single daily task by id. Returns true if a document was deleted.
        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _dailyTasks.DeleteOneAsync(x => x.Id == id);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        // Bulk delete daily tasks by a list of ids. Returns the number of documents deleted.
        public async Task<long> BulkDeleteAsync(List<string> ids)
        {
            if (ids == null || ids.Count == 0) return 0;

            var filter = Builders<DailyTask>.Filter.In(x => x.Id, ids);
            var result = await _dailyTasks.DeleteManyAsync(filter);
            return result.IsAcknowledged ? result.DeletedCount : 0;
        }
    }
}