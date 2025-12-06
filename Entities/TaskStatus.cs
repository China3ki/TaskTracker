using System;
using System.Collections.Generic;

namespace TaskTracker.Entities;

public partial class TaskStatus
{
    public int StatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public virtual ICollection<TaskMain> Tasks { get; set; } = new List<TaskMain>();

    public virtual ICollection<TasksSub> TasksSubs { get; set; } = new List<TasksSub>();
}
