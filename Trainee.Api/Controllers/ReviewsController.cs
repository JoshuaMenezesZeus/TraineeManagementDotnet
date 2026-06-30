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
    public class ReviewsController: ControllerBase
    {

        private readonly IReviewService _service;
        public ReviewsController(IReviewService service)
        {
            _service = service;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllReviews()
        {
            // try
            // {
                var resp = await _service.GetAllReviews();
                return Ok(resp);
            // }
            // catch (Exception ex)
            // {
            //     return StatusCode(500, $"Internal Server Error: {ex.Message}");
            // }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetByIdReview(int id)
        {
            // try
            // {
                if (id <= 0)
                    return BadRequest("ID should be greater than zero.");

                var res = await _service.GetByIdReview(id);
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
        public async Task<ActionResult> AddANewReview([FromBody] CreateReviewRequest inputDTO)
        {
            // try
            // {
                return Ok(await _service.AddANewReview(inputDTO));

            // }
            // catch (Exception ex)
            // {
            //     return StatusCode(500, $"Internal Server Error: {ex.Message}");
            // }
        }
    }
}