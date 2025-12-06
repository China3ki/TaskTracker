using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TaskTracker.Entities;

public partial class TaskTrackerContext : DbContext
{
    public TaskTrackerContext()
    {
    }

    public TaskTrackerContext(DbContextOptions<TaskTrackerContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AssignSubTask> AssignSubTasks { get; set; }

    public virtual DbSet<Task> Tasks { get; set; }

    public virtual DbSet<TaskComment> TaskComments { get; set; }

    public virtual DbSet<TaskStatus> TaskStatuses { get; set; }

    public virtual DbSet<TaskSubComment> TaskSubComments { get; set; }

    public virtual DbSet<TaskUser> TaskUsers { get; set; }

    public virtual DbSet<TasksSub> TasksSubs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=127.0.0.1:5432;Username=postgres;Password=2004Pawel!;Database=taskTracker");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AssignSubTask>(entity =>
        {
            entity.HasKey(e => e.AssignId).HasName("assign_sub_task_pkey");

            entity.ToTable("assign_sub_task");

            entity.Property(e => e.AssignId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("assign_id");
            entity.Property(e => e.AssignSubTaskId).HasColumnName("assign_sub_task_id");
            entity.Property(e => e.AssignUserId).HasColumnName("assign_user_id");

            entity.HasOne(d => d.AssignSubTaskNavigation).WithMany(p => p.AssignSubTasks)
                .HasForeignKey(d => d.AssignSubTaskId)
                .HasConstraintName("fk_assign_sub_task_id");

            entity.HasOne(d => d.AssignUser).WithMany(p => p.AssignSubTasks)
                .HasForeignKey(d => d.AssignUserId)
                .HasConstraintName("fk_assign_sub_user_id");
        });

        modelBuilder.Entity<Task>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("tasks_pkey");

            entity.ToTable("tasks");

            entity.Property(e => e.TaskId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("task_id");
            entity.Property(e => e.TaskDescription).HasColumnName("task_description");
            entity.Property(e => e.TaskEnd)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("task_end");
            entity.Property(e => e.TaskStart)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("task_start");
            entity.Property(e => e.TaskStatusId).HasColumnName("task_status_id");

            entity.HasOne(d => d.TaskStatus).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.TaskStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_task_status");
        });

        modelBuilder.Entity<TaskComment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("task_comments_pkey");

            entity.ToTable("task_comments");

            entity.Property(e => e.CommentId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("comment_id");
            entity.Property(e => e.CommentName).HasColumnName("comment_name");
            entity.Property(e => e.CommentTaskId).HasColumnName("comment_task_id");
            entity.Property(e => e.CommentUserId).HasColumnName("comment_user_id");

            entity.HasOne(d => d.CommentTask).WithMany(p => p.TaskComments)
                .HasForeignKey(d => d.CommentTaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_comment_task_id");

            entity.HasOne(d => d.CommentUser).WithMany(p => p.TaskComments)
                .HasForeignKey(d => d.CommentUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_comment_user_id");
        });

        modelBuilder.Entity<TaskStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("task_status_pkey");

            entity.ToTable("task_status");

            entity.Property(e => e.StatusId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("status_id");
            entity.Property(e => e.StatusName)
                .HasMaxLength(50)
                .HasColumnName("status_name");
        });

        modelBuilder.Entity<TaskSubComment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("task_sub_comments_pkey");

            entity.ToTable("task_sub_comments");

            entity.Property(e => e.CommentId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("comment_id");
            entity.Property(e => e.CommentName).HasColumnName("comment_name");
            entity.Property(e => e.CommentSubTaskId).HasColumnName("comment_sub_task_id");
            entity.Property(e => e.CommentUserId).HasColumnName("comment_user_id");

            entity.HasOne(d => d.CommentSubTask).WithMany(p => p.TaskSubComments)
                .HasForeignKey(d => d.CommentSubTaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_sub_task_id");

            entity.HasOne(d => d.CommentUser).WithMany(p => p.TaskSubComments)
                .HasForeignKey(d => d.CommentUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_comment_user_id");
        });

        modelBuilder.Entity<TaskUser>(entity =>
        {
            entity.HasKey(e => e.TaskUserId).HasName("task_users_pkey");

            entity.ToTable("task_users");

            entity.Property(e => e.TaskUserId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("task_user_id");
            entity.Property(e => e.TaskAdmin).HasColumnName("task_admin");
            entity.Property(e => e.TaskId).HasColumnName("task_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Task).WithMany(p => p.TaskUsers)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_task_task_id");

            entity.HasOne(d => d.User).WithMany(p => p.TaskUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_user_id");
        });

        modelBuilder.Entity<TasksSub>(entity =>
        {
            entity.HasKey(e => e.SubTaskId).HasName("tasks_sub_pkey");

            entity.ToTable("tasks_sub");

            entity.Property(e => e.SubTaskId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("sub_task_id");
            entity.Property(e => e.TaskEnd)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("task_end");
            entity.Property(e => e.TaskId).HasColumnName("task_id");
            entity.Property(e => e.TaskName)
                .HasMaxLength(50)
                .HasColumnName("task_name");
            entity.Property(e => e.TaskStart)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("task_start");
            entity.Property(e => e.TaskStatusId).HasColumnName("task_status_id");

            entity.HasOne(d => d.Task).WithMany(p => p.TasksSubs)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_task_id");

            entity.HasOne(d => d.TaskStatus).WithMany(p => p.TasksSubs)
                .HasForeignKey(d => d.TaskStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_task_sub");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.UserId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("user_id");
            entity.Property(e => e.UserEmail)
                .HasMaxLength(100)
                .HasColumnName("user_email");
            entity.Property(e => e.UserName)
                .HasMaxLength(100)
                .HasColumnName("user_name");
            entity.Property(e => e.UserPassword).HasColumnName("user_password");
            entity.Property(e => e.UserSurname)
                .HasMaxLength(100)
                .HasColumnName("user_surname");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
