using MongoDB.Driver;
using TrackYourTasksAPI.Models;


namespace TrackYourTasksAPI.Services
{
    public class MongoTaskService
    {
        private readonly IMongoCollection<TrackTask> _tasks;

        public MongoTaskService(IConfiguration config)
        {
            var client = new MongoClient(config["MongoDb:ConnectionString"]);
            var database = client.GetDatabase(config["MongoDb:DatabaseName"]);

            _tasks = database.GetCollection<TrackTask>("TYTTasks");
        }

        public async Task<List<TrackTask>> GetAsync() =>
            await _tasks.Find(_ => true).ToListAsync();

        public async Task CreateAsync(TrackTask task) =>
            await _tasks.InsertOneAsync(task);

        public async Task UpdateAsync(string id, TrackTask task) =>
            await _tasks.ReplaceOneAsync(x => x.Id == id, task);

        public async Task DeleteAsync(string id) =>
            await _tasks.DeleteOneAsync(x => x.Id == id);
    }
}