using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trainee.Api.DTO;
using Trainee.Api.Models;
using Trainee.Api.Services;

namespace Trainee.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("/api/[controller]")]
    public class MentorsController : ControllerBase
    {
        private readonly IMentorService _service;
        public MentorsController(IMentorService service)
        {
            _service = service;
        }

        //routing
        [HttpGet]
        public async Task<ActionResult> GetAllMentors(int? pageNumber, int? pageSize, string? search, MentorStatus? status)
        {
                var (resp, count) = await _service.GetAllMentors(pageNumber, pageSize, search, status);

                if (resp == null)
                    return NotFound();

                return Ok(new PaginationMentorResponse() { PageNumber = pageNumber ?? 1, PageSize = pageSize ?? 10, TotalRecords = count, Data = resp });
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetSpecificMentor(int id)
        {
                if (id <= 0)
                    return BadRequest("ID should be greater than zero.");

                var res = await _service.GetMentorById(id);
                if (res == null)
                    return NotFound();
                return Ok(res);
        }

        [HttpPost]
        public async Task<ActionResult> CreateNewMentor([FromBody] CreateMentorRequest inputDTO)
        {
                return Ok(await _service.AddANewMentor(inputDTO));

        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteMentor(int id)
        {
                if (id <= 0)
                    return BadRequest("ID should be greater than zero.");

                if (await _service.DeleteMentor(id))
                    return NoContent();
                return NotFound();

        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateMentor(int id, UpdateMentorRequest request)
        {
                if (id <= 0)
                    return BadRequest("ID should be greater than zero.");
                var resp = await _service.EditMentor(id, request);
                if (resp == null)
                    return NotFound();

                return Ok(resp);
        }
    }
}