namespace SubmissionProcessor.Worker.Models
{
    public class SubmissionProcessRequested
    {
        public Guid MessageID {get; set;} = Guid.NewGuid();
        public string? CorrelationId {get; set;}
        public int SubmissionId {get; set;}
        public int FileId {get; set;}
        public DateTime RequestedAt {get; set;} = DateTime.Now;
        public int ContractVersion {get; set;} =1;
    }
}