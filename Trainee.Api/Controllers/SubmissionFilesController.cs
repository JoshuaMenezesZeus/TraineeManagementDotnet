using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trainee.Api.DTO;
using Trainee.Api.Models;
using Trainee.Api.Services;

namespace Trainee.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/submission-files")]
    public class SubmissionFilesController: ControllerBase
    {

        private readonly ISubmissionFileService _service;
        public SubmissionFilesController(ISubmissionFileService service)
        {
            _service = service;
        }

        [HttpGet("{id:int}/download")]
        public async Task<ActionResult> DownloadFile(int id)
        {
            var file = await _service.DownloadAsync(id);
            return File(file.Stream, file.ContentType, file.FileName);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteFile(int id)
        {   
            await _service.DeleteAsync(id);
            return NoContent();
        }


    }
}