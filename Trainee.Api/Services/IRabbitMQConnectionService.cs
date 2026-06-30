using RabbitMQ.Client;
namespace Trainee.Api.Services
{
    public interface IRabbitMQConnectionService
    {
        Task<IConnection> CreateConnection();
    }
}