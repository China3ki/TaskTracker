using System;
using System.Collections.Generic;

namespace TaskTracker.Entities;

public partial class TaskMain
{
    public int TaskId { get; set; }

    public string? TaskDescription { get; set; }

    public DateTime TaskStart { get; set; }

    public DateTime? TaskEnd { get; set; }

    public int TaskStatusId { get; set; }

    public string? TaskName { get; set; }

    public virtual ICollection<TaskComment> TaskComments { get; set; } = new List<TaskComment>();

    public virtual TaskStatus TaskStatus { get; set; } = null!;

    public virtual ICollection<TaskUser> TaskUsers { get; set; } = new List<TaskUser>();

    public virtual ICollection<TasksSub> TasksSubs { get; set; } = new List<TasksSub>();
}
