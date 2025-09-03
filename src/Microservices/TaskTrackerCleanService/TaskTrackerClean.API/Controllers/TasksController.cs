using Microsoft.AspNetCore.Mvc;
using TaskTrackerClean.Application.Dtos;
    
using TaskTrackerClean.Application.Interfaces;


namespace TaskTrackerClean.API.Controllers;


[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpPost]
    public async Task<ActionResult<TaskResponseDto>> Create([FromBody] CreateTaskDto dto)
    {
        var createdBy = "system";
        var result = await _taskService.CreateAsync(dto, createdBy);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TaskResponseDto>> Update([FromBody] UpdateTaskDto dto)
    {
        var updatedBy = "system";
        var result = await _taskService.UpdateAsync(dto, updatedBy);
        return result != null ? Ok(result) : NotFound($"Task with ID {dto.Id} not found");
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskResponseDto>> GetById(int id)
    {
        var result = await _taskService.FindByIdAsync(id);
        return result != null ? Ok(result) : NotFound($"Task with ID {id} not found");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskResponseDto>>> Find([FromQuery] FindTaskDto dto, int? page, int? pageSize, string? sortBy, string? sortAs)
    {
        var results = await _taskService.FindAsync(dto, page ?? 1, pageSize ?? 10, sortBy, sortAs);
        return Ok(results);
    }

    [HttpGet("overdue")]
    public async Task<ActionResult<IEnumerable<TaskResponseDto>>> FindOverdue([FromQuery] int? userId, [FromQuery] int? projectId)
    {
        var results = await _taskService.FindOverdueAsync(userId, projectId);
        return Ok(results);
    }

    [HttpPost("{id}/complete")]
    public async Task<ActionResult<TaskResponseDto>> SetComplete(int id)
    {
        var result = await _taskService.SetCompleteAsync(id);
        return result != null ? Ok(result) : NotFound($"Task with ID {id} not found");
    }


    [HttpDelete("{id}")]
    public async Task<ActionResult<TaskResponseDto>> Delete(int id)
    {

        var result = await _taskService.DeleteAsync(id);
        return result != null ? Ok(result) : NotFound($"Task with ID {id} not found");
    }

    [HttpPost("{id}/recover")]
    public async Task<ActionResult<TaskResponseDto>> Recover(int id)
    {
        var result = await _taskService.RecoverAsync(id);
        return result != null ? Ok(result) : NotFound($"Task with ID {id} not found");
    }
}