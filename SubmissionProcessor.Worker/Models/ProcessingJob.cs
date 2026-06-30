using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SubmissionProcessor.Worker.Models
{
    public enum ProcessingJobStatus
    {
        Queued, Processing, Completed, Failed
    }
    public class ProcessingJob
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
