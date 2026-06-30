using Trainee.Api.Models;

namespace Trainee.Api.Data
{
    public static class TraineeStore
    {
        public static List<TraineeModel> Trainees = new List<TraineeModel> {
            new TraineeModel
            {
                Id = 1,
                FirstName = "Joshua",
                LastName = "Menezes",
                Email="joshua@gmail.com",
                TechStack="Python",
                Status=TraineeStatus.Active,
                CreatedDate=DateTime.Now,
                UpdatedDate=DateTime.Now,
            }
        };

    }
}