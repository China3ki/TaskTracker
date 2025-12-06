using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TaskTracker.Dto;
using TaskTracker.Entities;
using TaskTracker.Models;
using TaskTracker.Services;

namespace TaskTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(AuthService authService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult> Register([Required] RegisterDto data)
        {
            (bool success, string message) = await authService.Register(data);
            if (!success) return BadRequest(new { message });
            else return Ok( new { message });
        }
        [HttpPost("login")]
        public async Task<ActionResult> Login([Required] LoginDto data)
        {
            (bool success, string message) = await authService.Login(data);
            if (!success) return BadRequest(new { message });
            else return Ok(new { token = message });
        }
    }
}
