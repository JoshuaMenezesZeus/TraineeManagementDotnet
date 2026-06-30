using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Trainee.Api.Models;

namespace Trainee.Api.DTO
{
    public class LearningTaskResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = String.Empty;
        public string Description {get; set;} = String.Empty;
        public string ExpectedTechStack{ get; set; } = String.Empty;
        public DateTime DueDate { get; set; } = DateTime.Now;
        public LearningTaskStatus Status { get; set; } = LearningTaskStatus.Draft;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
    }
}