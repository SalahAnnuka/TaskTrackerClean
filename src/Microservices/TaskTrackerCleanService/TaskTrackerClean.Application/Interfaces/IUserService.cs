using TaskTrackerClean.Application.Dtos;

namespace TaskTrackerClean.Application.Interfaces;

public interface IUserService
{
    Task<UserResponseDto> CreateAsync(CreateUserDto dto, string createdBy);
    Task<UserResponseDto> UpdateAsync(UpdateUserDto dto, string updatedBy);
    Task<UserResponseDto> FindByIdAsync(int id);
    Task<object> FindAsync(FindUserDto dto, int page, int pageSize, string? sortBy, string? sortAs);
    Task<UserResponseDto> DeleteAsync(int id);
    Task<UserResponseDto> RemoveTaskFromUser(int userId, int taskId);
    Task<UserResponseDto> AddTaskToUser(int userId, int taskId);
    Task<UserResponseDto> RecoverAsync(int id);
}