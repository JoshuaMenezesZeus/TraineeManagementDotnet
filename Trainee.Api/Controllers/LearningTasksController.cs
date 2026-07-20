using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trainee.Api.DTO;
using Trainee.Api.Models;
using Trainee.Api.Services;

namespace Trainee.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("/api/learning-tasks")]
    public class LearningTasksController : ControllerBase
    {
        private readonly ILearningTaskService _service;
        public LearningTasksController(ILearningTaskService service)
        {
            _service = service;
        }

        //routing
        [HttpGet]
        public async Task<ActionResult> GetAllLearningTasks(int? pageNumber, int? pageSize, string? search, LearningTaskStatus? status)
        {
                var (resp, count) = await _service.GetAllLearningTasks(pageNumber, pageSize, search, status);

                if (resp == null)
                    return NotFound();

                return Ok(new PaginationLearningTaskResponse() { PageNumber = pageNumber ?? 1, PageSize = pageSize ?? 10, TotalRecords = count, Data = resp });

        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetSpecificLearnigTask(int id)
        {
                if (id <= 0)
                    return BadRequest("ID should be greater than zero.");

                var res = await _service.GetLearningTaskById(id);
                if (res == null)
                    return NotFound();
                return Ok(res);
        }

        [HttpPost]
        public async Task<ActionResult> CreateNewLearningTask([FromBody] CreateLearningTask inputDTO)
        {
                return Ok(await _service.AddNewLearningTask(inputDTO));
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteLearnigTask(int id)
        {
                if (id <= 0)
                    return BadRequest("ID should be greater than zero.");

                if (await _service.DeleteLearningTask(id))
                    return NoContent();
                return NotFound();

        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateLearnigTask(int id, UpdateLearningTask request)
        {
                if (id <= 0)
                    return BadRequest("ID should be greater than zero.");
                var resp = await _service.EditLearningTask(id, request);
                if (resp == null)
                    return NotFound();

                return Ok(resp);
        }
    }
}