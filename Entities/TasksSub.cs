using System;
using System.Collections.Generic;

namespace TaskTracker.Entities;

public partial class TasksSub
{
    public int SubTaskId { get; set; }

    public int TaskId { get; set; }

    public string TaskName { get; set; } = null!;

    public DateTime TaskStart { get; set; }

    public DateTime? TaskEnd { get; set; }

    public int TaskStatusId { get; set; }

    public string? TaskDescription { get; set; }

    public virtual ICollection<AssignSubTask> AssignSubTasks { get; set; } = new List<AssignSubTask>();

    public virtual TaskMain Task { get; set; } = null!;

    public virtual TaskStatus TaskStatus { get; set; } = null!;

    public virtual ICollection<TaskSubComment> TaskSubComments { get; set; } = new List<TaskSubComment>();
}
