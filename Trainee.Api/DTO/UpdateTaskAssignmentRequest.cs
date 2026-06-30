using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Trainee.Api.Models;

namespace Trainee.Api.DTO
{
    public class UpdateTaskAssignmentRequest
    {
        [Required(ErrorMessage = "Task Assignment Status is Required")]
        public TaskAssignmentStatus Status {get;set;} = TaskAssignmentStatus.Assigned;
    }
}