using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TaskTracker.Dto;
using TaskTracker.Entities;
using TaskTracker.Services;

namespace TaskTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(ILogger<AuthController> logger, AuthService authServices) : ControllerBase
    {
        private readonly ILogger _logger = logger;
        private readonly AuthService _authServices = authServices;
        [HttpPost("register")]
        public async Task<ActionResult> Register([Required] RegisterDto user)
        {
            _logger.LogInformation("Register proccess");
            (bool Success, string Message) = await _authServices.Register(user);
            if (Success) return Ok(new { message = Message });
            else return BadRequest(new { message = Message });
        }
        [HttpPost("login")]
        public async Task<ActionResult> Login([Required] LoginDto user)
        {
            _logger.LogInformation("Login proccess");
            bool success = await _authServices.Login(user);
            if (success) return Ok(new { message = "Succesful Login" });
            else return BadRequest(new { message = "Account does not exist or wrong data" });
        }
    }
}
