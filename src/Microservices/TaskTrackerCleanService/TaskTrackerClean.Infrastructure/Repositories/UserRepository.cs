using Microsoft.EntityFrameworkCore;
using TaskTrackerClean.Domain.Entities;
using TaskTrackerClean.Domain.Interfaces;
using TaskTrackerClean.Domain.Enums;

namespace TaskTrackerClean.Infrastructure.Repositories;

public class UserRepository : GenericRepository<UserEntity>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }

    public async Task<UserEntity> AddTaskToUserAsync(int userId, int taskId)
    {
        var user = await _dbSet
            .Include(u => u.Tasks)
            .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);

        var task = await _context.Tasks.FindAsync(taskId);


        if (user != null && task != null)
        {
            if (task.Status == Status.COMPLETE)
            {
                throw new InvalidOperationException("You cannot modify a complete task");
            }

            task.UserId = userId;
            task.Status = Status.IN_PROGRESS;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        return user!;
    }

    public async Task<UserEntity> RemoveTaskFromUserAsync(int userId, int taskId)
    {
        var user = await _dbSet
            .Include(u => u.Tasks)
            .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);

        var task = await _context.Tasks.FindAsync(taskId);

        if (task != null && task.Status == Status.COMPLETE)
        {
            throw new InvalidOperationException("You cannot modify a complete task");
        }

        if (user != null && task != null && !task.IsDeleted)
        {
            task.UserId = null;
            task.Status = Status.NEW;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        return user!;
    }
}