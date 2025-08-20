using TaskTrackerClean.Domain.Entities;

namespace TaskTrackerClean.Domain.Interfaces;

public interface ITaskRepository : IGenericRepository<TaskEntity>
{
    Task<IEnumerable<TaskEntity>> FindOverdueAsync(int? userId, int? projectId);
    Task<TaskEntity> SetCompleteAsync(int id);
}