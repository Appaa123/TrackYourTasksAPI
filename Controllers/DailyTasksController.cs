using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TrackYourTasksAPI.Models;
using TrackYourTasksAPI.Services;

namespace TrackYourTasksAPI.Controllers
{
    [ApiController]
    [Route("api/dailytasks")]
    public class DailyTasksController : ControllerBase
    {
        private readonly DailyTaskService _service;

        public DailyTasksController(DailyTaskService service)
        {
            _service = service;
        }

        // GET api/dailytasks
        // Returns an array of envelopes: [{ "task": { ... } }, ...]
        [HttpGet]
        public async Task<ActionResult<List<DailyTasks>>> Get()
        {
            var list = await _service.GetAsync();
            var envelopes = list.Select(t => new DailyTasks { Task = t }).ToList();
            return Ok(envelopes);
        }

        // GET api/dailytasks/{id}
        // Removed strict 24-length constraint so GUID ids are supported
        [HttpGet("{id}")]
        public async Task<ActionResult<DailyTasks>> GetById(string id)
        {
            var task = await _service.GetAsync(id);
            if (task is null) return NotFound();
            return Ok(new DailyTasks { Task = task });
        }

        // POST api/dailytasks
        // Accepts body: { "task": { ... } } and returns the created envelope
        [HttpPost]
        public async Task<ActionResult<DailyTasks>> Create([FromBody] DailyTasks envelope)
        {
            if (envelope?.Task is null)
                return BadRequest("Request body must contain a 'task' object.");

            var task = envelope.Task;

            // Ensure CreatedAt is set if not provided
            if (task.CreatedAt == default) task.CreatedAt = DateTime.UtcNow;

            await _service.CreateAsync(task);

            return CreatedAtAction(nameof(GetById), new { id = task.Id }, new DailyTasks { Task = task });
        }
    }
}