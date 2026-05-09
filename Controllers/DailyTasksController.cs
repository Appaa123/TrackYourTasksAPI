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
        [HttpGet]
        public async Task<ActionResult<List<DailyTask>>> Get()
        {
            var list = await _service.GetAsync();
            return Ok(list);
        }

        // GET api/dailytasks/{id}
        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<DailyTask>> GetById(string id)
        {
            var task = await _service.GetAsync(id);
            if (task is null) return NotFound();
            return Ok(task);
        }

        // POST api/dailytasks
        [HttpPost]
        public async Task<ActionResult<DailyTask>> Create(DailyTask task)
        {
            await _service.CreateAsync(task);

            // If Mongo assigned an Id, return CreatedAtAction; otherwise return Ok
            if (!string.IsNullOrEmpty(task.Id))
                return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);

            return Ok(task);
        }
    }
}