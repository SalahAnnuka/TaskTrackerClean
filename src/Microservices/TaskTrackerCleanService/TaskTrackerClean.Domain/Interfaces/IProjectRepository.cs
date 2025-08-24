using TaskTrackerClean.Domain.Entities;

namespace TaskTrackerClean.Domain.Interfaces;

public interface IProjectRepository : IGenericRepository<ProjectEntity>
{
    Task<IEnumerable<ProjectEntity>> FindProjectsWithProgressLessThan(double progress);
    Task<IEnumerable<ProjectEntity>> FindProjectsWithProgressMoreThan(double progress);
    Task<ProjectEntity> AddTaskToProjectAsync(int projectId, int taskId);
    Task<ProjectEntity> RemoveTaskFromProjectAsync(int projectId, int taskId);
    Task<ProjectEntity> AddUserToProjectAsync(int projectId, int userId);
    Task<ProjectEntity> RemoveUserFromProjectAsync(int projectId, int userId);
}