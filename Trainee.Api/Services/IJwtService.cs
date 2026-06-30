using Trainee.Api.DTO;
using Trainee.Api.Models;
namespace Trainee.Api.Services
{
    public interface IJwtService
    {
        string GenerateToken(UserModel user);
    }
}