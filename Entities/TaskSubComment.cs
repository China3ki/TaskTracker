using System;
using System.Collections.Generic;

namespace TaskTracker.Entities;

public partial class TaskSubComment
{
    public int CommentId { get; set; }

    public string CommentName { get; set; } = null!;

    public int CommentSubTaskId { get; set; }

    public int CommentUserId { get; set; }

    public virtual TasksSub CommentSubTask { get; set; } = null!;

    public virtual User CommentUser { get; set; } = null!;
}
