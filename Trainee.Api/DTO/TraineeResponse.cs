using Trainee.Api.Models;

namespace Trainee.Api.DTO
{
    public class TraineeResponse
    {
        public int Id {get; set;}
        public string FirstName {get; set;} = String.Empty;
        public string LastName {get; set;} = String.Empty;
        public string Email {get; set;} = String.Empty;
        public string TechStack {get; set;} = String.Empty;
        public TraineeStatus Status {get; set;} = TraineeStatus.Active;
        public DateTime CreatedDate {get; set;} = DateTime.Now;
        public DateTime UpdatedDate {get; set;} = DateTime.Now;
    }
}