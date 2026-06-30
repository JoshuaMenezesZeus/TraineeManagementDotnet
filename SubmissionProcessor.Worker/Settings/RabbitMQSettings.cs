namespace SubmissionProcessor.Worker.Settings
{
    public class RabbitMQSettings
    {
        public string Host {get; set;} = String.Empty;
        public int Port {get; set;}
        public string VirtualHost {get; set;} = String.Empty;
        public string Username {get; set;} = String.Empty;
        public string Password {get; set;} = String.Empty;
        public string SubmissionProcessingQueueName {get; set;}= "submission-processing";
        public int MaxRetryAttempts {get; set;}= 3;
        public string DlxName {get; set;}= "submission-processing-dead-letter-exchange";
        public string DlqName {get; set;}= "submission-processing-failed";
    }
}