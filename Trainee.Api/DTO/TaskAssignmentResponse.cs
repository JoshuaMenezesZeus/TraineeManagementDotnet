using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Trainee.Api.Models;

namespace Trainee.Api.DTO
{
    public class TaskAssignmentResponse
    {
        public int Id {get; set;}
        public int TraineeId {get; set;}
        public string TraineeName {get; set;} = String.Empty;
        public int MentorId {get; set;}
        public string MentorName {get; set;} = String.Empty;

        // public MentorResponse Mentor {get; set;} = null!;
        public int LearningTaskId {get; set;}
        public string LearningTaskTitle {get; set;} = String.Empty;
        public string LearningTaskDescription {get; set;} = String.Empty;

        // public LearningTaskResponse LearningTask {get; set;} = null!;
        public DateTime AssignedDate {get; set;} = DateTime.Now;
        public DateTime DueDate {get; set;} = DateTime.Now.AddDays(5);
        public TaskAssignmentStatus Status {get; set;}= TaskAssignmentStatus.Assigned;
        public string Remarks {get; set;}= String.Empty;
    }
}