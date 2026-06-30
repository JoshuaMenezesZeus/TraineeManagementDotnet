using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Trainee.Api.Models;

namespace Trainee.Api.DTO
{
    public class CreateTaskAssignmentRequest
    {
        [Required]
        public int TraineeId {get; set;}
        [Required]
        public int MentorId {get; set;}
        [Required]
        public int LearningTaskId {get; set;}

        [Required(ErrorMessage = "Assigned Date is Required")]
        public DateTime AssignedDate {get; set;} = DateTime.Now;
        [Required(ErrorMessage ="Due Date is required")]
        public DateTime DueDate {get; set;} = DateTime.Now.AddDays(5);
        [Required(ErrorMessage = "Task Assignment Status is Required")]
        public TaskAssignmentStatus Status {get; set;} = TaskAssignmentStatus.Assigned;
        public string Remarks {get; set;}= String.Empty;
    }
}