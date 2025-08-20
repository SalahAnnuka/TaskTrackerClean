using TaskTrackerClean.Application.Dtos;
using TaskTrackerClean.Application.Helpers;
using TaskTrackerClean.Application.Interfaces;
using TaskTrackerClean.Application.Mappers;
using TaskTrackerClean.Domain.Enums;
using TaskTrackerClean.Domain.Interfaces;

namespace TaskTrackerClean.Application.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;

    public TaskService(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<TaskResponseDto> CreateAsync(CreateTaskDto dto, string createdBy)
    {
        var entity = dto.ToEntity(createdBy);

        if (entity.StartDate > entity.DueDate)
        {
            throw new ArgumentException("Due date cannot predate start date, default due date is 7 days from current one");
        }

        var createdEntity = await _taskRepository.CreateAsync(entity);
        return createdEntity.ToResponseDto();
    }

    public async Task<TaskResponseDto> UpdateAsync(UpdateTaskDto dto, string updatedBy)
    {
        var existing = await _taskRepository.FindByIdAsync(dto.Id);
        if (existing == null)
            return null!;

        if (existing.Status == Status.COMPLETE)
        {
            throw new InvalidOperationException("You cannot modify a complete task");
        }

        existing.Name = dto.Name ?? existing.Name;
        existing.Description = dto.Description ?? existing.Description;
        existing.StartDate = dto.StartDate ?? existing.StartDate;
        existing.DueDate = dto.DueDate ?? existing.DueDate;
        existing.UpdatedBy = updatedBy;

        if (!string.IsNullOrEmpty(dto.PriorityText))
        {
            existing.Priority = dto.PriorityText != null? EnumHelper.GetPriority(dto.PriorityText) : existing.Priority;
        }

        if (existing.StartDate > existing.DueDate)
        {
            throw new ArgumentException("Due date cannot predate start date");
        }

        var updatedEntity = await _taskRepository.UpdateAsync(existing);
        return updatedEntity.ToResponseDto();
    }

    public async Task<TaskResponseDto> FindByIdAsync(int id)
    {
        var entity = await _taskRepository.FindByIdAsync(id);
        return entity.ToResponseDto();
    }

    public async Task<object> FindAsync(FindTaskDto dto, int page, int pageSize)
    {
        Priority? priority = dto.PriorityText != null? EnumHelper.GetPriority(dto.PriorityText) : null;
        Status? status = dto.StatusText != null? EnumHelper.GetStatus(dto.StatusText) : null;

        var (items, totalPages, totalItems) = await _taskRepository.FindWithIncludesAsync(
            page,
            pageSize,
            t => (dto.Id == 0 || t.Id == dto.Id) &&
                 (string.IsNullOrEmpty(dto.Name) || t.Name.Contains(dto.Name)) &&
                 (string.IsNullOrEmpty(dto.Description) || t.Description.Contains(dto.Description)) &&
                 (!priority.HasValue || t.Priority == priority) &&
                 (!status.HasValue || t.Status == status) &&
                 (!dto.StartDate.HasValue || t.StartDate.Date == dto.StartDate.Value.Date) &&
                 (!dto.DueDate.HasValue || t.DueDate.Date == dto.DueDate.Value.Date),
            null!
        );

        return Paginator.ToPagedResult(items.Select(t => t.ToBriefResponseDto()), page, pageSize, totalItems, totalPages);
    }


    public async Task<TaskResponseDto> DeleteAsync(int id)
    {
        var entity = await _taskRepository.DeleteAsync(id);
        return entity.ToResponseDto();
    }

    public async Task<TaskResponseDto> RecoverAsync(int id)
    {
        var entity = await _taskRepository.RecoverAsync(id);
        return entity.ToResponseDto();
    }

    public async Task<IEnumerable<TaskResponseDto>> FindOverdueAsync(int? userId, int? projectId)
    {
        var entities = await _taskRepository.FindOverdueAsync(userId, projectId);
        return entities.Select(e => e.ToResponseDto());
    }

    public async Task<TaskResponseDto> SetCompleteAsync(int id)
    {
        var entity = await _taskRepository.SetCompleteAsync(id);
        return entity.ToResponseDto();
    }
}