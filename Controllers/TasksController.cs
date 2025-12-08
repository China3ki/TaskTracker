using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskTracker.Dto;
using TaskTracker.Entities;
using TaskTracker.Services;

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
            (bool idExist, int id) = AuthService.CheckId(this);
            if (!idExist) return Unauthorized("You do not have a permission!");

            var tasks = await ctx.TaskUsers.Where(u => u.UserId == id).Select(t => new
            {
                t.TaskId,
                t.Task.TaskName,
                t.Task.TaskDescription,
                t.Task.TaskStart,
                t.Task.TaskEnd,
                t.Task.TaskStatus.StatusName,
                Users = t.Task.TaskUsers.Where(u => u.TaskId == t.TaskId).Select(u => new {u.User.UserId, u.User.UserName, u.User.UserSurname, u.TaskAdmin }).ToList()
            }).ToListAsync();
            if (tasks.Count == 0) return BadRequest("You do not have any tasks!");
            else return Ok(tasks);
        }
        [HttpPost("task/add")]
        [Authorize]
        public async Task<ActionResult> AddTask(TaskDto data)
        {
            (bool idExist, int id) = AuthService.CheckId(this);
            if (!idExist) return Unauthorized("You do not have a permission!");

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
        [Authorize]
        public async Task<ActionResult> AddUser([Required] TaskUserDto data)
        {
            (bool idExist, int id) = AuthService.CheckId(this);
            if (!idExist) return Unauthorized("You do not have a permission!");

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
        [HttpDelete("user/delete")]
        [Authorize]
        public async Task<ActionResult> DeleteUser([Required] int taskId, [Required] int userId)
        {
            (bool idExist, int id) = AuthService.CheckId(this);
            if (!idExist) return Unauthorized("You do not have a permission!");

            bool admin = await ctx.TaskUsers.AnyAsync(u => u.TaskId == taskId && u.UserId == id && u.TaskAdmin == true);
            if (!admin) return BadRequest(new { message = "You do not have a permission!" });
            var user = await ctx.TaskUsers.FirstOrDefaultAsync(u => u.TaskId == taskId && u.UserId == userId);
            if (user is null) return BadRequest(new { message = "User does not exist in this task!" });
            if (id == userId) return BadRequest(new { message = "You cannot delete yourself! Use /task/leave" });
            ctx.Remove<TaskUser>(user);
            await ctx.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("task/delete")]
        [Authorize]
        public async Task<ActionResult> DeleteTask([Required] int taskId)
        {
            (bool idExist, int id) = AuthService.CheckId(this);
            if (!idExist) return Unauthorized("You do not have a permission!");
            bool admin = await ctx.TaskUsers.AnyAsync(u => u.TaskId == taskId && u.UserId == id && u.TaskAdmin == true);
            if (!admin) return BadRequest(new { message = "You do not have a permission!" });

            var task = await ctx.Tasks.FirstOrDefaultAsync(t => t.TaskId == taskId);
            if (task is null) return BadRequest(new { message = "Task does not exist!" });
            var taskUsers = await ctx.TaskUsers.Where(t => t.TaskId == taskId).ToListAsync();
            ctx.TaskUsers.RemoveRange(taskUsers);
            ctx.Remove<TaskMain>(task);
            await ctx.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("task/leave")]
        [Authorize]
        public async Task<ActionResult> LeaveTask([Required] int taskId)
        {
            (bool idExist, int id) = AuthService.CheckId(this);
            if (!idExist) return Unauthorized("You do not have a permission!");

            var countTaskUser = await ctx.TaskUsers.CountAsync(u => u.TaskId == taskId);
            bool userExist = await ctx.TaskUsers.AnyAsync(u => u.TaskId == taskId && u.UserId == id);
            bool admin = await ctx.TaskUsers.AnyAsync(u => u.TaskId == taskId && u.UserId != id && u.TaskAdmin == true);
            if (!userExist) return BadRequest(new { message = "You are not already in this task!" });
            if (!admin && countTaskUser > 1) return BadRequest(new { message = "You have to give someone admin permission before leave!" });
            else if(countTaskUser == 1)
            {
                var task = await ctx.Tasks.FirstOrDefaultAsync(t => t.TaskId == taskId);
                if (task is null) return BadRequest(new { message = "Task already not exist!" });
                ctx.Remove<TaskMain>(task);
            }
            var taskUser = await ctx.TaskUsers.FirstOrDefaultAsync(t => t.TaskId == taskId && t.UserId == id);
            if (taskUser is null) return BadRequest(new { message = "Task already not exist!" });
            ctx.Remove<TaskUser>(taskUser);
            await ctx.SaveChangesAsync();
            return NoContent();
        }
        [HttpPut("task/edit")]
        [Authorize]
        public async Task<ActionResult> EditTask([Required] int taskId, [Required] EditTaskDto task)
        {
            (bool idExist, int id) = AuthService.CheckId(this);
            if (!idExist) return Unauthorized("You do not have a permission!");

            bool admin = await ctx.TaskUsers.AnyAsync(t => t.TaskId == taskId && t.UserId == id && t.TaskAdmin == true);
            if (!admin) return Unauthorized(new { message = "You do not have a permission!" });

            var editedTask = await ctx.Tasks.FirstOrDefaultAsync(t => t.TaskId == taskId);
            if (editedTask is null) return BadRequest(new { message = "Task does not exist!" });
            editedTask.TaskName = task.Name ?? editedTask.TaskName;
            editedTask.TaskDescription = task.Description ?? editedTask.TaskDescription;
            editedTask.TaskStart = task.Start ?? editedTask.TaskStart;
            editedTask.TaskEnd = task.End ?? editedTask.TaskEnd;
            await ctx.SaveChangesAsync();
            return NoContent();
        }
        [HttpPut("user/edit")]
        [Authorize]
        public async Task<ActionResult> EditUser([Required]int taskId, [Required] int userId, [Required] bool admin)
        {
            (bool idExist, int id) = AuthService.CheckId(this);
            if (!idExist) return Unauthorized("You do not have a permission!");
            bool adminPermission = await ctx.TaskUsers.AnyAsync(u => u.TaskId == taskId && u.UserId == id && u.TaskAdmin == true);
            if(!adminPermission) return Unauthorized(new { message = "You do not have a permission!" });
            if(userId == id)
            {
                bool anotherAdminExist = await ctx.TaskUsers.AnyAsync(u => u.TaskId == taskId && u.UserId != id && u.TaskAdmin == true);
                if (!anotherAdminExist) return BadRequest(new { message = "You have to give someone admin permission before this action!" });
            }

            var taskUser = await ctx.TaskUsers.FirstOrDefaultAsync(t => t.TaskId == taskId && t.UserId == userId);
            if (taskUser is null) return BadRequest(new { message = "User does not exist!" });
            taskUser.TaskAdmin = admin;
            await ctx.SaveChangesAsync();
            return NoContent();
        }
    }
}
