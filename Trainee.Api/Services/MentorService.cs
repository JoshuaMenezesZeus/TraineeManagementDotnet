using Microsoft.AspNetCore.Http.HttpResults;
using Trainee.Api.Data;
using Trainee.Api.DTO;
using Trainee.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Trainee.Api.Services
{
    public class MentorService : IMentorService
    {

        private readonly AppDbContext _context;
        private readonly ILogger<MentorService> _logger;
        public MentorService(AppDbContext context, ILogger<MentorService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public MentorResponse MaptoDTO(MentorModel mentor)
        {
            return new MentorResponse
            {
                FirstName = mentor.FirstName,
                LastName = mentor.LastName,
                Id = mentor.Id,
                Email = mentor.Email,
                Expertise = mentor.Expertise,
                Status = mentor.Status,
                CreatedDate = mentor.CreatedDate,
                UpdatedDate = mentor.UpdatedDate
            };
        }
        public async Task<MentorResponse> AddANewMentor(CreateMentorRequest request)
        {
                var newmentor = new MentorModel
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    Expertise = request.Expertise,
                    Status = request.Status,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                };

                await _context.Mentors.AddAsync(newmentor);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Mentor added successfully! Id: {Id}", newmentor.Id);
                return MaptoDTO(newmentor);
        }

        public async Task<bool> DeleteMentor(int id)
        {
                var mentor = await _context.Mentors.FindAsync(id);

                if (mentor == null)
                {
                    throw new KeyNotFoundException("Mentor not found in the database");
                }

                _context.Mentors.Remove(mentor);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Mentor Deleted successfully! Id: {Id}", id);

                return true;
        }

        public async Task<MentorResponse?> EditMentor(int id, UpdateMentorRequest request)
        {
                var mentor = await _context.Mentors.FindAsync(id);

                if (mentor == null)
                {
                    throw new KeyNotFoundException("Mentor not found in the database");
                }

                mentor.FirstName = request.FirstName;
                mentor.LastName = request.LastName;
                mentor.Status = request.Status;
                mentor.Expertise = request.Expertise;
                mentor.Email = request.Email;
                mentor.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Mentor Edited successfully! Id: {Id}", id);

                return MaptoDTO(mentor);
        }

        public async Task<(List<MentorResponse>?, int)> GetAllMentors(int? pageNumber, int? pageSize, string? search, MentorStatus? status)
        {
                var _mentors = await _context.Mentors.ToListAsync();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    search = search.ToLower();
                    _mentors = _mentors.Where(
                        t => t.FirstName.ToLower().Contains(search) ||
                        t.LastName.ToLower().Contains(search) ||
                        t.Email.ToLower().Contains(search) ||
                        t.Expertise.ToLower().Contains(search)).ToList();
                }
                if (status != null)
                    _mentors = _mentors.Where(t => t.Status == status).ToList();


                if (_mentors.Count() == 0 && (!string.IsNullOrWhiteSpace(search) || status != null))
                {
                    throw new KeyNotFoundException("No Mentor Found");
                }

                return (_mentors.Skip(((pageNumber ?? 1) - 1) * (pageSize ?? 10)).Take(pageSize ?? 10).Select(MaptoDTO).ToList(), _mentors.Count);
        }

        public async Task<MentorResponse?> GetMentorById(int id)
        {
            var mentor = await _context.Mentors.FindAsync(id);

            if (mentor == null)
            {
                throw new KeyNotFoundException("Mentor not found in the database");
            }
            else
            {
                _logger.LogInformation("Mentor retrieved successfully with id: {Id}", id);   
                return MaptoDTO(mentor);
            }
        }
    }
}