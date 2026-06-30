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
            // try
            // {
                var (resp, count) = await _service.GetAllLearningTasks(pageNumber, pageSize, search, status);

                if (resp == null)
                    return NotFound();

                return Ok(new PaginationLearningTaskResponse() { PageNumber = pageNumber ?? 1, PageSize = pageSize ?? 10, TotalRecords = count, Data = resp });
            // }
            // catch (Exception ex)
            // {
            //     return StatusCode(500, $"Internal Server Error: {ex.Message}");
            // }


        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetSpecificLearnigTask(int id)
        {
            // try
            // {
                if (id <= 0)
                    return BadRequest("ID should be greater than zero.");

                var res = await _service.GetLearningTaskById(id);
                if (res == null)
                    return NotFound();
                return Ok(res);
            // }
            // catch (Exception ex)
            // {
            //     return StatusCode(500, $"Internal Server Error: {ex.Message}");
            // }
        }

        [HttpPost]
        public async Task<ActionResult> CreateNewLearningTask([FromBody] CreateLearningTask inputDTO)
        {
            // try
            // {
                return Ok(await _service.AddNewLearningTask(inputDTO));

            // }
            // catch (Exception ex)
            // {
            //     return StatusCode(500, $"Internal Server Error: {ex.Message}");
            // }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteLearnigTask(int id)
        {
            // try
            // {
                if (id <= 0)
                    return BadRequest("ID should be greater than zero.");

                if (await _service.DeleteLearningTask(id))
                    return NoContent();
                return NotFound();
            // }
            // catch (Exception ex)
            // {
            //     return StatusCode(500, $"Internal Server Error: {ex.Message}");
            // }

        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateLearnigTask(int id, UpdateLearningTask request)
        {
            // try
            // {
                if (id <= 0)
                    return BadRequest("ID should be greater than zero.");
                var resp = await _service.EditLearningTask(id, request);
                if (resp == null)
                    return NotFound();

                return Ok(resp);
            // }
            // catch (Exception ex)
            // {
            //     return StatusCode(500, $"Internal Server Error: {ex.Message}");
            // }
        }
    }
}