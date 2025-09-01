using TaskTrackerClean.Domain.Entities;
using TaskTrackerClean.Domain.Enums;
using TaskTrackerClean.Application.Dtos;
using TaskTrackerClean.Application.Helpers;

namespace TaskTrackerClean.Application.Mappers;

public static class TaskMapper
{
    public static TaskEntity ToEntity(this CreateTaskDto dto, string createdBy)
    {
        return new TaskEntity
        {
            Name = dto.Name ?? "Untitled Task",
            Description = dto.Description ?? string.Empty,
            Priority = dto.PriorityText != null? EnumHelper.GetPriority(dto.PriorityText) ?? Priority.LOW : Priority.LOW,
            StartDate = dto.StartDate ?? DateTime.UtcNow,
            DueDate = dto.DueDate ?? DateTime.UtcNow.AddDays(3), 
            Status = Status.NEW,
            CreatedBy = createdBy,
            UpdatedBy = createdBy,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            UserId = null, 
            ProjectId = null 
        };
    }

    public static TaskResponseDto ToResponseDto(this TaskEntity entity)
    {
        return new TaskResponseDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            UserId = entity.UserId,
            ProjectId = entity.ProjectId,
            PriorityText = EnumHelper.GetPriorityText(entity.Priority),
            StatusText = EnumHelper.GetStatusText(entity.Status),
            StartDate = entity.StartDate,
            DueDate = entity.DueDate,
            Duration = entity.Duration,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    public static TaskBriefResponseDto ToBriefResponseDto(this TaskEntity entity)
    {
        return new TaskBriefResponseDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            PriorityText = EnumHelper.GetPriorityText(entity.Priority),
            StatusText = EnumHelper.GetStatusText(entity.Status),
            StartDate = entity.StartDate,
            DueDate = entity.DueDate,
            Duration = entity.Duration
        };
    }
}
