using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Trainee.Api.Data;
using Trainee.Api.Models;
using BCrypt.Net;

namespace Trainee.Api.Settings;
 
public static class DbSeeder
{
    public static async Task SeedAdminUserAsync(AppDbContext context)
    {
        if (await context.Users.AnyAsync(u => u.Username == "admin2"))
        {
            return;
        }
 
        var passwordHasher = new PasswordHasher<UserModel>();
 
        var adminUser = new UserModel
        {
            Username = "admin2",
            Email = "admin@test.com",
            Role = UserRole.Admin,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow
        };
 
        adminUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin2");
 
        await context.Users.AddAsync(adminUser);
        await context.SaveChangesAsync();
    }
}