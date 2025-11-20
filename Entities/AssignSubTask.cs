using System;
using System.Collections.Generic;

namespace TaskTracker.Entities;

public partial class AssignSubTask
{
    public int AssignId { get; set; }

    public int? AssingUserId { get; set; }

    public int? AssignSubTaskId { get; set; }

    public virtual TasksSub? AssignSubTaskNavigation { get; set; }

    public virtual User? AssingUser { get; set; }
}
