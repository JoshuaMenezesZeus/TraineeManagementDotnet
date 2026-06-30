using System.ComponentModel.DataAnnotations;
using Trainee.Api.Models;

namespace Trainee.Api.DTO
{
    public class PaginationResponse
    {
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public int TotalRecords {get; set;} = 25;

        public List<TraineeResponse> Data { get; set; } = [];
    }
}