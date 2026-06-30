using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Trainee.Api.Settings;

namespace Trainee.Api.Services
{
    public class RabbitMQConnectionService:IRabbitMQConnectionService
    {
        private readonly RabbitMQSettings _settings;
        public RabbitMQConnectionService(IOptions<RabbitMQSettings> options)
        {
            _settings = options.Value;

        }
        public async Task<IConnection> CreateConnection()
        {
            var factory = new ConnectionFactory
            {
                HostName = _settings.Host,
                Port = _settings.Port,
                UserName = _settings.Username,
                Password = _settings.Password,
                VirtualHost = _settings.VirtualHost,
            };
            return await factory.CreateConnectionAsync();
        }
    }
}