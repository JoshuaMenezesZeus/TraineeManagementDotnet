using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trainee.Api.Models
{
    public enum TaskAssignmentStatus
    {
        Assigned, InProgress, Submitted, Reviewed, Completed
    }
    public class TaskAssignmentModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id {get; set;}

        [ForeignKey("TraineeModel")]
        public int TraineeId {get; set;}
        public TraineeModel TraineeModel {get; set;} = null!;

        [ForeignKey("MentorModel")]
        public int MentorId {get; set;}
        public MentorModel MentorModel {get; set;} = null!;

        [ForeignKey("LearningTaskModel")]
        public int LearningTaskId {get; set;}
        public LearningTaskModel LearningTaskModel {get; set;} = null!;

        [Required(ErrorMessage = "Assigned Date is Required")]
        public DateTime AssignedDate {get; set;} = DateTime.Now;
        [Required(ErrorMessage ="Due Date is required")]
        public DateTime DueDate {get; set;} = DateTime.Now.AddDays(5);
        [Required(ErrorMessage = "Task Assignment Status is Required")]
        public TaskAssignmentStatus Status {get; set;}= TaskAssignmentStatus.Assigned;
        public string Remarks {get; set;}= String.Empty;
    }
}