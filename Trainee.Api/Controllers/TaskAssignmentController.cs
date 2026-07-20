using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trainee.Api.DTO;
using Trainee.Api.Models;
using Trainee.Api.Services;

namespace Trainee.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/task-assignment")]
    public class TaskAssignmentController: ControllerBase
    {

        private readonly ITaskAssignmentService _service;
        public TaskAssignmentController(ITaskAssignmentService service)
        {
            _service = service;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllTaskAssignment()
        {
                var resp = await _service.GetAllTaskAssignments();
                return Ok(resp);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
                if (id <= 0)
                    return BadRequest("ID should be greater than zero.");

                var res = await _service.GetByIdTaskAssignment(id);
                if (res == null)
                    return NotFound();
                return Ok(res);
        }

        [HttpPost]
        public async Task<ActionResult> CreateNewTaskAssignment([FromBody] CreateTaskAssignmentRequest inputDTO)
        {
                return Ok(await _service.AddANewTaskAssignment(inputDTO));

        }



        [HttpPut("{id:int}/status")]
        public async Task<ActionResult> UpdateTaskAssingnmentStatus(int id, UpdateTaskAssignmentRequest request)
        {
                if (id <= 0)
                    return BadRequest("ID should be greater than zero.");
                var resp = await _service.UpdateTaskAssingnmentStatus(id, request);
                if (resp == false)
                    return NotFound();

                return Ok(new {
                    resp,
                    message="Task status changed successfully"
                });
        }
    }
}