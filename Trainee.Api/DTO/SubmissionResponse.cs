using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Trainee.Api.Models;

namespace Trainee.Api.DTO
{
    public class SubmissionResponse
    {
        public int Id {get; set;}
        public int TaskAssignmentId {get; set;}
        public DateTime AssignedDate {get; set;} = DateTime.Now;
        public DateTime DueDate {get; set;} = DateTime.Now;
        public TaskAssignmentStatus TaskAssignmentStatus {get; set;} = TaskAssignmentStatus.Assigned;
        public string SubmissionUrl {get; set;} = String.Empty;
        public string Notes {get; set;} = String.Empty;
        public DateTime SubmittedDate {get; set;} = DateTime.Now;
        public SubmissionStatus Status {get; set;} = SubmissionStatus.Submitted;
    }
}