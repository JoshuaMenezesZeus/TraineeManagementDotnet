using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SubmissionProcessor.Worker.Settings;
using System.Collections.Generic;
using SubmissionProcessor.Worker.Models;
using SubmissionProcessor.Worker.Data;
using SubmissionProcessor.Worker.Services;
using SubmissionProcessor.Worker.DTO;

namespace SubmissionProcessor.Worker.Workers;
 
public class SubmissionProcessingWorker : BackgroundService
{
    private readonly RabbitMQSettings _settings;
    private readonly ILogger<SubmissionProcessingWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private IConnection? _connection;
    private IChannel? _channel;
 
    public SubmissionProcessingWorker(IOptions<RabbitMQSettings> options, ILogger<SubmissionProcessingWorker> logger,
    IServiceScopeFactory scopeFactory)
    {
        _settings = options.Value;
        _logger = logger;
        _scopeFactory=scopeFactory;
    }
 
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            
            var factory = new ConnectionFactory
            {
                HostName = _settings.Host,  
                Port = _settings.Port,              
                VirtualHost = _settings.VirtualHost,
                UserName = _settings.Username,
                Password = _settings.Password
            };
    
            _connection = await factory.CreateConnectionAsync(cancellationToken);
            if (!_connection.IsOpen)
            {
                throw new Exception("Connection to RabbitMQ couldn't be made.");
                
            }
            _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
    
            //DEAD LETTER QUEUE
            string dlxName = _settings.DlxName;
            string dlqName = _settings.DlqName;
    
            await _channel.ExchangeDeclareAsync(
                exchange: dlxName,
                type: ExchangeType.Direct,
                durable: true,
                cancellationToken: cancellationToken
            );
    
            await _channel.QueueDeclareAsync(
                queue: dlqName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                cancellationToken: cancellationToken
            );
    
            await _channel.QueueBindAsync(
                queue: dlqName,
                exchange: dlxName,
                routingKey: "dead-letter",
                cancellationToken: cancellationToken
            );
            var mainQueueArguments = new Dictionary<string, object?>
            {
                { "x-dead-letter-exchange", dlxName },
                { "x-dead-letter-routing-key", "dead-letter" }
            };
        
            await _channel.QueueDeclareAsync(
                queue: _settings.SubmissionProcessingQueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: mainQueueArguments,
                cancellationToken: cancellationToken
            );
        
            await _channel.BasicQosAsync(
                prefetchSize: 0,
                prefetchCount: 1,
                global: false,
                cancellationToken: cancellationToken
            );
    
