using Microsoft.IdentityModel.Tokens;
using TaskTrackerClean.Application.Dtos;
using TaskTrackerClean.Application.Helpers;
using TaskTrackerClean.Application.Interfaces;
using TaskTrackerClean.Application.Mappers;
using TaskTrackerClean.Domain.Interfaces;

namespace TaskTrackerClean.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserResponseDto> CreateAsync(CreateUserDto dto, string createdBy)
    {
        var existing = await _userRepository.FindAsync(u => 
            u.Username == dto.Username ||
            u.Email == dto.Email
        );

        if (!existing.IsNullOrEmpty()) {
            throw new InvalidOperationException("User with this username/ Email already exists");
        }

        var entity = dto.ToEntity(createdBy);
        var createdEntity = await _userRepository.CreateAsync(entity);
        return createdEntity.ToResponseDto();
    }

    public async Task<UserResponseDto> UpdateAsync(UpdateUserDto dto, string updatedBy)
    {
        var existing = await _userRepository.FindByIdAsync(dto.Id);
        if (existing == null) return null!;

        existing.Username = dto.Username ?? existing.Username;
        existing.Email = dto.Email ?? existing.Email;
        existing.UpdatedBy = updatedBy;


        var updatedEntity = await _userRepository.UpdateAsync(existing);
        return updatedEntity.ToResponseDto();
    }

    public async Task<UserResponseDto> FindByIdAsync(int id)
    {
        var entity = await _userRepository.FindByIdAsync(id,
            u => u.Tasks);

        if (entity == null) return null!;

        return entity.ToResponseDto();
    }

    public async Task<object> FindAsync(FindUserDto dto, int page, int pageSize, string? sortBy, string? sortAs)
    {
        var (users, totalPages, totalItems) = await _userRepository.FindWithIncludesAsync(
            page,
            pageSize,
            sortBy,
            sortAs,
            u => (!dto.Id.HasValue || u.Id == dto.Id.Value) &&
                 (string.IsNullOrEmpty(dto.Username) || u.Username.Contains(dto.Username)) &&
                 (string.IsNullOrEmpty(dto.Email) || u.Email.Contains(dto.Email)),
            u => u.Tasks
        );

        var userDtos = users.Select(u => u.ToResponseDto());

        return Paginator.ToPagedResult(userDtos, page, pageSize, totalItems, totalPages);
    }


    public async Task<UserResponseDto> DeleteAsync(int id)
    {
        var entity = await _userRepository.DeleteAsync(id);
        if (entity == null) return null!;

        return entity.ToResponseDto();
    }

    public async Task<UserResponseDto> AddTaskToUser(int userId, int taskId)
    {
        var entity = await _userRepository.AddTaskToUserAsync(userId, taskId);
        if (entity == null) return null!;

        return entity.ToResponseDto();
    }

    public async Task<UserResponseDto> RemoveTaskFromUser(int userId, int taskId)
    {
        var entity = await _userRepository.RemoveTaskFromUserAsync(userId, taskId);
        if (entity == null) return null!;

        return entity.ToResponseDto();
    }

    public async Task<UserResponseDto> RecoverAsync(int id)
    {
        var entity = await _userRepository.RecoverAsync(id);
        if (entity == null) return null!;

        return entity.ToResponseDto();
    }

    public Task<object> FindAsync(FindUserDto dto, int page, int pageSize)
    {
        throw new NotImplementedException();
    }
}