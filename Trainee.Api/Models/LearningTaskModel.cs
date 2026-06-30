using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Trainee.Api.Models
{
    public enum LearningTaskStatus
    {
        Draft, Published, Closed
    }
    public class LearningTaskModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; } = String.Empty;

        [Required(ErrorMessage = "Description is required")]
        public string Description {get; set;} = String.Empty;

        [Required(ErrorMessage = "Expected Tech Stack is required")]
        public string ExpectedTechStack{ get; set; } = String.Empty;

        public DateTime DueDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Learning Task is required")]
        public LearningTaskStatus Status { get; set; } = LearningTaskStatus.Draft;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
    }
}