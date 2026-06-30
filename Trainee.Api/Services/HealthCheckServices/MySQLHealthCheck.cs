using Microsoft.Extensions.Diagnostics.HealthChecks;
using Trainee.Api.Data;

namespace Trainee.Api.Services.HealthCheckServices
{
    public class MySQLHealthCheck: IHealthCheck
    {
        private readonly AppDbContext _context;
        public MySQLHealthCheck(AppDbContext context)
        {
            _context = context;
        }   

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var canConnect = await _context.Database.CanConnectAsync(cancellationToken);
                if(canConnect)
                    return HealthCheckResult.Healthy("MySql is reachable");
                return HealthCheckResult.Unhealthy("MySql is unreachable");
                
            }
            catch (Exception ex)
            {
                
                return HealthCheckResult.Unhealthy("MySql is unreachable", ex);
            }
        }
    }
}