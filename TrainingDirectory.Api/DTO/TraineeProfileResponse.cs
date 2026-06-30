namespace TrainingDirectory.API.DTO
{
    public class TraineeProfileResponse
    {
        public int Id {get; set;}
        public string Name {get; set;} = String.Empty;
        public string Email {get; set;} = String.Empty;
        public string Designation {get; set;} = string.Empty;
    }
}