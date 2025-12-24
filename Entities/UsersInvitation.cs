using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TaskTracker.Entities;

public partial class UsersInvitation
{
    public int InvitationId { get; set; }

    public int InvitedUserId { get; set; }

    public int TaskId { get; set; }

    public bool TaskAdmin { get; set; }

    public int UserId { get; set; }
    public virtual User InvitedUser { get; set; } = null!;
    public virtual TaskMain Task { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}
