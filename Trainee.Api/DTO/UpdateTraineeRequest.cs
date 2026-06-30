using Trainee.Api.Models;
using System.ComponentModel.DataAnnotations;
namespace Trainee.Api.DTO
{
    public class UpdateTraineeRequest
    {
        [Required(ErrorMessage ="First name is required")] [MaxLength(50)] public String FirstName {get; set;} = String.Empty;
        [Required(ErrorMessage ="Last name is required")] [MaxLength(50)] public String LastName {get; set;} = String.Empty;
        [Required(ErrorMessage ="Email is required")] [EmailAddress] public String Email {get; set;} = String.Empty;
        [Required(ErrorMessage ="Tech Stack is required")] public String TechStack {get; set;} = String.Empty;
        [Required(ErrorMessage ="Status is required")] public TraineeStatus Status {get; set;}
    }
}

