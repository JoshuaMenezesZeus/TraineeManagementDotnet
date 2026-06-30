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
            // try
            // {
                var (resp, count) = await _traineeservice.GetAllTrainees(pageNumber, pageSize, search, status);

                if (resp == null)
                    return NotFound();

                return Ok(new PaginationResponse(){PageNumber=pageNumber ?? 1, PageSize = pageSize ?? 10, TotalRecords=count, Data=resp});
            // }
            // catch (Exception ex)
            // {
            //     return StatusCode(500, $"Internal Server Error: {ex.Message}");
            // }


        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetSpecificTrainee(int id)
        {
            // try
            // {
                if (id <= 0)
                    return BadRequest("ID should be greater than zero.");

                var res = await _traineeservice.GetById(id);
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
        public async Task<ActionResult> CreateNewTrainee([FromBody] CreateTraineeRequest inputDTO)
        {
            // try
            // {
                return Ok(await _traineeservice.AddANewTrainee(inputDTO));

            // }
            // catch (Exception ex)
            // {
            //     return StatusCode(500, $"Internal Server Error: {ex.Message}");
            // }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteTrainee(int id)
        {
            // try
            // {
                if (id <= 0)
                    return BadRequest("ID should be greater than zero.");

                if (await _traineeservice.DeleteTrainee(id))
                    return NoContent();
                return NotFound();
            // }
            // catch (Exception ex)
            // {
            //     return StatusCode(500, $"Internal Server Error: {ex.Message}");
            // }

        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateTrainee(int id, UpdateTraineeRequest request)
        {
            // try
            // {
                if (id <= 0)
                    return BadRequest("ID should be greater than zero.");
                var resp = await _traineeservice.EditTrainee(id, request);
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