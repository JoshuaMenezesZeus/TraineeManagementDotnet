namespace Trainee.Api.DTO
{
    public class FileDownloadResponse
    {
        public Stream Stream {get; set;} = null!;
        public string ContentType {get; set;}= "";
        public string FileName {get; set;}= "";
    }
}