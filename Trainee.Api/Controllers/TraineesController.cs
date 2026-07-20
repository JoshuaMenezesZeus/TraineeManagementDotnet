using Microsoft.AspNetCore.Mvc;
using Trainee.Api.Data;
using Trainee.Api.DTO;
using Trainee.Api.Models;
using Trainee.Api.Services;
using Microsoft.AspNetCore.Authorization;

namespace Trainee.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]

    public class TraineesController : ControllerBase
    {
        private readonly ITraineeService _traineeservice;
        public TraineesController(ITraineeService service)
        {
            _traineeservice = service;
        }

        //routing
        [HttpGet]
        public async Task<ActionResult> GetAllTrainees(int? pageNumber , int? pageSize, string? search, TraineeStatus? status)
        {
                var (resp, count) = await _traineeservice.GetAllTrainees(pageNumber, pageSize, search, status);

                if (resp == null)
                    return NotFound();

                return Ok(new PaginationResponse(){PageNumber=pageNumber ?? 1, PageSize = pageSize ?? 10, TotalRecords=count, Data=resp});
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetSpecificTrainee(int id)
        {
                if (id <= 0)
                    return BadRequest("ID should be greater than zero.");

                var res = await _traineeservice.GetById(id);
                if (res == null)
                    return NotFound();
                return Ok(res);
        }

        [HttpPost]
        public async Task<ActionResult> CreateNewTrainee([FromBody] CreateTraineeRequest inputDTO)
        {
                return Ok(await _traineeservice.AddANewTrainee(inputDTO));
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteTrainee(int id)
        {
                if (id <= 0)
                    return BadRequest("ID should be greater than zero.");

                if (await _traineeservice.DeleteTrainee(id))
                    return NoContent();
                return NotFound();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateTrainee(int id, UpdateTraineeRequest request)
        {
                if (id <= 0)
                    return BadRequest("ID should be greater than zero.");
                var resp = await _traineeservice.EditTrainee(id, request);
                if (resp == null)
                    return NotFound();

                return Ok(resp);
        }
    }


}