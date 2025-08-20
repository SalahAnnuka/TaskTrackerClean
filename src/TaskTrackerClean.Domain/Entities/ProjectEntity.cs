using System.ComponentModel.DataAnnotations;

namespace TaskTrackerClean.Domain.Entities;


public class ProjectEntity : BaseEntity<ProjectEntity>
{

    [Required, MaxLength(40)]
    public string Title { get; set; } = string.Empty;
    [Required, MaxLength(200)]
    public string Description { get; set; } = string.Empty;
    public ICollection<TaskEntity> Tasks { get; set; } = new List<TaskEntity>();
    public ICollection<UserEntity> Users { get; set; } = new List<UserEntity>();

}