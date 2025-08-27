namespace TaskTrackerClean.Application.Dtos
{
    public class UpdateProjectDto
    {
        public int Id { get; set; } = 0;
        public string? Title { get; set; }
        public string? Description { get; set; }

    }
}
