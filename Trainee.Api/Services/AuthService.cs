using Microsoft.AspNetCore.Identity;
using Trainee.Api.DTO;
using Trainee.Api.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

using Trainee.Api.Data;
namespace Trainee.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthService> _logger;
        public AuthService(AppDbContext context, IJwtService jwtService, ILogger<AuthService> logger)
        {
            _context = context;
            _jwtService = jwtService;
            _logger = logger;
        }

        // public async Task<LoginResponse?> Login(LoginRequest request)
        // {
        //         var hasher = new PasswordHasher<string>();

        //         var user = await _context.Users.FirstOrDefaultAsync(t => t.Username == request.Username);
        //         if (user == null)
        //             throw new UnauthorizedAccessException("Failed authentication");

        //         var result = hasher.VerifyHashedPassword(request.Username, user.PasswordHash, request.Password);
        //         Console.WriteLine(result == PasswordVerificationResult.Success ? "Valid" : "Invalid");
        //         if (result == PasswordVerificationResult.Failed)
        //         {
        //             throw new UnauthorizedAccessException($"Failed Login For User: {request.Username}");
        //         }
        //         var token = _jwtService.GenerateToken(user);
        //         _logger.LogInformation("Token generated succesfully for {Username}", request.Username);

        //         return new LoginResponse
        //         {
        //             Token = token,
        //             ExpiresIn = 3600,
        //             User = new UserResponse
        //             {
        //                 Id = user.Id,
        //                 Username = user.Username,
        //                 Role = user.Role
        //             }
        //         };
        // }
          public async Task<LoginResponse?> Login(LoginRequest request)
        {

                // Console.WriteLine(BCrypt.Net.BCrypt.HashPassword(request.Password));
                var user = await _context.Users.FirstOrDefaultAsync(t => t.Username == request.Username);
                if (user == null)
                    throw new UnauthorizedAccessException("Failed authentication");

                bool isValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
                // bool isValid = BCrypt.Net.BCrypt.Verify(request.Password, "$2a$11$FqNM7CGqeNqTTmYr9RFUUu2T4ukh.Zv94NbXCzx8kwikVuSws8hki");
                // Console.WriteLine("Passwords is ");
                // Console.WriteLine(isValid);
                if(!isValid){
                    throw new UnauthorizedAccessException($"Failed Login For User: {request.Username}");
                }

                var token = _jwtService.GenerateToken(user);
                _logger.LogInformation("Token generated succesfully for {Username}", request.Username);

                return new LoginResponse
                {
                    Token = token,
                    ExpiresIn = 3600,
                    User = new UserResponse
                    {
                        Id = user.Id,
                        Username = user.Username,
                        Role = user.Role
                    }
                };
        }

    }

}