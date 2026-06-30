using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Trainee.Api.Controllers
{
    [AllowAnonymous]
    [ApiController]  
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly HealthCheckService _healthcheckservice;

        public HealthController(HealthCheckService healthCheckService)
        {
            _healthcheckservice = healthCheckService;
        }

        [HttpGet("live")]
        public IActionResult Live()
        {
            return Ok(new
            {
                status = "Application is alive"
            });
        }

        [HttpGet("ready")]
        public async Task<IActionResult> Ready()
        {
            var report = await _healthcheckservice.CheckHealthAsync();
            var response = new
            {
                status = report.Status.ToString(),
                checks = report.Entries.Select(x => new
                {
                    service = x.Key,
                    status = x.Value.Status.ToString(),
                    description = x.Value.Description
                })
            };

            if (report.Status == HealthStatus.Healthy)
                return Ok(response);
            return StatusCode(503, response);
        }

        [HttpGet]
        public IActionResult Status()
        {
            return Ok(new
            {
                status = "running",
                application = "Trainee Management API",
                timestamp = DateTime.Now
            }
            );
        }
    }
}