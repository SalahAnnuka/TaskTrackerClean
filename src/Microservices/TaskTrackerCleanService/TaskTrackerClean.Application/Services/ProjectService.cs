using Microsoft.IdentityModel.Tokens;
using TaskTrackerClean.Application.Dtos;
using TaskTrackerClean.Application.Helpers;
using TaskTrackerClean.Application.Interfaces;
using TaskTrackerClean.Application.Mappers;
using TaskTrackerClean.Domain.Interfaces;

namespace TaskTrackerClean.Application.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;

    public ProjectService(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<ProjectResponseDto> CreateAsync(CreateProjectDto dto, string createdBy)
    {
        var existing = await _projectRepository.FindAsync(p =>
            p.Title == dto.Title
        );

        if (!existing.IsNullOrEmpty())
        {
            throw new InvalidOperationException("Project with this title already exists");
        }
        var entity = dto.ToEntity(createdBy);
        var createdEntity = await _projectRepository.CreateAsync(entity);
        return createdEntity.ToResponseDto();
    }

    public async Task<ProjectResponseDto> UpdateAsync(UpdateProjectDto dto, string updatedBy)
    {
        var existing = await _projectRepository.FindByIdAsync(dto.Id);
        existing.Title = dto.Title ?? existing.Title;
        existing.Description = dto.Description ?? existing.Description;
        existing.UpdatedBy = updatedBy;

        var updatedEntity = await _projectRepository.UpdateAsync(existing);
        return updatedEntity.ToResponseDto();
    }

    public async Task<ProjectResponseDto> FindByIdAsync(int id)
    {
        var entity = await _projectRepository.FindByIdAsync(id);
        return entity.ToResponseDto();
    }

    public async Task<object> FindAsync(FindProjectDto dto, int page, int pageSize)
    {
        var (projects, totalPages, totalItems) = await _projectRepository.FindWithIncludesAsync(
            page,
            pageSize,
            p => (!dto.Id.HasValue || p.Id == dto.Id) &&
                 (string.IsNullOrEmpty(dto.Title) || p.Title.Contains(dto.Title)),
            null!
        );

        var briefDtos = projects.Select(p => p.ToBriefDto());

        return Paginator.ToPagedResult(briefDtos, page, pageSize, totalItems, totalPages);
    }

    public async Task<ProjectResponseDto> AddTask(int projectId, int taskId)
    {

        var project = await _projectRepository.AddTaskToProjectAsync(projectId, taskId);
        return project.ToResponseDto();
    }

    public async Task<ProjectResponseDto> RemoveTask(int projectId, int taskId)
    {
        var project = await _projectRepository.RemoveTaskFromProjectAsync(projectId, taskId);

        return project.ToResponseDto();
    }

    public async Task<ProjectResponseDto> AddUser(int projectId, int userId)
    {
        var project = await _projectRepository.AddUserToProjectAsync(projectId, userId);
        return project.ToResponseDto();
    }

    public async Task<ProjectResponseDto> RemoveUser(int projectId, int userId)
    {
        var project = await _projectRepository.RemoveUserFromProjectAsync(projectId, userId);
        return project.ToResponseDto();
    }

    public async Task<ProjectResponseDto> DeleteAsync(int id)
    {
        var entity = await _projectRepository.DeleteAsync(id);
        return entity.ToResponseDto();
    }

    public async Task<ProjectResponseDto> RecoverAsync(int id)
    {
        var entity = await _projectRepository.RecoverAsync(id);
        return entity.ToResponseDto();
    }
}