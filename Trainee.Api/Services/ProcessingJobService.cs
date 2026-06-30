using Trainee.Api.Data;
using Trainee.Api.DTO;

namespace Trainee.Api.Services
{
    public class ProcessingJobService : IProcessingJobService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ProcessingJobService> _logger;
        public ProcessingJobService(AppDbContext context, ILogger<ProcessingJobService> logger)
        {
            _logger = logger;
            _context=context;
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

    }
}