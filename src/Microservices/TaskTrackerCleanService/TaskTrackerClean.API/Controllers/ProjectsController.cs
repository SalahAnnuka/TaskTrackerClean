using Microsoft.AspNetCore.Mvc;

using TaskTrackerClean.Application.Dtos;
using TaskTrackerClean.Application.Interfaces;

namespace TaskTrackerClean.API.Controllers;


[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpPost]
    public async Task<ActionResult<ProjectResponseDto>> Create([FromBody] CreateProjectDto dto)
    {
        var createdBy = "system"; 
        var result = await _projectService.CreateAsync(dto, createdBy);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProjectResponseDto>> Update([FromBody] UpdateProjectDto dto)
    {
        var updatedBy = "system";
        var result = await _projectService.UpdateAsync(dto, updatedBy);
        return result != null ? Ok(result) : NotFound($"Project with ID {dto.Id} not found");
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectResponseDto>> GetById(int id)
    {
        var result = await _projectService.FindByIdAsync(id);
        return result != null ? Ok(result) : NotFound($"Project with ID {id} not found");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectResponseDto>>> Find([FromQuery] ProjectBriefResponseDto dto, int? page, int? pageSize, string? sortBy, string? sortAs)
    {
        var results = await _projectService.FindAsync(dto, page ?? 1, pageSize ?? 10, sortBy, sortAs);
        return Ok(results);
    }



    [HttpPost("{projectId}/tasks/{taskId}")]
    public async Task<ActionResult<ProjectResponseDto>> AddTask(int projectId, int taskId)
    {
        var updated = await _projectService.AddTask(projectId, taskId);
        return updated != null ? Ok(updated) : NotFound("Either project or task does not exist");
    }


    [HttpDelete("{projectId}/tasks/{taskId}")]
    public async Task<ActionResult<ProjectResponseDto>> RemoveTask(int projectId, int taskId)
    {

        var updated = await _projectService.RemoveTask(projectId, taskId);
        return updated != null ? Ok(updated) : NotFound("Either project or task does not exist");

    }


    [HttpPost("{projectId}/users/{userId}")]
    public async Task<ActionResult<ProjectResponseDto>> AddUser(int projectId, int userId)
    {
        var updated = await _projectService.AddUser(projectId, userId);
        return updated != null ? Ok(updated) : NotFound("Either project or user does not exist");
    }


    [HttpDelete("{projectId}/users/{userId}")]
    public async Task<ActionResult<ProjectResponseDto>> RemoveUser(int projectId, int userId)
    {

        var updated = await _projectService.RemoveUser(projectId, userId);
        return updated != null? Ok(updated): NotFound("Either project or user does not exist");
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ProjectResponseDto>> Delete(int id)
    {
        var result = await _projectService.DeleteAsync(id);
        return result != null ? Ok(result) : NotFound($"Project with ID {id} not found");

    }

    [HttpPost("{id}/recover")]
    public async Task<ActionResult<ProjectResponseDto>> Recover(int id)
    {
        var result = await _projectService.RecoverAsync(id);
        return result != null ? Ok(result) : NotFound($"Project with ID {id} not found");
    }
}
