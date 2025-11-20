using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace TaskTracker.Entities;

public partial class TasktrackerContext : DbContext
{
    public TasktrackerContext()
    {
    }

    public TasktrackerContext(DbContextOptions<TasktrackerContext> options)
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8_polish_ci")
            .HasCharSet("utf8");

        modelBuilder.Entity<AssignSubTask>(entity =>
        {
            entity.HasKey(e => e.AssignId).HasName("PRIMARY");

            entity.ToTable("assign_sub_task");

            entity.HasIndex(e => e.AssignSubTaskId, "fk_assign_sub_task_id");

            entity.HasIndex(e => e.AssingUserId, "fk_assign_user_id");

            entity.Property(e => e.AssignId)
                .HasColumnType("int(11)")
                .HasColumnName("assign_id");
            entity.Property(e => e.AssignSubTaskId)
                .HasColumnType("int(11)")
                .HasColumnName("assign_sub_task_id");
            entity.Property(e => e.AssingUserId)
                .HasColumnType("int(11)")
                .HasColumnName("assing_user_id");

            entity.HasOne(d => d.AssignSubTaskNavigation).WithMany(p => p.AssignSubTasks)
                .HasForeignKey(d => d.AssignSubTaskId)
                .HasConstraintName("fk_assign_sub_task_id");

            entity.HasOne(d => d.AssingUser).WithMany(p => p.AssignSubTasks)
                .HasForeignKey(d => d.AssingUserId)
                .HasConstraintName("fk_assign_user_id");
        });

        modelBuilder.Entity<Task>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("PRIMARY");

            entity.ToTable("tasks");

            entity.HasIndex(e => e.TaskStatusId, "fk_task_status_id");

            entity.Property(e => e.TaskId)
                .HasColumnType("int(11)")
                .HasColumnName("task_id");
            entity.Property(e => e.TaskDescription)
                .HasColumnType("text")
                .HasColumnName("task_description");
            entity.Property(e => e.TaskEnd)
                .HasColumnType("datetime")
                .HasColumnName("task_end");
            entity.Property(e => e.TaskName)
                .HasMaxLength(100)
                .HasColumnName("task_name");
            entity.Property(e => e.TaskStart)
                .HasColumnType("datetime")
                .HasColumnName("task_start");
            entity.Property(e => e.TaskStatusId)
                .HasColumnType("int(11)")
                .HasColumnName("task_status_id");

