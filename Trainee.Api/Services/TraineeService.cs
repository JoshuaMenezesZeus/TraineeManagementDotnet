using Microsoft.AspNetCore.Http.HttpResults;
using Trainee.Api.Data;
using Trainee.Api.DTO;
using Trainee.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Trainee.Api.Services
{
    public class TraineeService : ITraineeService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TraineeService> _logger;
        private readonly IRedisService _cache;
        public TraineeService(AppDbContext context, ILogger<TraineeService> logger, IRedisService cache)
        {
            _context = context;
            _logger = logger;
            _cache = cache;
            
        }
        public TraineeResponse MaptoDTO(TraineeModel trainee)
        {
            return new TraineeResponse
            {
                FirstName = trainee.FirstName,
                LastName = trainee.LastName,
                Id = trainee.Id,
                Email = trainee.Email,
                Status = trainee.Status,
                TechStack = trainee.TechStack,
                CreatedDate = trainee.CreatedDate,
                UpdatedDate = trainee.UpdatedDate
            };
        }

        public async Task<TraineeResponse> AddANewTrainee(CreateTraineeRequest request)
        {
                var newtrainee = new TraineeModel
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    TechStack = request.TechStack,
                    Status = request.Status,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                };

                await _context.Trainees.AddAsync(newtrainee);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Trainee added successfully! Id: {Id}", newtrainee.Id);
                return MaptoDTO(newtrainee);

        }

        public async Task<bool> DeleteTrainee(int id)
        {
                await _cache.RemoveAsync($"trainee:{id}");

                var trainee = await _context.Trainees.FindAsync(id);

                if (trainee == null)
                {
                    throw new KeyNotFoundException("Trainee not found in the database");
                }

                _context.Trainees.Remove(trainee);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Trainee Deleted successfully! Id: {Id}", id);

                return true;
        }

        public async Task<TraineeResponse?> EditTrainee(int id, UpdateTraineeRequest request)
        {
                await _cache.RemoveAsync($"trainee:{id}");
                var trainee = await _context.Trainees.FindAsync(id);

                if (trainee == null)
                {
                    throw new KeyNotFoundException("Trainee not found in the database");
                }

                trainee.FirstName = request.FirstName;
                trainee.LastName = request.LastName;
                trainee.Status = request.Status;
                trainee.TechStack = request.TechStack;
                trainee.Email = request.Email;
                trainee.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Trainee Edited successfully! Id: {Id}", id);

                return MaptoDTO(trainee);
        }

        public async Task<(List<TraineeResponse>?, int)> GetAllTrainees(int? pageNumber, int? pageSize, string? search, TraineeStatus? status)
        {
                var _trainees = await _context.Trainees.ToListAsync();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    search = search.ToLower();
                    _trainees = _trainees.Where(
                        t => t.FirstName.ToLower().Contains(search) ||
                        t.LastName.ToLower().Contains(search) ||
                        t.Email.ToLower().Contains(search) ||
                        t.TechStack.ToLower().Contains(search)).ToList();
                }
                if (status != null)
                    _trainees = _trainees.Where(t => t.Status == status).ToList();


                if (_trainees.Count() == 0 && (!string.IsNullOrWhiteSpace(search) || status != null))
                {
                    throw new KeyNotFoundException("TNo trainees found");

                }

                return (_trainees.Skip(((pageNumber ?? 1) - 1) * (pageSize ?? 10)).Take(pageSize ?? 10).Select(MaptoDTO).ToList(), _trainees.Count);

        }

        public async Task<TraineeResponse?> GetById(int id)
        {
                var data = await _cache.GetAsync<TraineeResponse>($"trainee:{id}");
                if (data != null)
                {
                    return data;
                }

                var trainee = await _context.Trainees.AsNoTracking().FirstOrDefaultAsync(t=>t.Id==id);

                if (trainee == null)
                {
                    throw new KeyNotFoundException("Trainee not found in the database");
                }
                else
                {
                    var resp = MaptoDTO(trainee);
                    await _cache.SetAsync(resp, $"trainee:{id}");
                    _logger.LogInformation("Retrieved Trainee with id: {id}", id);
                    return resp;
                }
        }
    }

}