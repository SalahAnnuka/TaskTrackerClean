using TaskTrackerClean.Application.Dtos;

namespace TaskTrackerClean.Application.Interfaces
{
    public interface IProjectService
    {
        Task<ProjectResponseDto> CreateAsync(CreateProjectDto dto, string createdBy);
        Task<ProjectResponseDto> UpdateAsync(UpdateProjectDto dto, string updatedBy);
        Task<ProjectResponseDto> FindByIdAsync(int id);
        Task<object> FindAsync(ProjectBriefResponseDto dto, int page, int pageSize, string? sortBy, string? sortAs);
        Task<ProjectResponseDto> AddTask(int projectId, int taskId);
        Task<ProjectResponseDto> RemoveTask(int projectId, int taskId);
        Task<ProjectResponseDto> AddUser(int projectId, int userId);
        Task<ProjectResponseDto> RemoveUser(int projectId, int userId);
        Task<ProjectResponseDto> DeleteAsync(int id);
        Task<ProjectResponseDto> RecoverAsync(int id);
    }
}