            entity.HasOne(d => d.TaskStatus).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.TaskStatusId)
                .HasConstraintName("fk_task_status_id");
        });

        modelBuilder.Entity<TaskComment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PRIMARY");

            entity.ToTable("task_comments");

            entity.HasIndex(e => e.CommentTaskId, "fk_comment_task_id");

            entity.HasIndex(e => e.CommentUserId, "fk_main_comment_user_id");

            entity.Property(e => e.CommentId)
                .HasColumnType("int(11)")
                .HasColumnName("comment_id");
            entity.Property(e => e.CommentName)
                .HasColumnType("text")
                .HasColumnName("comment_name");
            entity.Property(e => e.CommentTaskId)
                .HasColumnType("int(11)")
                .HasColumnName("comment_task_id");
            entity.Property(e => e.CommentUserId)
                .HasColumnType("int(11)")
                .HasColumnName("comment_user_id");

            entity.HasOne(d => d.CommentTask).WithMany(p => p.TaskComments)
                .HasForeignKey(d => d.CommentTaskId)
                .HasConstraintName("fk_comment_task_id");

            entity.HasOne(d => d.CommentUser).WithMany(p => p.TaskComments)
                .HasForeignKey(d => d.CommentUserId)
                .HasConstraintName("fk_main_comment_user_id");
        });

        modelBuilder.Entity<TaskStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PRIMARY");

            entity.ToTable("task_status");

            entity.Property(e => e.StatusId)
                .HasColumnType("int(11)")
                .HasColumnName("status_id");
            entity.Property(e => e.StatusName)
                .HasMaxLength(50)
                .HasColumnName("status_name");
        });

        modelBuilder.Entity<TaskSubComment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PRIMARY");

            entity.ToTable("task_sub_comments");

            entity.HasIndex(e => e.CommentSubTaskId, "fk_comment_sub_task_id");

            entity.HasIndex(e => e.CommentUserId, "fk_comment_user_id");

            entity.Property(e => e.CommentId)
                .HasColumnType("int(11)")
                .HasColumnName("comment_id");
            entity.Property(e => e.CommentName)
                .HasColumnType("text")
                .HasColumnName("comment_name");
            entity.Property(e => e.CommentSubTaskId)
                .HasColumnType("int(11)")
                .HasColumnName("comment_sub_task_id");
            entity.Property(e => e.CommentUserId)
                .HasColumnType("int(11)")
                .HasColumnName("comment_user_id");

            entity.HasOne(d => d.CommentSubTask).WithMany(p => p.TaskSubComments)
                .HasForeignKey(d => d.CommentSubTaskId)
                .HasConstraintName("fk_comment_sub_task_id");

            entity.HasOne(d => d.CommentUser).WithMany(p => p.TaskSubComments)
                .HasForeignKey(d => d.CommentUserId)
                .HasConstraintName("fk_comment_user_id");
        });

        modelBuilder.Entity<TaskUser>(entity =>
        {
            entity.HasKey(e => e.TaskUserId).HasName("PRIMARY");

            entity.ToTable("task_users");

            entity.HasIndex(e => e.TaskId, "fk_main_task_id");

            entity.HasIndex(e => e.UserId, "fk_main_user_task");

            entity.Property(e => e.TaskUserId)
                .HasColumnType("int(11)")
                .HasColumnName("task_user_id");
            entity.Property(e => e.TaskAdmin).HasColumnName("task_admin");
            entity.Property(e => e.TaskId)
                .HasColumnType("int(11)")
                .HasColumnName("task_id");
            entity.Property(e => e.UserId)
                .HasColumnType("int(11)")
                .HasColumnName("user_id");

            entity.HasOne(d => d.Task).WithMany(p => p.TaskUsers)
                .HasForeignKey(d => d.TaskId)
                .HasConstraintName("fk_main_task_id");

            entity.HasOne(d => d.User).WithMany(p => p.TaskUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_main_user_task");
        });

        modelBuilder.Entity<TasksSub>(entity =>
        {
            entity.HasKey(e => e.SubTaskId).HasName("PRIMARY");

            entity.ToTable("tasks_sub");

            entity.HasIndex(e => e.TaskId, "fk_task_id");

            entity.HasIndex(e => e.TaskStatusId, "fk_task_status");

            entity.Property(e => e.SubTaskId)
                .HasColumnType("int(11)")
                .HasColumnName("sub_task_id");
            entity.Property(e => e.TaskDescription)
                .HasColumnType("text")
                .HasColumnName("task_description");
            entity.Property(e => e.TaskEnd)
                .HasColumnType("datetime")
                .HasColumnName("task_end");
            entity.Property(e => e.TaskId)
                .HasColumnType("int(11)")
                .HasColumnName("task_id");
            entity.Property(e => e.TaskName)
                .HasMaxLength(100)
                .HasColumnName("task_name");
            entity.Property(e => e.TaskStart)
                .HasColumnType("datetime")
                .HasColumnName("task_start");
            entity.Property(e => e.TaskStatusId)
                .HasColumnType("int(11)")
                .HasColumnName("task_status_id");

            entity.HasOne(d => d.Task).WithMany(p => p.TasksSubs)
                .HasForeignKey(d => d.TaskId)
                .HasConstraintName("fk_task_id");

            entity.HasOne(d => d.TaskStatus).WithMany(p => p.TasksSubs)
                .HasForeignKey(d => d.TaskStatusId)
                .HasConstraintName("fk_task_status");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("users");

            entity.Property(e => e.UserId)
                .HasColumnType("int(11)")
                .HasColumnName("user_id");
            entity.Property(e => e.UserEmail)
                .HasMaxLength(100)
                .HasColumnName("user_email");
            entity.Property(e => e.UserName)
                .HasMaxLength(100)
                .HasColumnName("user_name");
            entity.Property(e => e.UserPassword)
                .HasColumnType("text")
                .HasColumnName("user_password");
            entity.Property(e => e.UserSurname)
                .HasMaxLength(100)
                .HasColumnName("user_surname");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
