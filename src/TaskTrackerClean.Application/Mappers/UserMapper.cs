using TaskTrackerClean.Domain.Entities;
using TaskTrackerClean.Application.Dtos;


namespace TaskTrackerClean.Application.Mappers;

public static class UserMapper
{
    public static UserEntity ToEntity(this CreateUserDto dto, string createdBy)
    {
        return new UserEntity
        {
            Username = dto.Username ?? "N/A",
            Email = dto.Email ?? "N/A",
            PasswordHash = string.Empty,
            Salt = string.Empty,
            CreatedBy = createdBy,
            UpdatedBy = createdBy,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ProjectId = null
        };
    }

    public static UserResponseDto ToResponseDto(this UserEntity entity)
    {
        return new UserResponseDto
        {
            Id = entity.Id,
            Username = entity.Username,
            Email = entity.Email,
            Tasks = entity.Tasks?.Select(t => t.ToBriefResponseDto()).ToList() ?? new List<TaskBriefResponseDto>(),
            TasksNumber = entity.TasksNumber,
            CompletedTasksNumber = entity.NumberOfCompletedTasks,
            ProjectId = entity.ProjectId,
            CreatedAt = entity.CreatedAt
        };
    }

    public static UserBriefResponseDto ToBriefResponseDto(this UserEntity entity)
    {
        return new UserBriefResponseDto
        {
            Id = entity.Id,
            Username = entity.Username,
            Email = entity.Email
        };
    }
}