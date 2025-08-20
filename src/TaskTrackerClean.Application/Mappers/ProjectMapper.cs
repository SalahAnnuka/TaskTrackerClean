using TaskTrackerClean.Domain.Entities;
using TaskTrackerClean.Application.Dtos;



namespace TaskTrackerClean.Application.Mappers
{
    public static class ProjectMapper
    {
        public static ProjectEntity ToEntity(this CreateProjectDto dto, string createdBy)
        {
            return new ProjectEntity
            {
                Title = dto.Title ?? "New Project",
                Description = dto.Description ?? string.Empty,
                CreatedBy = createdBy,
                UpdatedBy = createdBy,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public static ProjectResponseDto ToResponseDto(this ProjectEntity entity)
        {
            return new ProjectResponseDto
            {
                Id = entity.Id,
                Title = entity.Title,
                Description = entity.Description,
                CreatedAt = entity.CreatedAt,
                Tasks = entity.Tasks?.Select(t => t.ToBriefResponseDto()).ToList(),
                Users = entity.Users?.Select(u => u.ToBriefResponseDto()).ToList()
            };
        }

        public static BriefProjectDto ToBriefDto(this ProjectEntity entity)
        {
            return new BriefProjectDto
            {
                Id = entity.Id,
                Title = entity.Title,
                Description = entity.Description,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }
    }
}