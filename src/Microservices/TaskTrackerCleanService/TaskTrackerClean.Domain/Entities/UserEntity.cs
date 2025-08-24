using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskTrackerClean.Domain.Enums;

namespace TaskTrackerClean.Domain.Entities;

public class UserEntity : BaseEntity<UserEntity>
{
    [Required, MaxLength(40)]
    public string Username { get; set; } = string.Empty;

    [Required, MaxLength(40)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public string Salt { get; set; } = string.Empty;

    public ICollection<TaskEntity> Tasks { get; set; } = new List<TaskEntity>();

    [NotMapped]
    public int TasksNumber { get { 
            return this.Tasks.Count;
        }
    }

    [NotMapped]
    public int NumberOfCompletedTasks
    {
        get
        {
            return Tasks.Where(t => t.Status == Status.COMPLETE).Count();
        }
    }

    [ForeignKey(nameof(Project))]
    public int? ProjectId { get; set; }
    public ProjectEntity? Project { get; set; }
}
