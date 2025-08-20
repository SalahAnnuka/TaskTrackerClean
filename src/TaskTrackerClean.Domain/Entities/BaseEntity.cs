using System.ComponentModel.DataAnnotations;


namespace TaskTrackerClean.Domain.Entities
{
    public abstract class BaseEntity<TEntity>
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        [Required, MaxLength(40)]
        public string CreatedBy { get; set; } = string.Empty;
        [Required, MaxLength(40)]
        public string UpdatedBy { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;
    }
}