using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using TaskTracker.Dto;
using TaskTracker.Entities;

namespace TaskTracker.Services
{
    public sealed class AuthService(TasktrackerContext ctx, IPasswordHasher<User> hasher, IConfiguration config)
    {
        public async Task<(bool, string)> Register(RegisterDto user)
        {
            (bool SuccessPasswordValidate, string Message) = ValidatePassword(user.Password, user.ConfirmedPassword);
            if (!SuccessPasswordValidate) return (false, Message);
            if (await ctx.Users.AnyAsync(u => u.UserEmail == user.Email)) return (false, "Account already Exist!");
            User newUser = new()
            {
                UserName = user.Name,
                UserSurname = user.Surname,
                UserEmail = user.Email,
            };
            newUser.UserPassword = hasher.HashPassword(newUser, user.Password);
            ctx.Add<User>(newUser);
            await ctx.SaveChangesAsync();
            return (true, "Account has been created!");
        }
        public async Task<(bool, string)> Login(LoginDto user)
        {
            var userFromDb = await ctx.Users.FirstOrDefaultAsync(u => u.UserEmail == user.Email);
            if (userFromDb is null || string.IsNullOrEmpty(userFromDb.UserPassword)) return (false, string.Empty);
            if (hasher.VerifyHashedPassword(userFromDb, userFromDb.UserPassword, user.Password) == PasswordVerificationResult.Failed) return (false, string.Empty);
            return (true, TokenProvider(userFromDb));
        }
        private string TokenProvider(User user)
        {
            var settings = config.GetSection("Jwt");

            var keyBytes = Encoding.UTF8.GetBytes(config["Jwt:Secret"] ?? throw new InvalidOperationException("Variable does not exist!"));
            var key = new SymmetricSecurityKey(keyBytes);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            List<Claim> claims =
                [
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                ];
            var token = new JwtSecurityToken(
                issuer: settings["Issuer"],
                audience: settings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private (bool, string) ValidatePassword(string password, string confirmedPassword)
        {
            if (!password.Any(char.IsUpper)) return (false, "Password must have atleast one uppercase!");
            if (!Regex.IsMatch(password, "[0-9]")) return (false, "Password must have atleast one number!");
            if (!Regex.IsMatch(password, "(?=.*?[#?!@$%^&*-])")) return (false, "Password must have atleast one special character!");
            if (password != confirmedPassword) return (false, "Passwords does not match!");
            return (true, string.Empty);
        }
    }
}
