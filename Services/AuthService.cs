using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TaskTracker.Dto;
using TaskTracker.Entities;
using TaskTracker.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TaskTracker.Services
{
    public class AuthService(TaskTrackerContext ctx, IPasswordHasher<User> hasher, IConfiguration configuration)
    {
        public async Task<(bool, string)> Register(RegisterDto data)
        {
            bool userExist = await ctx.Users.AnyAsync(u => u.UserEmail == data.Email);
            if (userExist) return (false, "A user with this email already exists!");
            (bool validPassword, string message) = PasswordValidation(data.Password, data.ConfirmedPassword);
            if (!validPassword) return (false, message);
            User user = new()
            {
                UserName = data.Name.Trim(),
                UserSurname = data.Surname.Trim(),
                UserEmail = data.Email.Trim(),
            };
            user.UserPassword = hasher.HashPassword(user, data.Password);
            ctx.Add<User>(user);
            await ctx.SaveChangesAsync();
            return (true, "Account has been created!");
        }
        public async Task<(bool, string)> Login(LoginDto data)
        {
            string message = "Account does not exist or provided password is wrong!";
            var user = await ctx.Users.FirstOrDefaultAsync(u => u.UserEmail == data.Email);
            if (user is null) return (false, message);
            var passwordEquality = hasher.VerifyHashedPassword(user, user.UserPassword, data.Password);
            if (passwordEquality == PasswordVerificationResult.Failed) return (false, message);
            return (true, GenerateToken(user.UserId));
        }

        private string GenerateToken(int id)
        {
            var keyBytes = Encoding.UTF8.GetBytes(configuration["jwtSecret"] ?? throw new InvalidOperationException("Api key does not exist!"));
            var key = new SymmetricSecurityKey(keyBytes);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            List<Claim> claims =
                [
                    new Claim(JwtRegisteredClaimNames.Sub, id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("id", id.ToString())
                ];
            var token = new JwtSecurityToken
                (
                    issuer: configuration["Jwt:Issuer"],
                    audience: configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(1),
                    signingCredentials: creds
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private (bool, string) PasswordValidation(string password, string confirmedPassword)
        {
            if (password != confirmedPassword) return (false, "Passwords are not the same!");
            if (!password.Any(char.IsUpper)) return (false, "Password must have atleast one uppercase!");
            if (!Regex.IsMatch(password, "[0-9]")) return (false, "Password must have atleast one number!");
            if (!Regex.IsMatch(password, "(?=.*?[#?!@$%^&*-])")) return (false, "Password must have atleast one special character!");
            return (true, string.Empty);
        }
    }
}
