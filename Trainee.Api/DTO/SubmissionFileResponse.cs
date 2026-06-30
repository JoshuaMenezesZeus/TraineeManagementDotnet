namespace Trainee.Api.DTO
{
    public class SubmissionFileResponse
    {
        public int Id {get; set;}
        public string OriginalFileName {get; set;} = "";
        public string ContentType {get; set;}= "";
        public long FileSize {get; set;}
        public DateTime CreatedDate {get; set;} = DateTime.Now;
        public string CorrelationId {get; set;} = String.Empty;
    }
}