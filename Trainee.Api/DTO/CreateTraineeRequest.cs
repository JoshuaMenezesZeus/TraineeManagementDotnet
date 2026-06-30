using Trainee.Api.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace Trainee.Api.DTO
{
    public class CreateTraineeRequest
    {
        [Required(ErrorMessage = "First name is required")][MaxLength(50)] public string FirstName { get; set; }= String.Empty;
        [Required(ErrorMessage = "Last name is required")][MaxLength(50)] public string LastName { get; set; }= String.Empty;
        [Required(ErrorMessage = "Email is required")][EmailAddress] public string Email { get; set; }= String.Empty;
        [Required(ErrorMessage = "Tech Stack is required")] public string TechStack { get; set; }= String.Empty;
        
        [Required(ErrorMessage = "Status is required")] 
        [EnumDataType(typeof(TraineeStatus), ErrorMessage ="Valid Enum is required")] 
        public TraineeStatus Status { get; set; } = TraineeStatus.Active;

    }
}

