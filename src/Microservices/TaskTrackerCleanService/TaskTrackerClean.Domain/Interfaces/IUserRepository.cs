using TaskTrackerClean.Domain.Entities;

namespace TaskTrackerClean.Domain.Interfaces;

public interface IUserRepository : IGenericRepository<UserEntity>
{
    Task<UserEntity> AddTaskToUserAsync(int userId, int taskId);
    Task<UserEntity> RemoveTaskFromUserAsync(int userId, int taskId);
}