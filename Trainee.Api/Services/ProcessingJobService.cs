using Trainee.Api.Data;
using Trainee.Api.DTO;
using Trainee.Api.Models;

namespace Trainee.Api.Services
{
    public class ProcessingJobService : IProcessingJobService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ProcessingJobService> _logger;
        private readonly ISubmissionProcessingPublisher _messagePublisher;

        public ProcessingJobService(AppDbContext context, ILogger<ProcessingJobService> logger, ISubmissionProcessingPublisher messagePublisher)
        {
            _logger = logger;
            _context=context;
            _messagePublisher = messagePublisher;
        }

        public async Task<ProcessingJobResponse?> GetByIdAsync(int id)
        {
            var job = await _context.ProcessingJobs.FindAsync(id);
            if(job == null)
            {
                _logger.LogInformation("No job found with id: {id}", id);
                return null;
            }
            return new ProcessingJobResponse
            {
                Id = job.Id,
                MessageID = job.MessageID,
                CorrelationId=job.CorrelationId,
                SubmissionId=job.SubmissionId,
                FileId=job.FileId,
                ProcessingJobStatus=job.ProcessingJobStatus,
                Attempts=job.Attempts,
                ErrorSummary=job.ErrorSummary,
                StartedAt=job.StartedAt,
                CompletedAt=job.CompletedAt
            };
        }
        public async Task<ProcessingJobResponse?> RetryProcessingJob(int id)
        {
            var job = await _context.ProcessingJobs.FindAsync(id);
            if(job == null)
            {
                _logger.LogInformation("No job found with id: {id}", id);
                return null;
            }
            var message = new SubmissionProcessRequested
            {
                CorrelationId = job.CorrelationId,
                SubmissionId= job.SubmissionId,
                FileId= job.FileId,
            }; 
            using var transaction = await _context.Database.BeginTransactionAsync();  
            try
            {   
                job.ProcessingJobStatus = ProcessingJobStatus.Queued;
                // job.Attempts++;
                await _context.SaveChangesAsync();

                await _messagePublisher.PublishAsync(message);
                await transaction.CommitAsync();
            }
            catch(Exception ex)
            {
                await transaction.RollbackAsync();
                _context.ChangeTracker.Clear();
                throw new Exception("Connection to RabbitMQ Failed at Publisher side!");
            }
   
            return new ProcessingJobResponse
            {
                Id = job.Id,
                MessageID = message.MessageID,
                CorrelationId=job.CorrelationId,
                SubmissionId=job.SubmissionId,
                FileId=job.FileId,
                ProcessingJobStatus=job.ProcessingJobStatus,
                Attempts=job.Attempts,
                ErrorSummary=job.ErrorSummary,
                StartedAt=job.StartedAt,
                CompletedAt=job.CompletedAt
            };
                
    
        }

    }
}