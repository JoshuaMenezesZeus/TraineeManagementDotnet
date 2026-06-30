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
    [Route("api/processing-jobs")]
    public class ProcessingJobController : ControllerBase
    {
        private readonly IProcessingJobService _service;
        public ProcessingJobController(IProcessingJobService service)
        {
            _service = service;
        }


        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetById(int id)
        {
                var res =  await _service.GetByIdAsync(id);
                if (res==null)
                    return NotFound();
                return Ok(res);

        }
    }
}