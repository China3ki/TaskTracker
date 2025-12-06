using System;
using System.Collections.Generic;

namespace TaskTracker.Entities;

public partial class TaskUser
{
    public int TaskUserId { get; set; }

    public int TaskId { get; set; }

    public int UserId { get; set; }

    public bool TaskAdmin { get; set; }

    public virtual Task Task { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
