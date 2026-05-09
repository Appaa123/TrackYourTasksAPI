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
    }
}