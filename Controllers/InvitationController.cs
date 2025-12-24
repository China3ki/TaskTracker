using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using TaskTracker.Dto;
using TaskTracker.Entities;
using TaskTracker.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TaskTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvitationController(TaskTrackerContext ctx, AuthService authService) : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<UsersInvitation>>> GetInvitation()
        {
            int id = Convert.ToInt32(User.FindFirstValue("id"));
            if (id == 0) return Unauthorized(new { message = "You do not have a permission!" });
            var invitation = await ctx.UsersInvitations.Where(i => i.InvitedUserId == id).Select(i => new { i.InvitationId, i.TaskId, i.UserId, i.User.UserName, i.User.UserSurname }).ToListAsync();
            if (invitation.Count == 0) return NoContent();
            else return Ok(invitation);
        }
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> SendInvitation([Required] TaskUserDto data)
        {
            (bool isAdminAndIdExist, int id) = await authService.CheckId(data.TaskId, this);
            if (!isAdminAndIdExist) return Unauthorized(new { message = "You do not have a permission!" });

            bool userExistInTheTask = await ctx.TaskUsers.AnyAsync(t => t.TaskId == data.TaskId && t.UserId == data.UserId);
            if (userExistInTheTask) return BadRequest(new { message = "User is already in the task!" });

            bool userExist = await ctx.Users.AnyAsync(u => u.UserId == data.UserId);
            if (!userExist) return BadRequest(new { message = "User does not exist!" });

            UsersInvitation invitation = new()
            {
                InvitedUserId = data.UserId,
                TaskId = data.TaskId,
                TaskAdmin = data.Admin,
                UserId = id
            };
            ctx.Add<UsersInvitation>(invitation);
            await ctx.SaveChangesAsync();
            return Created();
        }
        [HttpPost("{invitationId}")]
        [Authorize]
        public async Task<ActionResult> AcceptInvitation([Required] int invitationId)
        {
            int id = Convert.ToInt32(User.FindFirstValue("id"));
            if (id == 0) return Unauthorized(new { message = "You do not have a permission!" });

            var invitation = await ctx.UsersInvitations.FirstOrDefaultAsync(i => i.InvitationId == invitationId && i.InvitedUserId == id);
            if (invitation is null) return BadRequest(new { message = "Invitation does not exist!" });

            TaskUser taskUser = new()
            {
                TaskId = invitation.TaskId,
                UserId = id,
                TaskAdmin = invitation.TaskAdmin
            };
            ctx.Remove<UsersInvitation>(invitation);
            ctx.Add<TaskUser>(taskUser);
            await ctx.SaveChangesAsync();
            return Created();
        }
        [HttpDelete("{invitationId}")]
        [Authorize]
        public async Task<ActionResult> DeclineInvitation([Required] int invitationId)
        {
            int id = Convert.ToInt32(User.FindFirstValue("id"));
            if (id == 0) return Unauthorized(new { message = "You do not have a permission!" });

            var invitation = await ctx.UsersInvitations.FirstOrDefaultAsync(i => i.InvitationId == invitationId && (i.InvitedUserId == id || i.UserId == id));
            if (invitation is null) return BadRequest(new { message = "Invitation does not exist!" });

            ctx.Remove<UsersInvitation>(invitation);
            await ctx.SaveChangesAsync();
            return NoContent();
        }
    }
}
