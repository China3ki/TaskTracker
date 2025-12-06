using System;
using System.Collections.Generic;

namespace TaskTracker.Entities;

public partial class TaskComment
{
    public int CommentId { get; set; }

    public string CommentName { get; set; } = null!;

    public int CommentTaskId { get; set; }

    public int CommentUserId { get; set; }

    public virtual TaskMain CommentTask { get; set; } = null!;

    public virtual User CommentUser { get; set; } = null!;
}
