using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using TaskTracker.Dto;
using TaskTracker.Entities;

namespace TaskTracker.Services
{
    public class AuthService(TasktrackerContext ctx)
    {
        public async Task<(bool, string)> Register(RegisterDto user)
        {
            (bool SuccessPasswordValidate, string Message) = ValidatePassword(user.Password, user.ConfirmedPassword);
            if (!SuccessPasswordValidate) return (false, Message);
            if (await ctx.Users.AnyAsync(u => u.UserEmail == user.Email)) return (false, "Account already Exist!");

            var hashedPassword = new PasswordHasher<RegisterDto>().HashPassword(user, user.Password);

            User newUser = new()
            {
                UserName = user.Name,
                UserSurname = user.Surname,
                UserEmail = user.Email,
                UserPassword = hashedPassword
            };
            ctx.Add<User>(newUser);
            await ctx.SaveChangesAsync();
            return (true, "Account has been created!");
        }
        public async Task<bool> Login(LoginDto user)
        {
            var findedUser = await ctx.Users.Select(u => new { u.UserEmail, u.UserPassword }).FirstOrDefaultAsync(u => u.UserEmail == user.Email);
            if (findedUser is null) return false;
            #pragma warning disable CS8604 // Możliwy argument odwołania o wartości null.
            if (new PasswordHasher<LoginDto>().VerifyHashedPassword(user, findedUser.UserPassword, user.Password) == PasswordVerificationResult.Failed) return false;
            return true;
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
