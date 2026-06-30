using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SubmissionProcessor.Worker.Models
{
    public class SubmissionFileModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id {get; set;}
        
        [ForeignKey("SubmissionModel")]
        public int SubmissionId {get; set;}
        public SubmissionModel SubmissionModel {get; set;} = null!;
        [Required(ErrorMessage ="File Name should be present")]
        public string OriginalFileName {get; set;} = String.Empty;
        [Required(ErrorMessage ="Stored file Name should be present")]
        public string StorageFileName {get; set;} = String.Empty;
        [Required(ErrorMessage ="Content Type should be present")]
        public string ContentType {get; set;} = String.Empty;
        [Required(ErrorMessage ="File Size should be present")]
        public long Size {get; set;}
        [Required(ErrorMessage ="CheckSum should be present")]
        public string CheckSum {get; set;} = String.Empty;
        [Required(ErrorMessage ="User ID should be present")]
        public int  UploadedByUserId {get; set;}
        [Required(ErrorMessage ="Created Date should be present")]
        public DateTime CreatedDate {get; set;} = DateTime.Now;
    }
}