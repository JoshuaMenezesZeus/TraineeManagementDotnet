namespace Trainee.Api.Settings
{
    public class RabbitMQSettings
    {
        public string Host {get; set;} = String.Empty;
        public int Port {get; set;}
        public string VirtualHost {get; set;} = String.Empty;
        public string Username {get; set;} = String.Empty;
        public string Password {get; set;} = String.Empty;
        public string SubmissionProcessingQueueName {get; set;}= "submission-processing";
    }
}