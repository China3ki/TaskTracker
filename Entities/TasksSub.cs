using System;
using System.Collections.Generic;

namespace TaskTracker.Entities;

public partial class TasksSub
{
    public int SubTaskId { get; set; }

    public int? TaskId { get; set; }

    public string? TaskName { get; set; }

    public string? TaskDescription { get; set; }

    public DateTime? TaskStart { get; set; }

    public DateTime? TaskEnd { get; set; }

    public int? TaskStatusId { get; set; }

    public virtual ICollection<AssignSubTask> AssignSubTasks { get; set; } = new List<AssignSubTask>();

    public virtual Task? Task { get; set; }

    public virtual TaskStatus? TaskStatus { get; set; }

    public virtual ICollection<TaskSubComment> TaskSubComments { get; set; } = new List<TaskSubComment>();
}
