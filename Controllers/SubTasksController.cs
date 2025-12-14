using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using TaskTracker.Dto;
using TaskTracker.Entities;
using TaskTracker.Services;

namespace TaskTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubTasksController(TaskTrackerContext ctx, AuthService authService) : ControllerBase
    {
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> AddSubTask([Required] int taskId, [Required] TaskDto task)
        {
            (bool isAdminAndIdExist, int id) = await authService.CheckId(taskId, this);
            if (!isAdminAndIdExist) return Unauthorized("You do not have a permission!");

            TasksSub subTask = new()
            {
                TaskId = taskId,
                TaskName = task.Name,
                TaskStart = task.Start ?? DateTime.Now,
                TaskEnd = task.End,
                TaskStatusId = 1
            };
            ctx.Add<TasksSub>(subTask);
            await ctx.SaveChangesAsync();
            return Created();
        }
        [HttpPost("comment")]
        [Authorize]
        public async Task<ActionResult> AddComment([Required] int subTaskId, [Required] CommentDto data)
        {
            int id = Convert.ToInt32(User.FindFirstValue("id"));
            if(id == 0) return Unauthorized("You do not have a permission!");

            bool userExist = await ctx.TaskUsers.AnyAsync(u => u.TaskId == data.TaskId && u.UserId == id);
            if (!userExist) return BadRequest(new { message = "You do not have a permission!" });
            bool subTaskExist = await ctx.TasksSubs.AnyAsync(t => t.SubTaskId == subTaskId);
            if (!subTaskExist) return BadRequest(new { message = "Sub task does not exist!" });
            TaskSubComment comment = new()
            {
                CommentName = data.Text,
                CommentSubTaskId = subTaskId,
                CommentUserId = id
            };
            ctx.Add<TaskSubComment>(comment);
            await ctx.SaveChangesAsync();
            return Created();
        }
        [HttpPost("assign")]
        [Authorize]
        public async Task<ActionResult> AssignUserToSubTask([Required] int taskId, [Required] int subTaskId, [Required] int userId)
        {
            (bool isAdminAndIdExist, int id) = await authService.CheckId(taskId, this);
            if (!isAdminAndIdExist) return Unauthorized("You do not have a permission!");

            bool userExist = await ctx.TaskUsers.AnyAsync(u => u.TaskId == taskId && u.UserId == userId);
            if (!userExist) return BadRequest(new { message = "User does not exist in this task!" });

            bool subTaskExist = await ctx.TasksSubs.AnyAsync(t => t.SubTaskId == subTaskId);
            if (!subTaskExist) return BadRequest(new { message = "Sub task does not exist!" });

            bool UserIsAlreadyInSubTask = await ctx.AssignSubTasks.AnyAsync(t => t.AssignSubTaskId == subTaskId && t.AssignUserId == userId);
            if (UserIsAlreadyInSubTask) return BadRequest(new { message = "User is already in this sub task!" });

            AssignSubTask newAssign = new()
            {
                AssignUserId = userId,
                AssignSubTaskId = subTaskId
            };
            ctx.Add<AssignSubTask>(newAssign);
            await ctx.SaveChangesAsync();
            return Created();
        }
        [HttpDelete("assign")]
        [Authorize]
        public async Task<ActionResult> UnassignUser([Required] int taskId, [Required] int subTaskId, [Required] int userId)
        {
            (bool isAdminAndIdExist, int id) = await authService.CheckId(taskId, this);
            if (!isAdminAndIdExist) return Unauthorized("You do not have a permission!");


            var assign = await ctx.AssignSubTasks.FirstOrDefaultAsync(t => t.AssignSubTaskId == subTaskId && t.AssignUserId == userId);
            if (assign is null) return BadRequest(new { message = "Sub task does not exist!" });

            bool userIsInTnSubTask = await ctx.AssignSubTasks.AnyAsync(t => t.AssignSubTaskId == subTaskId && t.AssignUserId == userId);
            if (!userIsInTnSubTask) return BadRequest(new { message = "User already is not in this task!" });
            ctx.Remove<AssignSubTask>(assign);

            await ctx.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete]
        [Authorize]
        public async Task<ActionResult> DeleteSubTask([Required] int taskId, [Required] int subTaskid)
        {
            (bool isAdminAndIdExist, int id) = await authService.CheckId(taskId, this);
            if (!isAdminAndIdExist) return Unauthorized("You do not have a permission!");

            var subTask = await ctx.TasksSubs.FirstOrDefaultAsync(t => t.SubTaskId == subTaskid);
            if(subTask is null) return BadRequest(new { message = "Sub task does not exist!" });

            var assigned = await ctx.AssignSubTasks.Where(t => t.AssignSubTaskId == subTaskid).ToListAsync();
            var comments = await ctx.TaskSubComments.Where(c => c.CommentSubTaskId == subTaskid).ToListAsync();
            if (assigned.Count > 0) ctx.AssignSubTasks.RemoveRange(assigned);
            if (comments.Count > 0) ctx.TaskSubComments.RemoveRange(comments);
            ctx.Remove<TasksSub>(subTask);
            await ctx.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("comments")]
        [Authorize]
        public async Task<ActionResult> DeleteComment([Required] int taskId, [Required] int commentId)
        {
            int id = Convert.ToInt32(User.FindFirstValue("id"));
            if (id == 0) return Unauthorized("You do not have a permission!");

            bool creator = await ctx.TaskSubComments.AnyAsync(c => c.CommentUserId == id);
            bool admin = await ctx.TaskUsers.AnyAsync(c => c.TaskId == taskId && c.UserId == id && c.TaskAdmin == true);
            if(!creator && !admin) return Unauthorized("You do not have a permission!");

            var comment = await ctx.TaskSubComments.FirstOrDefaultAsync(c => c.CommentId == commentId);
            if (comment is null) return BadRequest(new { message = "Comment does not exist!" });
            ctx.Remove<TaskSubComment>(comment);
            await ctx.SaveChangesAsync();
            return NoContent();
        }
    }
}
