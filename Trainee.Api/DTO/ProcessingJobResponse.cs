using Trainee.Api.Models;

namespace Trainee.Api.DTO
{
    public class ProcessingJobResponse
    {
        public int Id {get; set;}
        public Guid MessageID {get; set;}
        public string CorrelationId {get; set;} = String.Empty;
        public int SubmissionId {get; set;}
        public int FileId {get; set;}
        public ProcessingJobStatus ProcessingJobStatus {get; set;}= ProcessingJobStatus.Queued;
        public int Attempts {get; set;} = 0;
        public string? ErrorSummary {get; set;} = String.Empty;
        public DateTime StartedAt {get; set;}
        public DateTime CompletedAt {get; set;}
    }
}