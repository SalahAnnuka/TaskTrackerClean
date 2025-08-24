using Microsoft.AspNetCore.Mvc;
using TaskTrackerClean.Application.Dtos;
using TaskTrackerClean.Application.Interfaces;

namespace TaskTrackerClean.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    public async Task<ActionResult<UserResponseDto>> Create([FromBody] CreateUserDto dto)
    {
            var createdBy = "system";
            var result = await _userService.CreateAsync(dto, createdBy);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UserResponseDto>> Update([FromBody] UpdateUserDto dto)
    {
        var updatedBy = "system";
        var result = await _userService.UpdateAsync(dto, updatedBy);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponseDto>> GetById(int id)
    {
        var result = await _userService.FindByIdAsync(id);
        return result != null ? Ok(result) : NotFound($"User with ID {id} not found");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> Find([FromQuery] FindUserDto dto, int? page, int? pageSize )
    {
        var results = await _userService.FindAsync(dto, page ?? 1, pageSize ?? 10);
        return Ok(results);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<UserResponseDto>> Delete(int id)
    {
        var result = await _userService.DeleteAsync(id);
        return result != null ? Ok(result) : NotFound($"User with ID {id} not found");
    }

    [HttpPost("{id}/recover")]
    public async Task<ActionResult<UserResponseDto>> Recover(int id)
    {
        var result = await _userService.RecoverAsync(id);
        return result != null ? Ok(result) : NotFound($"User with ID {id} not found");
    }


    [HttpPost("{userId}/tasks/{taskId}")]
    public async Task<ActionResult<UserResponseDto>> AddTask(int userId, int taskId)
    {
        var updated = await _userService.AddTaskToUser(userId, taskId);
        return updated != null ? Ok(updated) : NotFound(new { message = $"User with ID {userId} or Task with ID {taskId} not found" });
    }


    [HttpDelete("{userId}/tasks/{taskId}")]
    public async Task<ActionResult<UserResponseDto>> RemoveTask(int userId, int taskId)
    {
        var updated = await _userService.RemoveTaskFromUser(userId, taskId);
        return updated != null ? Ok(updated) : NotFound(new { message = $"User with ID {userId} or Task with ID {taskId} not found" });
    }
}