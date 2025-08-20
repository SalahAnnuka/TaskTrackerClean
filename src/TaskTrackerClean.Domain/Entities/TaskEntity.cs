using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskTrackerClean.Domain.Enums;

namespace TaskTrackerClean.Domain.Entities;

public class TaskEntity : BaseEntity<TaskEntity>
{

    [Required, MaxLength(40)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Description { get; set; } = string.Empty;

    [ForeignKey(nameof(User))]
    public int? UserId { get; set; }

    public UserEntity? User { get; set; }

    [ForeignKey(nameof(Project))]
    public int? ProjectId { get; set; }

    public ProjectEntity? Project { get; set; }

    [Range(1, 3)]
    public Priority Priority { get; set; }

    [Range(1, 3)]
    public Status Status { get; set; } = Status.NEW; //once a task is complete it cant be modified (except when deleted, foreign key removed)
    public DateTime StartDate { get; set; }
    public DateTime DueDate { get; set; }

    [NotMapped]
    public int? Duration { get {
            return Status == Status.COMPLETE?  (DueDate - StartDate).Days : null;
        }
    }

}