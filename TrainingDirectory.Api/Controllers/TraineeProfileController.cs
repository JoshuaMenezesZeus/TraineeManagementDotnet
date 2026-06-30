using Microsoft.AspNetCore.Mvc;
using TrainingDirectory.API.DTO;

namespace TrainingDirectory.API.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class TraineeProfileController : ControllerBase
    {
        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            return Ok(new TraineeProfileResponse
                {
                    Id = id,
                    Name="Joshua",
                    Email = "Joshua@gmail.com",
                    Designation="Software Engineer"   
                }
            );
        }
    }
}