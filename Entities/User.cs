using System;
using System.Collections.Generic;

namespace TaskTracker.Entities;

public partial class User
{
    public int UserId { get; set; }

    public string? UserName { get; set; }

    public string? UserSurname { get; set; }

    public string? UserEmail { get; set; }

    public string? UserPassword { get; set; }

    public virtual ICollection<AssignSubTask> AssignSubTasks { get; set; } = new List<AssignSubTask>();

    public virtual ICollection<TaskComment> TaskComments { get; set; } = new List<TaskComment>();

    public virtual ICollection<TaskSubComment> TaskSubComments { get; set; } = new List<TaskSubComment>();

    public virtual ICollection<TaskUser> TaskUsers { get; set; } = new List<TaskUser>();
}
