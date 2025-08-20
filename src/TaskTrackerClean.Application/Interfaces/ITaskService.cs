using TaskTrackerClean.Domain.Enums;
using TaskTrackerClean.Application.Dtos;

namespace TaskTrackerClean.Application.Interfaces;

public interface ITaskService
{
    Task<TaskResponseDto> CreateAsync(CreateTaskDto dto, string createdBy);
    Task<TaskResponseDto> UpdateAsync(UpdateTaskDto dto, string updatedBy);
    Task<TaskResponseDto> FindByIdAsync(int id);
    Task<object> FindAsync(FindTaskDto dto, int page, int pageSize);
    Task<TaskResponseDto> DeleteAsync(int id);
    Task<TaskResponseDto> RecoverAsync(int id);
    Task<IEnumerable<TaskResponseDto>> FindOverdueAsync(int? userId, int? projectId);
    Task<TaskResponseDto> SetCompleteAsync(int id);
}