            await base.StartAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Connection to Rabbit MQ Failed");
            await Task.Delay(TimeSpan.FromSeconds(15), cancellationToken);
            await StartAsync(cancellationToken); 
        }
    }
 
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_channel == null)
        {
            throw new InvalidOperationException("RabbitMQ channel is not initialized");
        }
       
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                var message = JsonSerializer.Deserialize<SubmissionProcessRequested>(json);
                if (message == null)
                {
                    _logger.LogWarning("Received Invalid or Empty Message Payload");
                    await _channel.BasicNackAsync(
                        deliveryTag: ea.DeliveryTag,
                        multiple: false,
                        requeue: false,
                        cancellationToken: stoppingToken
                    );
                    return;
                }
 
                using (var scope = _scopeFactory.CreateScope())
                {
                    var _context=scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var existingJob=await _context.ProcessingJobs.FirstOrDefaultAsync(x=>x.CorrelationId==message.CorrelationId);
 
                    //IDEMPOTENCY DONE HERE
                    if(existingJob!=null && existingJob.ProcessingJobStatus==ProcessingJobStatus.Completed)
                    {
                        _logger.LogInformation("Duplicate message by RabbitMQ ignored");
                        await _channel.BasicAckAsync(ea.DeliveryTag,false,stoppingToken);
                        return;
                    }
                    _logger.LogInformation("Received message. MessageId:{MessageID}, CorrelationId:{CorrelationId}, SubmissionId:{SubmissionId}",
                        message.MessageID, message.CorrelationId, message.SubmissionId);
                        
                   
                    if (existingJob == null)
                    {
                        existingJob = new ProcessingJob
                        {
                            MessageID=message.MessageID,
                            CorrelationId=message.CorrelationId ?? "no context",
                            SubmissionId=message.SubmissionId,
                            FileId=message.FileId,
                            ProcessingJobStatus = ProcessingJobStatus.Processing,
                            Attempts = 0,
                            StartedAt = DateTime.Now
                        };
                        _context.ProcessingJobs.Add(existingJob);
                        await _context.SaveChangesAsync(stoppingToken);
                    }
                    else if(existingJob.ProcessingJobStatus==ProcessingJobStatus.Queued)
                    {
                        existingJob.ProcessingJobStatus=ProcessingJobStatus.Processing;
                        existingJob.StartedAt=DateTime.Now;
                        _logger.LogInformation("Processing of Job {existingJobId} for message {MessageID} has started. Status: {Status}", existingJob.Id, message.MessageID, existingJob.ProcessingJobStatus);
                    }
 
                    //SIMULATING THE PROCESSING
                    try
                    {
                        var metadata=await _context.SubmissionFiles.FindAsync(message.FileId);
                        
                        
                        // throw new Exception("Simulating transient failure for testing.");
                        
                        // Permanent Failure check
                        if (metadata != null)
                        {
                            _logger.LogInformation("Metadata of the File is: ID: {FileId}, Name: {FileName}, Size: {FileSize} bytes, ContentType: {ContentType}",
                                metadata.Id, metadata.OriginalFileName, metadata.Size, metadata.ContentType);


                            _logger.LogInformation("Fetching trainee profile from directory");
                            var directoryClient = scope.ServiceProvider.GetRequiredService<ITraineeDirectoryClient>();
                            TraineeProfileResponse? traineeProfile = null;

                            try
                            {
                                traineeProfile = await directoryClient.GetProfile(
                                    123,
                                    message.CorrelationId,
                                    stoppingToken
                                );
                            }
                            catch (Exception ex) 
                            {
                                _logger.LogWarning(ex, "Training Directory service is unavailable after resilience retries. Executing fallback policy.");
                                traineeProfile = new TraineeProfileResponse
                                {
                                    Name = "Fallback Name (Service Offline)",
                                    Email = "offline@system.com",
                                    Designation = "Unknown",
                                };
                            
                            }
                            if (traineeProfile != null)
                            {
                                _logger.LogInformation("Successfully retrieved profile ID:{Id} Name: {Name} Email: {Email} Designation: {Designation}", 
                                    traineeProfile.Id, traineeProfile.Name, traineeProfile.Email, traineeProfile.Designation);
                            }
                            else
                            {
                                _logger.LogWarning("Trainee profile was not found or returned null for CorrelationId: {CorrelationId}", message.CorrelationId);
                            }




                            existingJob.ProcessingJobStatus=ProcessingJobStatus.Completed;
                            existingJob.CompletedAt = DateTime.Now;
                            existingJob.Attempts++;
                            _logger.LogInformation("Processing of Job {existingJobId} for message {MessageID} has completed. Status: {Status}", existingJob.Id, message.MessageID, existingJob.ProcessingJobStatus);
                            await _context.SaveChangesAsync(stoppingToken);
                        }
                        else
                        {
                            // Permanent Failure
                            _logger.LogWarning("Metadata file not found for FileId: {FileId}", message.FileId);
                            throw new FileNotFoundException($"Metadata file not found for FileId: {message.FileId}");
                        }
                       
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Business processing logic failed for MessageId {MessageId}", message.MessageID);
                        existingJob.ErrorSummary = ex.Message;
                        bool isPermanentFailure = ex is FileNotFoundException;
                        bool isRetryExhausted = existingJob.Attempts >= _settings.MaxRetryAttempts;
                        if(isPermanentFailure || isRetryExhausted)
                        {
                            existingJob.ProcessingJobStatus = ProcessingJobStatus.Failed;
                            await _context.SaveChangesAsync();

                            _logger.LogCritical("Processing of Job {existingJobId} for message {MessageID} has failed. Status: {Status}.", existingJob.Id, message.MessageID, existingJob.ProcessingJobStatus);
                            await _channel.BasicNackAsync(
                                deliveryTag: ea.DeliveryTag,
                                multiple: false,
                                cancellationToken: stoppingToken,
                                requeue:false
                            );
                            _logger.LogCritical("Adding Message to Dead Letter Queue...");
                        }
                        else
                        {
                            existingJob.ProcessingJobStatus = ProcessingJobStatus.Queued;
                            existingJob.Attempts++;
                            await _context.SaveChangesAsync();
                            
                            _logger.LogCritical("Attempt: {attempt}. Changing Status to {status}. Retrying... ", existingJob.Attempts ,existingJob.ProcessingJobStatus);

                            await _channel.BasicNackAsync(
                                deliveryTag: ea.DeliveryTag,
                                multiple: false,
                                requeue: true,
                                cancellationToken: stoppingToken
                            );
                        }
                        return;
                    }                    
                }
               
                await _channel.BasicAckAsync(
                    deliveryTag: ea.DeliveryTag,
                    multiple: false,
                    cancellationToken: stoppingToken
                );
               
                _logger.LogInformation("Acknowledged Message {MessageId}", message.MessageID);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error while processing RabbitMQ message {e.Message}");
                if(_channel!=null)
                {
                    await _channel.BasicNackAsync(
                                deliveryTag: ea.DeliveryTag,
                                multiple: false,
                                requeue: true,
                                cancellationToken: stoppingToken
                            );
                }
            }
        };
 
        await _channel.BasicConsumeAsync(
            queue: _settings.SubmissionProcessingQueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);
           
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }
 

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel != null)
        {
            await _channel.CloseAsync(cancellationToken: cancellationToken);
            await _channel.DisposeAsync();
        }
 
        if (_connection != null)
        {
            await _connection.CloseAsync(cancellationToken: cancellationToken);
            await _connection.DisposeAsync();
        }
        await base.StopAsync(cancellationToken);
    }
}