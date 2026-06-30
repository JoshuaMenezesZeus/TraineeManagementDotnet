using Trainee.Api.DTO;

namespace Trainee.Api.Services
{
    public interface IAuthService
    {
            Task<LoginResponse?> Login(LoginRequest request);
    }
}