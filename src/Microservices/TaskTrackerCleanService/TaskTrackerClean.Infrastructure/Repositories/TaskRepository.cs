using Microsoft.EntityFrameworkCore;
using TaskTrackerClean.Domain.Entities;
using TaskTrackerClean.Domain.Enums;
using TaskTrackerClean.Domain.Interfaces;
namespace TaskTrackerClean.Infrastructure.Repositories;

public class TaskRepository : GenericRepository<TaskEntity>, ITaskRepository
{
    public TaskRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<TaskEntity>> FindOverdueAsync(int? userId, int? projectId)
    {
        var query = _dbSet.Where(t => !t.IsDeleted &&
                       t.DueDate < DateTime.UtcNow &&
                       t.Status != Status.COMPLETE);

        if (userId != null)
            query = query.Where(t => t.UserId == userId).Include(t => t.User);
        if (projectId != null)
            query = query.Where(t => t.ProjectId == projectId).Include(t => t.Project);

        return await query.ToListAsync();
    }

    public async Task<TaskEntity> SetCompleteAsync(int id)
    {
        var task = await _dbSet
            .FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);
        if (task == null)
        {
            throw new KeyNotFoundException($"Task with ID {id} was not found.");
        }
        if (task.Status == Status.COMPLETE)
        {
            throw new InvalidOperationException("Task is already completed.");
        }
        task.Status = Status.COMPLETE;
        task.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return task;
    }
}