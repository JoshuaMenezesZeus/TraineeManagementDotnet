using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Trainee.Api.Models;

namespace Trainee.Api.DTO
{
    public class UpdateLearningTask
    {
        public string Title { get; set; } = String.Empty;

        [Required(ErrorMessage = "Description is required")]
        public string Description {get; set;} = String.Empty;

        [Required(ErrorMessage = "Expected Tech Stack is required")]
        public string ExpectedTechStack{ get; set; } = String.Empty;

        public DateTime DueDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Learning Task is required")]
        public LearningTaskStatus Status { get; set; } = LearningTaskStatus.Draft;

    }
}