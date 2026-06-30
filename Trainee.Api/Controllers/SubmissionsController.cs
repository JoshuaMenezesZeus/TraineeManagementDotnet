using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trainee.Api.DTO;
using Trainee.Api.Models;
using Trainee.Api.Services;

namespace Trainee.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SubmissionsController: ControllerBase
    {

        private readonly ISubmissionService _service;
        private readonly ISubmissionFileService _service2;
        public SubmissionsController(ISubmissionService service, ISubmissionFileService service2)
        {
            _service = service;
            _service2 = service2;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllSubmissions()
        {
            // try
            // {
                var resp = await _service.GetAllSubmissions();
                return Ok(resp);
            // }
            // catch (Exception ex)
            // {
            //     return StatusCode(500, $"Internal Server Error: {ex.Message}");
            // }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetByIdSubmission(int id)
        {
            // try
            // {
                if (id <= 0)
                    return BadRequest("ID should be greater than zero.");

                var res = await _service.GetByIdSubmission(id);
                if (res == null)
                    return NotFound();
                return StatusCode(201, res);
            // }
            // catch (Exception ex)
            // {
            //     return StatusCode(500, $"Internal Server Error: {ex.Message}");
            // }
        }

        [HttpPost]
        public async Task<ActionResult> AddANewSubmission([FromBody] CreateSubmissionRequest inputDTO)
        {
            // try
            // {
                return Ok(await _service.AddANewSubmission(inputDTO));

            // }
            // catch (Exception ex)
            // {
            //     return StatusCode(500, $"Internal Server Error: {ex.Message}");
            // }
        }

        [DisableRequestSizeLimit]
        [RequestFormLimits(MultipartBodyLengthLimit=20971520)]
        [HttpPost("{submissionId:int}/files")]
        public async Task<ActionResult> AddNewFile(int submissionId, IFormFile file, int userId)
        {
            var resp = await _service2.UploadAsync(submissionId, file, userId);
            return Accepted(resp);
                // return Ok(await _service.SaveAsync());
        }
    }
}