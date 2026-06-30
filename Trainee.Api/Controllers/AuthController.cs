using Microsoft.AspNetCore.Mvc;
using Trainee.Api.Data;
using Trainee.Api.DTO;
using Trainee.Api.Models;
using Trainee.Api.Services;
using Microsoft.AspNetCore.Authorization;

namespace Trainee.Api.Controllers
{
    [AllowAnonymous]
    [ApiController]   
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authservice;
        public AuthController(IAuthService service)
        {
            _authservice = service;
        }


        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginRequest request)
        {
            // try
            // {
                var resp =  await _authservice.Login(request);
                if (resp==null)
                    return BadRequest();
                return Ok(resp);
            // }

            // catch (Exception ex)
            // {
            //     return StatusCode(500, $"Internal Server Error: {ex.Message}");
            // }

        }
    }
}