using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using TaskTracker.Dto;
using TaskTracker.Entities;

namespace TaskTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController(TaskTrackerContext ctx) : ControllerBase
    {
        [HttpGet("task/get")]
        [Authorize]
        public async Task<ActionResult<List<TaskMain>>> GetTasks()
        {
            int id = Convert.ToInt32(User.FindFirstValue("id"));
            if (id == 0) return Unauthorized("You do not have a permission!");

            var tasks = await ctx.TaskUsers.Where(u => u.UserId == id).Select(t => new
            {
                t.TaskId,
                t.Task.TaskName,
                t.Task.TaskDescription,
                t.Task.TaskStart,
                t.Task.TaskEnd,
                t.Task.TaskStatus.StatusName,
                Users = t.User.TaskUsers.Where(u => u.TaskId == t.TaskId).Select(u => new { u.User.UserName, u.User.UserSurname, u.TaskAdmin }).ToList()
            }).ToListAsync();
            if (tasks.Count == 0) return BadRequest("You do not have any tasks!");
            else return Ok(tasks);
        }
        [HttpPost("task/add")]
        //[Authorize]
        public async Task<ActionResult> AddTask(TaskDto data)
        {
            int id = Convert.ToInt32(User.FindFirstValue("id"));
            if (id == 0) return Unauthorized(new { message = "You do not have a permission!" });

            TaskMain task = new()
            {
                TaskName = data.Name,
                TaskDescription = data.Description,
                TaskStart = data.Start ?? DateTime.Now,
                TaskEnd = data.End,
                TaskStatusId = 1,
                TaskUsers =
                [
                    new TaskUser { UserId = id, TaskAdmin = true }
                ]
            };
            ctx.Add<TaskMain>(task);
            await ctx.SaveChangesAsync();
            return Created();
        }
        [HttpPost("user/add")]
        //[Authorize]
        public async Task<ActionResult> AddUser([Required] TaskUserDto data)
        {
            int id = Convert.ToInt32(User.FindFirstValue("id"));
            if (id == 0) return Unauthorized(new { message = "You do not have a permission!" });

            bool isAdmin = await ctx.TaskUsers.AnyAsync(t => t.TaskId == data.TaskId && t.UserId == id && t.TaskAdmin == true);
            if (!isAdmin) return Unauthorized(new { message = "You do not have a permission!" });

            bool userExistInTheTask = await ctx.TaskUsers.AnyAsync(t => t.TaskId == data.TaskId && t.UserId == data.UserId);
            if (userExistInTheTask) return BadRequest(new { message = "User is already in the task!" });
 
            bool userExist = await ctx.Users.AnyAsync(u => u.UserId == data.UserId);
            if (!userExist) return BadRequest(new { message = "User does not exist!" });

            TaskUser taskUser = new()
            {
                TaskId = data.TaskId,
                UserId = data.UserId,
                TaskAdmin = data.Admin,
            };
            ctx.Add<TaskUser>(taskUser);
            await ctx.SaveChangesAsync();
            return Created();
        }

    }
}
