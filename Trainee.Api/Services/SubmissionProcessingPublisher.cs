using Microsoft.Extensions.Options;
using Trainee.Api.Models;
using Trainee.Api.Settings;
using System;
using System.Text;
using System.Text.Json;
using System.Net.Mime;
using RabbitMQ.Client.Exceptions;
using RabbitMQ.Client;

namespace Trainee.Api.Services
{
    public class SubmissionProcessingPublisher : ISubmissionProcessingPublisher
    {
        private readonly IRabbitMQConnectionService _connectionFactory;
        private readonly ILogger<SubmissionProcessingPublisher> _logger;
        private readonly RabbitMQSettings _settings;
        public SubmissionProcessingPublisher(IRabbitMQConnectionService connectionFactory, IOptions<RabbitMQSettings> options, ILogger<SubmissionProcessingPublisher> logger)
        {
            _logger = logger;
            _connectionFactory = connectionFactory;
            _settings = options.Value;
        }
        public async Task PublishAsync(SubmissionProcessRequested message)
        {
            try
            {
                await using var connection = await _connectionFactory.CreateConnection();
                await using var channel = await connection.CreateChannelAsync();
                var mainQueueArguments = new Dictionary<string, object?>
            {
                { "x-dead-letter-exchange", "submission-processing-dead-letter-exchange" },
                { "x-dead-letter-routing-key", "dead-letter" }
            };
 
                await channel.QueueDeclareAsync(
                    queue: _settings.SubmissionProcessingQueueName,
                    durable:true,
                    exclusive:false,
                    autoDelete:false,
                    arguments: mainQueueArguments
                );
                var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
                var properties = new BasicProperties
                {
                    Persistent = true,
                    MessageId = message.MessageID.ToString(),
                    CorrelationId = message.CorrelationId,
                    ContentType = "application/json",
                    Type = nameof(SubmissionProcessRequested)
                };

                await channel.BasicPublishAsync(
                    exchange: string.Empty,
                    routingKey: _settings.SubmissionProcessingQueueName,
                    mandatory: true,
                    basicProperties: properties,
                    body: body
                );

                _logger.LogInformation("Published message {id1} for Submission {id2} for File {id3}", message.MessageID, message.SubmissionId, message.FileId);
                    
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Connection cannot be made to RabbitMQ {ex}", ex.Message);
                throw new Exception("Connection cannot be made.");
            }
        }
    }
}