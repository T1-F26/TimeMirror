
using Microsoft.EntityFrameworkCore;
using TimeMirror.Core.Models;

namespace TimeMirror.Infrastructure.Data;

public class TimeMirrorDbContext : DbContext
{
    public TimeMirrorDbContext(
        DbContextOptions<TimeMirrorDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Agenda> Agendas => Set<Agenda>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<ActivityRecord> ActivityRecords => Set<ActivityRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // USER: Primary key
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.UserID);

            entity.HasOne(u => u.Agenda)
                  .WithOne()
                  .HasForeignKey<Agenda>(a => a.UserID)
                  .IsRequired();

            entity.HasIndex(u => u.Email)
                  .IsUnique();
        });

        // AGENDA: Primary key and relationships
        modelBuilder.Entity<Agenda>(entity =>
        {
            entity.HasKey(a => a.AgendaID);

            entity.HasMany(a => a.Tasks)
                  .WithOne()
                  .HasForeignKey(t => t.AgendaID)
                  .IsRequired();
        });

        // TASK: Primary key
        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.HasKey(t => t.TaskID);
        });

        // ACTIVITY RECORD: Primary key and relationship
        modelBuilder.Entity<ActivityRecord>(entity =>
        {
            entity.HasKey(a => a.ActivityID);

            entity.HasOne<User>()
                  .WithMany()
                  .HasForeignKey(a => a.UserID)
                  .IsRequired();
        });
    }
}
