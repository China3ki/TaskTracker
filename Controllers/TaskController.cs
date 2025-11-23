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
        public async Task<ActionResult<IEnumerable<Tasks>>> GetTasks()
        {
            logger.LogInformation("Get all tasks");

            int id = Convert.ToInt32(User.FindFirstValue("id"));
            if (id == 0) return Unauthorized(new { message = "Id is not set!" });

            var tasks = await ctx.TaskUsers.Where(t => t.UserId == id).Select(t => new {
                t.TaskId,
                t.Task.TaskName,
                t.Task.TaskDescription,
                t.Task.TaskStart,
                t.Task.TaskEnd,
                t.Task.TaskStatus.StatusName,
                Users = t.Task.TaskUsers.Where(u => u.TaskId == t.TaskId).Select(u => new { u.User.UserName, u.User.UserSurname, u.TaskAdmin }).ToList()
            }).ToListAsync();


            if (tasks.Count == 0) return NotFound(new { message = "Tasks not found!" });
            return Ok(tasks);
        }
        [HttpGet("{taskId}")]
        [Authorize]
        public async Task<ActionResult<Tasks>> GetTask([Required] int taskId)
        {
            logger.LogInformation("Get task by id proccess");
            int id = Convert.ToInt32(User.FindFirstValue("id"));
            if (id == 0) return Unauthorized(new { message = "Id is not set!" });

            var task = await ctx.TaskUsers.Where(t => t.UserId == id && t.TaskId == taskId).Select(t => new
            {
                t.TaskId,
                t.Task.TaskName,
                t.Task.TaskDescription,
                t.Task.TaskStart,
                t.Task.TaskEnd,
                t.Task.TaskStatus.StatusName,
                Users = t.Task.TaskUsers.Where(u => u.TaskId == t.TaskId).Select(u => new { u.User.UserName, u.User.UserSurname, u.TaskAdmin }).ToList()
            }).FirstOrDefaultAsync();
            if(task is null) return BadRequest(new { message = "Task does not exist or you don't have a permission!" });
            return Ok(task);
        }
        [HttpPost("add/task")]
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
                TaskStatusId = task.StatusId is null ? 1 : (int)task.StatusId,
                TaskUsers = 
                [
                    new TaskUser()
                    {
                     UserId = id,
                     TaskAdmin = true
                    }
                ]
            };

            ctx.Add<Tasks>(newTask);
            await ctx.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTask), new { taskId = newTask.TaskId }, new
            {
                newTask.TaskId,
                newTask.TaskName,
                newTask.TaskDescription,
                newTask.TaskStart,
                newTask.TaskEnd,
                newTask.TaskStatusId
            });
        }
        [HttpPost("add/user")]
        [Authorize]
        public async Task<ActionResult> AddUserToTask([Required] AddUserTaskDto user)
        {
            logger.LogInformation("Add user to task process");
            int id = Convert.ToInt32(User.FindFirstValue("id"));
            if (id == 0) return Unauthorized(new { message = "Id is not set!" });

            bool checkUserIsAdmin = await ctx.TaskUsers.AnyAsync(t => t.UserId == id && t.TaskId == user.TaskId && t.TaskAdmin == true);
            if (!checkUserIsAdmin) return Unauthorized(new { message = "You do not have permission to add user!" });
            bool checkUserExist = await ctx.Users.AnyAsync(u => u.UserId == user.UserId);
            if (!checkUserExist) return BadRequest(new { message = "User does not exist!" });
            bool checkUserIsInTheTask = await ctx.TaskUsers.AnyAsync(t => t.UserId == user.UserId && t.TaskId == user.TaskId);
            if (checkUserIsInTheTask) return BadRequest(new { message = "User already is in the task!" });

            TaskUser task = new()
            {
                TaskId = user.TaskId,
                UserId = user.UserId,
                TaskAdmin = user.Admin
            };
            ctx.Add<TaskUser>(task);
            await ctx.SaveChangesAsync();
            return Created();
        }
        [HttpPut("admin")]
        [Authorize]
        public async Task<ActionResult> HandleAdminPermission([Required] int userId, [Required] int taskId, [Required] bool admin)
        {
            logger.LogInformation("Handle admin permission proccess");
            int id = Convert.ToInt32(User.FindFirstValue("id"));
            if (id == 0) return Unauthorized(new { message = "Id is not set!" });

            bool checkAdmin = await ctx.TaskUsers.AnyAsync(t => t.UserId == id && t.TaskId == taskId && t.TaskAdmin == true);
            if (!checkAdmin) return Unauthorized(new { message = "You do not have a permission to add admin" });

            var taskUser = await ctx.TaskUsers.FirstOrDefaultAsync(t => t.UserId == userId && t.TaskId == taskId);
            if (taskUser is null) return BadRequest(new { message = "User or task does not exist!"});
            if (taskUser.TaskAdmin && admin) return BadRequest(new { message = "User is already admin!" });
            if (!taskUser.TaskAdmin && !admin) return BadRequest(new { message = "User already do not have an admin!" });
            taskUser.TaskAdmin = admin;
            await ctx.SaveChangesAsync();
            return Created();
        }
        [HttpDelete("leave/{taskId}")]
        [Authorize]
        public async Task<ActionResult> LeaveTask([Required] int taskId)
        {
            logger.LogInformation("Leave task proccess");
            int id = Convert.ToInt32(User.FindFirstValue("id"));
            if (id == 0) return Unauthorized(new { message = "Id is not set!" });

            var taskUser = await ctx.TaskUsers.FirstOrDefaultAsync(t => t.UserId == id && t.TaskId == taskId);
            if (taskUser is null) return BadRequest(new { message = "You are not already in this task" });

            ctx.Remove<TaskUser>(taskUser);

            int countUser = await ctx.TaskUsers.CountAsync(t => t.TaskId == taskId);
            if(countUser == 1)
            {
                var task = await ctx.Tasks.FirstOrDefaultAsync(t => t.TaskId == taskId);
                if (task is null) return BadRequest(new { message = "Task does not exist!" });
                ctx.Remove<Tasks>(task);
            } else
            {
                var checkIsAnotherAdminInTask = await ctx.TaskUsers.AnyAsync(t => t.TaskAdmin == true && t.UserId != id && t.TaskId == taskId);
                if (!checkIsAnotherAdminInTask) return BadRequest( new { message = "You have to give someone admin permission to leave!" });
            }

            await ctx.SaveChangesAsync();
            return NoContent();

        }
        [HttpDelete("delete/user")]
        [Authorize]
        public async Task<ActionResult> DeleteUser([Required] int userId, [Required] int taskId)
        {
            logger.LogInformation("Delete user process");
            int id = Convert.ToInt32(User.FindFirstValue("id"));
            if (id == 0) return Unauthorized(new { message = "Id is not set!" });

            bool checkIsUserAdmin = await ctx.TaskUsers.AnyAsync(t => t.UserId == id && t.TaskId == taskId && t.TaskAdmin == true);
            if (!checkIsUserAdmin) return Unauthorized(new { message = "You do not have permission to delete user!" });
            if (id == userId) return BadRequest(new { message = "You cannot delete yourself!" });
            var taskUser = await ctx.TaskUsers.FirstOrDefaultAsync(t => t.UserId == userId && t.TaskId == taskId);
            if (taskUser is null) return BadRequest(new { message = "User in this task does not exist!" });

            ctx.Remove<TaskUser>(taskUser);
            await ctx.SaveChangesAsync();
            logger.LogInformation("User has been deleted!");

            return NoContent();
        }

        [HttpDelete("delete/task")]
        [Authorize]
        public async Task<ActionResult> DeleteTask([Required] int taskId)
        {
            logger.LogInformation("Remove task proccess");
            int id = Convert.ToInt32(User.FindFirstValue("id"));
            if (id == 0) return Unauthorized(new { message = "Id is not set!" });

            var task = await ctx.TaskUsers.FirstOrDefaultAsync(t => t.TaskId == taskId && t.UserId == id && t.TaskAdmin);
            if (task is null) return BadRequest(new { message = "Task does not exist or you don't have a permission!" });

            var taskUsers = await ctx.TaskUsers.Where(t => t.TaskId == task.TaskId).ToListAsync();
            ctx.TaskUsers.RemoveRange(taskUsers);

            ctx.Tasks.Remove(new Tasks { TaskId = taskId });
            await ctx.SaveChangesAsync();
            return NoContent();

        }
    }
}
