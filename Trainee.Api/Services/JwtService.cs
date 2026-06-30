using System.Diagnostics.Contracts;
using Microsoft.AspNetCore.Mvc;
using Trainee.Api.Data;
using Trainee.Api.DTO;
using Trainee.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using Trainee.Api.Settings;
 
namespace Trainee.Api.Services;
 
public class JwtService : IJwtService
{
    private readonly JwtSettings _settings;
 
    public JwtService(IOptions<JwtSettings> options)
    {
        _settings = options.Value;
    }
 
    public string GenerateToken(UserModel user)
    {
 
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),  
            new Claim(ClaimTypes.Name, user.Username),  
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };
 
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
 
        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_settings.ExpiresIn),
            signingCredentials: creds
        );
 
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}