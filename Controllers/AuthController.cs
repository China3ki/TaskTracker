using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TaskTracker.Dto;
using TaskTracker.Services;

namespace TaskTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(ILogger<AuthController> logger, AuthService authServices) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult> Register([Required] RegisterDto user)
        {
            if (!ModelState.IsValid) return BadRequest(new { message = "Data is not valid!" });
            logger.LogInformation("Register proccess");
            (bool Success, string Message) = await authServices.Register(user);
            if (Success) return Ok(new { message = Message });
            else return BadRequest(new { message = Message });
        }
        [HttpPost("login")]
        public async Task<ActionResult> Login([Required] LoginDto user)
        {
            logger.LogInformation("Login proccess");
            (bool success, string token) = await authServices.Login(user);
            if (success) return Ok(new { Token = token });
            else return BadRequest(new { message = "Account does not exist or wrong data" });
        }
    }
}
