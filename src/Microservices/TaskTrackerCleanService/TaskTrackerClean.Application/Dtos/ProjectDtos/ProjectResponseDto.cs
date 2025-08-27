namespace TaskTrackerClean.Application.Dtos
{
    public class ProjectResponseDto
    {
        public int Id { get; set; } = 0;
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; }
        public ICollection<TaskBriefResponseDto>? Tasks { get; set; }
        public ICollection<UserBriefResponseDto>? Users { get; set; }
    }
}
