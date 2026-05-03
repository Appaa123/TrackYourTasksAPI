using Microsoft.AspNetCore.Mvc;
using TrackYourTasksAPI.Models;
using TrackYourTasksAPI.Services;

namespace TrackYourTasksAPI.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    public class TasksController : ControllerBase
    {
        private readonly MongoTaskService _service;

        public TasksController(MongoTaskService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<List<TrackTask>> Get() =>
            await _service.GetAsync();

        [HttpPost]
        public async Task Create(TrackTask task) =>
            await _service.CreateAsync(task);

        [HttpPut("{id}")]
        public async Task Update(string id, TrackTask task) =>
            await _service.UpdateAsync(id, task);

        [HttpDelete("{id}")]
        public async Task Delete(string id) =>
            await _service.DeleteAsync(id);
    }
}
