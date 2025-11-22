using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskTracker.Dto;
using TaskTracker.Entities;

namespace TaskTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController(TasktrackerContext ctx, ILogger<TaskController> logger) : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Task>>> GetTasks()
        {
            logger.LogInformation("Get all tasks");
            int id = Convert.ToInt32(User.FindFirstValue("id"));
            if (id == 0) return Unauthorized(new { message = "Id is not set!" });
            var tasks = await ctx.TaskUsers.Where(t => t.User.UserId == id).Select(t => new { t.TaskId, t.Task.TaskName, t.Task.TaskDescription, t.Task.TaskStart, t.Task.TaskEnd, t.Task.TaskStatus.StatusName, 
                Users = ctx.TaskUsers.Where(u => u.TaskId == t.TaskId).Select(u => new { u.User.UserName, u.User.UserSurname, u.TaskAdmin }).ToList() 
            }).ToListAsync();
            if (tasks is null) return NotFound(new { message = "Tasks not found!" });
            return Ok(tasks);
        }
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> AddTask([Required] TaskDto task)
        {
            logger.LogInformation("Add Task process");
            int id = Convert.ToInt32(User.FindFirstValue("id"));
            if (id == 0) return Unauthorized(new { message = "Id is not set!" });
            Tasks newTask = new()
            {
                TaskName = task.Name,
                TaskDescription = task.Description,
                TaskStart = task.StartDate ?? DateTime.Now,
                TaskEnd = task.EndDate,
                TaskStatusId = task.StatusId is null ? 1 : (int)task.StatusId
            };
            TaskUser newTaskUser = new()
            {
                UserId = id,
                TaskAdmin = true
            };
            ctx.Add<Tasks>(newTask);
            await ctx.SaveChangesAsync();
            newTaskUser.TaskId = newTask.TaskId;
            ctx.Add<TaskUser>(newTaskUser);
            await ctx.SaveChangesAsync();
            return Created();
        }
    }
}
