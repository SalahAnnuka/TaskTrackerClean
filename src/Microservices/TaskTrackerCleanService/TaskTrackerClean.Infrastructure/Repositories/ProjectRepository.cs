using Microsoft.EntityFrameworkCore;
using TaskTrackerClean.Domain.Entities;
using TaskTrackerClean.Domain.Interfaces;

namespace TaskTrackerClean.Infrastructure.Repositories;

public class ProjectRepository : GenericRepository<ProjectEntity>, IProjectRepository
{
    public ProjectRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<ProjectEntity>> FindProjectsWithProgressLessThan(double progress)
    {
        return await _dbSet
            .Include(p => p.Tasks)
            .Where(p => !p.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProjectEntity>> FindProjectsWithProgressMoreThan(double progress)
    {
        return await _dbSet
            .Include(p => p.Tasks)
            .Where(p => !p.IsDeleted)
            .ToListAsync();
    }

    public async Task<ProjectEntity> AddTaskToProjectAsync(int projectId, int taskId)
    {
        var project = await _dbSet
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == projectId && !p.IsDeleted);

        var task = await _context.Tasks.FindAsync(taskId);

        if (project != null && task != null && !task.IsDeleted)
        {
            task.ProjectId = projectId;
            project.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        return project!;
    }

    public async Task<ProjectEntity> RemoveTaskFromProjectAsync(int projectId, int taskId)
    {
        var project = await _dbSet
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == projectId && !p.IsDeleted);

        var task = await _context.Tasks.FindAsync(taskId);

        if (project != null && task != null)
        {
            task.ProjectId = null;
            project.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        return project!;
    }

    public async Task<ProjectEntity> AddUserToProjectAsync(int projectId, int userId)
    {
        var project = await _dbSet
            .Include(p => p.Users)
            .FirstOrDefaultAsync(p => p.Id == projectId && !p.IsDeleted);

        var user = await _context.Users.FindAsync(userId);

        if (project != null && user != null && !user.IsDeleted)
        {
            user.ProjectId = projectId;
            project.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        return project!;
    }

    public async Task<ProjectEntity> RemoveUserFromProjectAsync(int projectId, int userId)
    {
        var project = await _dbSet
            .Include(p => p.Users)
            .FirstOrDefaultAsync(p => p.Id == projectId && !p.IsDeleted);

        var user = await _context.Users.FindAsync(userId);

        if (project != null && user != null)
        {
            user.ProjectId = null;
            project.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        return project!;
    }
}
