using Microsoft.Extensions.Caching.Memory;
using MongoDB.Driver;
using TrackYourTasksAPI.Models;


namespace TrackYourTasksAPI.Services
{
    public class MongoTaskService
    {
        private readonly IMongoCollection<TrackTask> _tasks;
        private readonly IMemoryCache _cache;
        private static readonly string AllTasksCacheKey = "tasks:all";

        public MongoTaskService(IConfiguration config, IMemoryCache cache)
        {
            var client = new MongoClient(config["MongoDb:ConnectionString"]);
            var database = client.GetDatabase(config["MongoDb:DatabaseName"]);

            _tasks = database.GetCollection<TrackTask>("TYTTasks");
            _cache = cache;
        }

        // Get all tasks - read from cache if available
        public async Task<List<TrackTask>> GetAsync()
        {
            return await _cache.GetOrCreateAsync(AllTasksCacheKey, async entry =>
            {
                // Tune these values as needed
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                entry.SlidingExpiration = TimeSpan.FromMinutes(2);

                var tasks = await _tasks.Find(_ => true).ToListAsync();
                return tasks;
            });
        }

        // Create task - insert and evict cached list so subsequent GETs are fresh
        public async Task CreateAsync(TrackTask task)
        {
            await _tasks.InsertOneAsync(task);
            _cache.Remove(AllTasksCacheKey);
        }

        // Update task - update DB and evict cache
        public async Task UpdateAsync(string id, TrackTask task)
        {
            await _tasks.ReplaceOneAsync(x => x.Id == id, task);
            _cache.Remove(AllTasksCacheKey);
        }

        // Delete task - delete from DB and evict cache
        public async Task DeleteAsync(string id)
        {
            await _tasks.DeleteOneAsync(x => x.Id == id);
            _cache.Remove(AllTasksCacheKey);
        }
    }
}