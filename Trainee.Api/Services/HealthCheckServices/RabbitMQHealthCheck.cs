using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Trainee.Api.Data;
using Trainee.Api.Settings;
using RabbitMQ.Client;

namespace Trainee.Api.Services.HealthCheckServices
{
    public class RabbitMQHealthCheck: IHealthCheck
    {
        private readonly RabbitMQSettings _settings;
        public RabbitMQHealthCheck(IOptions<RabbitMQSettings> options)
        {
            _settings = options.Value;
        }   

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {

            var factory = new ConnectionFactory
            {
                HostName = _settings.Host,
                Port = _settings.Port,
                UserName = _settings.Username,
                Password = _settings.Password,
                VirtualHost = _settings.VirtualHost,
            };
            await using var connection = await factory.CreateConnectionAsync(cancellationToken);
            if(connection.IsOpen)
                return HealthCheckResult.Healthy("RabbitmQ is reachable");
            return HealthCheckResult.Unhealthy("RabbitMQ is unreachable");

        }
    }
}