using Microsoft.EntityFrameworkCore;
using TaskTrackerClean.Domain.Entities;

namespace TaskTrackerClean.Infrastructure.Repositories;

public class AppDbContext : DbContext
{
    public DbSet<TaskEntity> Tasks { get; set; } = null!;
    public DbSet<UserEntity> Users { get; set; } = null!;
    public DbSet<ProjectEntity> Projects { get; set; } = null!;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserEntity>(entity =>
        {
            entity.HasIndex(u => u.Username)
                  .IsUnique();
            entity.HasIndex(u => u.Email)
                  .IsUnique();
        });

        modelBuilder.Entity<ProjectEntity>(entity =>
        {
            entity.HasIndex(p => p.Title)
                  .IsUnique();
        });
    }


}